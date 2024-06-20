/*****************************************************************************
 * Foundation for this comes from                                            *
 * Mert Kirimgeri (https://www.youtube.com/@MertKirimgeriGameDev)            *
 * Updates by Jacob Berman (https://www.youtube.com/@InexperiencedDeveloper) *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace InexperiencedDeveloper.Dialogue.Editor
{
    public enum ENodeType
    {
        DIALOGUE,
        EVENT
    }

    /// <summary>
    /// GraphView extension that allows for dialogue and event nodes to be added to the DialogueGraph
    /// </summary>
    public class DialogueGraphView : GraphView
    {
        public readonly Vector2 r_DefaultNodeSize = new Vector2(150, 200);

        public Blackboard Blackboard { get; private set; }
        public void SetBlackboard(Blackboard blackboard) => Blackboard = blackboard;
        public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
        private NodeSearchWindow m_searchWindow;

        public DialogueGraphView(EditorWindow window)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GenerateEntryPointNode());
            AddSearchWindow(window);
        }

        private void AddSearchWindow(EditorWindow window)
        {
            m_searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            m_searchWindow.Init(window, this);
            nodeCreationRequest = ctx => SearchWindow.Open(new SearchWindowContext(ctx.screenMousePosition), m_searchWindow);
        }

        private DialogueNode GenerateEntryPointNode()
        {
            DialogueNode node = new DialogueNode
            {
                title = "Start",
                Guid = Guid.NewGuid().ToString(),
                DialogueText = "",
                EntryPoint = true
            };

            AddPort(node, Direction.Output, "Next");

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;
            node.SetPosition(new Rect(100, 200, 100, 150));

            return node;
        }

        public DialogueNode CreateDialogueNode(string nodeName, Vector2 pos)
        {
            DialogueNode node = new DialogueNode
            {
                title = nodeName,
                DialogueText = nodeName,
                Guid = Guid.NewGuid().ToString(),
            };


            node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            Button addChoiceBtn = new Button(() => AddChoicePort(node));
            addChoiceBtn.text = "New Choice";
            node.titleContainer.Add(addChoiceBtn);

            TextField textField = new TextField(99999, true, false, '*');
            textField.RegisterValueChangedCallback(e =>
            {
                node.DialogueText = e.newValue;
                node.title = e.newValue.Length > 10 ? e.newValue.Substring(0, 10) + "..." : e.newValue;
            });
            textField.SetValueWithoutNotify(node.title);
            node.mainContainer.Add(textField);

            AddPort(node, Direction.Input, "Input", Port.Capacity.Multi);
            node.SetPosition(new Rect(pos, r_DefaultNodeSize));
            return node;
        }

        public UnityEventNode CreateEventNode(string nodeName, Vector2 pos)
        {
            UnityEventNode node = new UnityEventNode
            {
                title = nodeName,
                EventTitle = nodeName,
                Guid = Guid.NewGuid().ToString(),
            };
            node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            Button addChoiceBtn = new Button(() => AddChoicePort(node));
            addChoiceBtn.text = "New Choice";
            node.titleContainer.Add(addChoiceBtn);

            TextField textField = new TextField(string.Empty);
            textField.RegisterValueChangedCallback(e =>
            {
                node.EventTitle = e.newValue;
                node.title = e.newValue.Length > 10 ? e.newValue.Substring(0, 10) + "..." : e.newValue;
            });
            textField.SetValueWithoutNotify(node.title);
            node.mainContainer.Add(textField);
            AddPort(node, Direction.Input, "Input", Port.Capacity.Multi);
            node.SetPosition(new Rect(pos, r_DefaultNodeSize));
            return node;
        }

        public void CreateNode(string nodeName, Vector2 pos, ENodeType type)
        {
            CustomNode node = type switch
            {
                ENodeType.EVENT => CreateEventNode(nodeName, pos),
                _ => CreateDialogueNode(nodeName, pos),
            };
            AddElement(node);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node) compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        private Port GeneratePort(CustomNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }

        private void AddPort(CustomNode node, Direction portDirection, string portName, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = GeneratePort(node, portDirection, capacity);
            port.portName = portName;

            Action addToContainer = portDirection switch
            {
                Direction.Input => () => node.inputContainer.Add(port),
                _ => () => node.outputContainer.Add(port),
            };

            addToContainer();
            node.RefreshExpandedState();
            node.RefreshPorts();
        }

        private void AddRemovablePort(CustomNode node, Direction portDirection, string portName, TextField textField, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = GeneratePort(node, portDirection, capacity);


            textField.RegisterValueChangedCallback(e => port.portName = e.newValue);
            port.contentContainer.Add(new Label(" "));
            port.contentContainer.Add(textField);
            Button deleteBtn = new Button(() => RemovePort(node, port)) { text = "X" };
            port.contentContainer.Add(deleteBtn);

            port.portName = portName;
            Action addToContainer = portDirection switch
            {
                Direction.Input => () => node.inputContainer.Add(port),
                _ => () => node.outputContainer.Add(port),
            };
            addToContainer();

            node.RefreshExpandedState();
            node.RefreshPorts();
        }

        private void RemovePort(CustomNode node, Port port)
        {
            List<Edge> edgeList = edges.ToList();
            List<Edge> targetEdge = edges.ToList().Where(x => x.output.portName == port.portName && x.output.node == node).ToList();
            if (targetEdge.Count == 0) return;
            Edge edge = targetEdge[0];
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge[0]);

            node.outputContainer.Remove(port);

            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private void AddChoicePort(CustomNode node)
        {
            int outputPortCount = node.outputContainer.Query("connector").ToList().Count;
            AddChoicePort(node, $"{(char)(65 + outputPortCount)}");
        }

        public void AddChoicePort(CustomNode node, string portName)
        {
            TextField textField = new TextField
            {
                name = string.Empty,
                value = portName
            };
            textField.style.maxWidth = 100;
            AddRemovablePort(node, Direction.Output, portName, textField);
        }

        public void AddPropertyToBlackboard(ExposedProperty exposedProperty)
        {
            string propertyName = exposedProperty.PropertyName;
            while (ExposedProperties.Any(x => x.PropertyName == propertyName))
            {
                propertyName = $"{propertyName}(1)";
            }

            ExposedProperty property = new ExposedProperty
            {
                PropertyName = propertyName,
                PropertyValue = exposedProperty.PropertyValue
            };

            ExposedProperties.Add(property);

            VisualElement container = new VisualElement();
            BlackboardField field = new BlackboardField { text = property.PropertyName, typeText = "string" };
            container.Add(field);

            TextField propertyValueText = new TextField("Value:")
            {
                value = property.PropertyValue,
            };
            propertyValueText.RegisterValueChangedCallback(e =>
            {
                int propertyChangedIndex = ExposedProperties.FindIndex(x => x.PropertyName == property.PropertyName);
                ExposedProperties[propertyChangedIndex].PropertyValue = e.newValue;
            });
            BlackboardRow valueRow = new BlackboardRow(field, propertyValueText);
            container.Add(valueRow);

            Blackboard.Add(container);
        }

        public void ClearBlackboardAndExposedProperties()
        {
            ExposedProperties.Clear();
            Blackboard.Clear();
        }
    }
}
