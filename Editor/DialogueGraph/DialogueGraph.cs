/*****************************************************************************
 * Foundation for this comes from                                            *
 * Mert Kirimgeri (https://www.youtube.com/@MertKirimgeriGameDev)            *
 *****************************************************************************/

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;

namespace InexperiencedDeveloper.Dialogue.Editor
{
    /// <summary>
    /// Window creation for the Dialogue Graph
    /// </summary>
    public class DialogueGraph : EditorWindow
    {
        private DialogueGraphView m_graphView;
        private string m_fileName = "New Dialogue Tree";

        [MenuItem("Graph/Dialogue Graph")]
        public static void OpenDialogueGraphWindow()
        {
            DialogueGraph window = GetWindow<DialogueGraph>();
            window.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            GenerateMinimap();
            //GenerateBlackboard();
        }

        private void ConstructGraphView()
        {
            m_graphView = new DialogueGraphView(this)
            {
                name = "Dialogue Graph"
            };

            m_graphView.StretchToParentSize();
            rootVisualElement.Add(m_graphView);
        }

        private void GenerateToolbar()
        {
            Toolbar toolbar = new Toolbar();

            TextField fileNameText = new TextField("File Name:");
            fileNameText.SetValueWithoutNotify(m_fileName);
            fileNameText.MarkDirtyRepaint();
            fileNameText.RegisterValueChangedCallback(e => m_fileName = e.newValue);
            toolbar.Add(fileNameText);

            Button saveButton = new Button(() => RequestSaveLoadData(true)) { text = "Save Data" };
            Button loadButton = new Button(() => RequestSaveLoadData(false)) { text = "Load Data" };
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);

            rootVisualElement.Add(toolbar);
        }

        private void GenerateMinimap()
        {
            MiniMap minimap = new MiniMap { anchored = true };
            minimap.SetPosition(new Rect(10, 30, 200, 140));
            m_graphView.Add(minimap);
        }

        private void GenerateBlackboard()
        {
            Blackboard blackboard = new Blackboard(m_graphView);
            blackboard.Add(new BlackboardSection { title = "Exposed Properties" });
            blackboard.addItemRequested = board => m_graphView.AddPropertyToBlackboard(new ExposedProperty());
            blackboard.editTextRequested = (board, element, newValue) =>
            {
                string oldPropertyName = ((BlackboardField)element).text;
                if (m_graphView.ExposedProperties.Any(x => x.PropertyName == newValue))
                {
                    EditorUtility.DisplayDialog("Error", "This property name already exists.", "OK");
                    return;
                }
                int propertyIndex = m_graphView.ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
                m_graphView.ExposedProperties[propertyIndex].PropertyName = newValue;
                ((BlackboardField)element).text = newValue;
            };
            m_graphView.Add(blackboard);
            m_graphView.SetBlackboard(blackboard);
        }

        private void RequestSaveLoadData(bool shouldSave)
        {
            if (string.IsNullOrEmpty(m_fileName))
            {
                EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name.", "OK");
                return;
            }

            GraphSaveUtility saveUtility = GraphSaveUtility.GetInstance(m_graphView);

            if (shouldSave) saveUtility.SaveGraph(m_fileName);
            else saveUtility.LoadGraph(m_fileName);
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(m_graphView);
        }
    }
}   