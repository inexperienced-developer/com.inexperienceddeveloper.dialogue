/*****************************************************************************
 * Foundation for this comes from                                            *
 * Mert Kirimgeri (https://www.youtube.com/@MertKirimgeriGameDev)            *
 * Updates by Jacob Berman (https://www.youtube.com/@InexperiencedDeveloper) *
 *****************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace InexperiencedDeveloper.Dialogue.Editor
{
    /// <summary>
    /// Saves and loads the dialogue graph into scriptable objects at the set path
    /// </summary>
    public class GraphSaveUtility
    {
        private const string save_path = "Assets/DialogueTrees";

        private DialogueGraphView m_targetGraphView;
        private DialogueContainerSO m_containerCache;

        private List<Edge> m_edges => m_targetGraphView.edges.ToList();
        private List<CustomNode> m_nodes => m_targetGraphView.nodes.ToList().Cast<CustomNode>().ToList();

        public static GraphSaveUtility GetInstance(DialogueGraphView dialogueGraphView)
        {
            return new GraphSaveUtility
            {
                m_targetGraphView = dialogueGraphView,
            };
        }
        private bool SaveNodes(DialogueContainerSO dialogueContainer)
        {
            if (!m_edges.Any()) return false; //No connections


            Edge[] connectedPorts = m_edges.Where(x => x.input.node != null).ToArray();
            for (int i = 0; i < connectedPorts.Length; i++)
            {
                CustomNode outNode = connectedPorts[i].output.node as CustomNode;
                CustomNode inNode = connectedPorts[i].input.node as CustomNode;
                dialogueContainer.NodeLinks.Add(new NodeLinkData(outNode.Guid, connectedPorts[i].output.portName, inNode.Guid));
            }

            foreach (DialogueNode node in m_nodes.Where(node => node is DialogueNode && !((DialogueNode)node).EntryPoint))
            {
                dialogueContainer.DialogueNodeData.Add(new DialogueNodeData(node.Guid, node.DialogueText, node.GetPosition().position));
            }

            foreach (UnityEventNode node in m_nodes.Where(node => node is UnityEventNode))
            {
                dialogueContainer.UnityEventNodeData.Add(new UnityEventNodeData(node.Guid, node.EventTitle, node.GetPosition().position));
            }

            return true;
        }

        private void SaveExposedProperties(DialogueContainerSO dialogueContainer)
        {
            dialogueContainer.ExposedProperties.AddRange(m_targetGraphView.ExposedProperties);
        }

        public void SaveGraph(string fileName)
        {
            DialogueContainerSO dialogueContainer = ScriptableObject.CreateInstance<DialogueContainerSO>();

            if (!SaveNodes(dialogueContainer)) return;
            SaveExposedProperties(dialogueContainer);
            if (!AssetDatabase.IsValidFolder(save_path))
            {
                string[] folders = save_path.Split('/');
                AssetDatabase.CreateFolder(folders[0], folders[1]);
            }

            AssetDatabase.CreateAsset(dialogueContainer, $"{save_path}/{fileName}.asset"); // Saves the asset for use in game
            AssetDatabase.SaveAssets();
        }

        public void LoadGraph(string fileName)
        {
            m_containerCache = AssetDatabase.LoadAssetAtPath<DialogueContainerSO>($"{save_path}/{fileName}.asset");
            if (m_containerCache == null)
            {
                EditorUtility.DisplayDialog("File not found.", $"{fileName} dialogue graph does not exist.", "OK");
                return;
            }

            ClearGraph();
            CreateNodes();
            ConnectNodes();
            //CreateExposedProperties();
        }

        private void ClearGraph()
        {
            // Find entry point
            m_nodes.Find(node => node is DialogueNode && ((DialogueNode)node).EntryPoint).Guid = m_containerCache.NodeLinks[0].BaseNodeGuid;

            // Remove all connections and nodes from graph
            foreach (CustomNode node in m_nodes)
            {
                if (node is DialogueNode && ((DialogueNode)node).EntryPoint) continue;
                m_edges.Where(x => x.input.node == node).ToList()
                    .ForEach(edge => m_targetGraphView.RemoveElement(edge));
                m_targetGraphView.RemoveElement(node);
            }
        }

        private void CreateNodes()
        {
            foreach (DialogueNodeData data in m_containerCache.DialogueNodeData)
            {
                // Position is changed later in ConnectNodes()
                DialogueNode node = m_targetGraphView.CreateDialogueNode(data.DialogueText, Vector2.zero);
                node.Guid = data.Guid;
                m_targetGraphView.AddElement(node);

                // Add choice ports
                var nodePorts = m_containerCache.NodeLinks.Where(x => x.BaseNodeGuid == data.Guid).ToList();
                nodePorts.ForEach(x => m_targetGraphView.AddChoicePort(node, x.PortName));
            }

            foreach (UnityEventNodeData data in m_containerCache.UnityEventNodeData)
            {
                // Position is changed later in ConnectNodes()
                UnityEventNode node = m_targetGraphView.CreateEventNode(data.EventTitle, Vector2.zero);
                node.Guid = data.Guid;
                m_targetGraphView.AddElement(node);

                // Add choice ports
                var nodePorts = m_containerCache.NodeLinks.Where(x => x.BaseNodeGuid == data.Guid).ToList();
                nodePorts.ForEach(x => m_targetGraphView.AddChoicePort(node, x.PortName));
            }
        }

        private void ConnectNodes()
        {
            for (int i = 0; i < m_nodes.Count; i++)
            {
                List<NodeLinkData> connections = m_containerCache.NodeLinks.Where(x => x.BaseNodeGuid == m_nodes[i].Guid).ToList();
                for (int j = 0; j < connections.Count; j++)
                {
                    string targetNodeGuid = connections[j].TargetNodeGuid;
                    CustomNode node = m_nodes.First(x => x.Guid == targetNodeGuid);
                    LinkNodes(m_nodes[i].outputContainer[j].Q<Port>(), (Port)node.inputContainer[0]);
                    Vector3 pos = node switch
                    {
                        UnityEventNode => m_containerCache.UnityEventNodeData.First(x => x.Guid == targetNodeGuid).Position,
                        _ => m_containerCache.DialogueNodeData.First(x => x.Guid == targetNodeGuid).Position,
                    };
                    node.SetPosition(new Rect(pos, m_targetGraphView.r_DefaultNodeSize));
                }
            }
        }

        private void CreateExposedProperties()
        {
            m_targetGraphView.ClearBlackboardAndExposedProperties();
            foreach (ExposedProperty prop in m_containerCache.ExposedProperties)
            {
                m_targetGraphView.AddPropertyToBlackboard(prop);
            }
        }

        private void LinkNodes(Port output, Port input)
        {
            Edge edge = new Edge
            {
                output = output,
                input = input,
            };

            edge?.input.Connect(edge);
            edge?.output.Connect(edge);
            m_targetGraphView.Add(edge);
        }
    }
}
