/*****************************************************************************
 * Foundation for this comes from                                            *
 * Mert Kirimgeri (https://www.youtube.com/@MertKirimgeriGameDev)            *
 * Updates by Jacob Berman (https://www.youtube.com/@InexperiencedDeveloper) *
 *****************************************************************************/

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace InexperiencedDeveloper.Dialogue.Editor
{
    /// <summary>
    /// Search window allowing for quick-finding of nodes for the DialogueGraph
    /// </summary>
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private EditorWindow m_window;
        private DialogueGraphView m_graphView;
        private Texture2D m_indentationIcon;

        public void Init(EditorWindow window, DialogueGraphView graphView)
        {
            m_window = window;
            m_graphView = graphView;

            m_indentationIcon = new Texture2D(1, 1);
            m_indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            m_indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
            new SearchTreeGroupEntry(new GUIContent("Dialogue"), 1),
            new SearchTreeEntry(new GUIContent("Dialogue Node", m_indentationIcon))
            {
                userData = new DialogueNode(),
                level = 2,
            },
            new SearchTreeEntry(new GUIContent("Event Node", m_indentationIcon))
            {
                userData = new UnityEventNode(),
                level = 2,
            },
        };

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext ctx)
        {
            Vector2 mousePosWS = m_window.rootVisualElement.ChangeCoordinatesTo(
                m_window.rootVisualElement.parent,
                ctx.screenMousePosition - m_window.position.position);
            Vector2 mousePosLocal = m_graphView.contentContainer.WorldToLocal(mousePosWS);

            return searchTreeEntry.userData switch
            {
                DialogueNode dialogueNode => CreateDialogueNode(mousePosLocal),
                UnityEventNode eventNode => CreateEventNode(mousePosLocal),
                _ => false
            };
        }

        private bool CreateDialogueNode(Vector2 pos)
        {
            m_graphView.CreateNode("Dialogue Node", pos, ENodeType.DIALOGUE);
            return true;
        }

        private bool CreateEventNode(Vector2 pos)
        {
            m_graphView.CreateNode("Event Node", pos, ENodeType.EVENT);
            return true;
        }
    }
}