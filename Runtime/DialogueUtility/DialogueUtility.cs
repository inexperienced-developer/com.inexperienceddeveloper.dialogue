/*****************************************************************************
 * Created by Jacob Berman (https://www.youtube.com/@InexperiencedDeveloper) *
 * Runtime utility to work with ScriptableObject-based Dialogue System       *
 * Which was created by:                                                     *
 * Mert Kirimgeri (https://www.youtube.com/@MertKirimgeriGameDev)            *
 *****************************************************************************/

using System.Collections.Generic;

namespace InexperiencedDeveloper.Dialogue
{
    public enum EDialogueType
    {
        DIALOGUE,
        EVENT
    }

    /// <summary>
    /// Utility class that allows for traversing the branches of the nodes in the Dialogue graph
    /// allowing them to be utilized in game.
    /// </summary>
    public static class DialogueUtility
    {
        public static DialogueEvent GetNextDialogueEvent(this DialogueContainerSO container, DialogueSegment selectedNode = null)
        {
            // If this is the first dialogue event, set our base to the first node
            string baseGuid = selectedNode == null ? container.NodeLinks[1].BaseNodeGuid : selectedNode.Guid;

            // Get the targetGuids from the lastDialogueEvent
            string[] targetGuids = container.GetTargetNodeGuids(baseGuid);

            (NodeData data, EDialogueType type) node = container.GetNodeData(baseGuid);
            string baseNodeText = node.data switch
            {
                UnityEventNodeData => (node.data as UnityEventNodeData).EventTitle,
                _ => (node.data as DialogueNodeData).DialogueText
            };
            DialogueSegment baseNode = new DialogueSegment(baseGuid, baseNodeText);
            // If we have no more targets then we have reached the end
            if (targetGuids.Length == 0)
            {
                return new DialogueEvent(baseNode, node.type);
            }

            List<DialogueSegment> targetNodes = new List<DialogueSegment>();
            foreach (string guid in targetGuids)
            {
                string text = container.GetTargetNodeText(guid);
                targetNodes.Add(new DialogueSegment(guid, text));
            }

            return new DialogueEvent(baseNode, targetNodes.ToArray(), node.type);
        }

        private static string[] GetTargetNodeGuids(this DialogueContainerSO container, string baseGuid)
        {
            List<string> targetGuids = new List<string>();
            foreach (NodeLinkData node in container.NodeLinks)
            {
                if (node.BaseNodeGuid == baseGuid) targetGuids.Add(node.TargetNodeGuid);
            }
            return targetGuids.ToArray();
        }

        private static string GetTargetNodeText(this DialogueContainerSO container, string guid)
        {
            foreach (NodeLinkData node in container.NodeLinks)
            {
                if (node.TargetNodeGuid == guid) return node.PortName;
            }
            return null;
        }

        private static (NodeData data, EDialogueType type) GetNodeData(this DialogueContainerSO container, string guid)
        {
            foreach (DialogueNodeData node in container.DialogueNodeData)
            {
                if (node.Guid == guid) return (node, EDialogueType.DIALOGUE);
            }
            foreach (UnityEventNodeData node in container.UnityEventNodeData)
            {
                if (node.Guid == guid) return (node, EDialogueType.EVENT);
            }
            return (null, EDialogueType.DIALOGUE);
        }
    }

    public class DialogueEvent
    {
        public readonly DialogueSegment BaseNode;
        public readonly DialogueSegment[] TargetNodes = null;
        public readonly EDialogueType DialogueType;

        public DialogueEvent()
        {
            BaseNode = null;
        }

        public DialogueEvent(DialogueSegment baseNode, EDialogueType dialogueType)
        {
            BaseNode = baseNode;
            DialogueType = dialogueType;
        }

        public DialogueEvent(DialogueSegment baseNode, DialogueSegment[] targetNodes, EDialogueType dialogueType)
        {
            BaseNode = baseNode;
            TargetNodes = targetNodes;
            DialogueType = dialogueType;
        }
    }

    public class DialogueSegment
    {
        public readonly string Guid;
        public readonly string Text;

        public DialogueSegment(string guid, string text)
        {
            Guid = guid;
            Text = text;
        }
    }
}
