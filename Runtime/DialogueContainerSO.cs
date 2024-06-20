/*****************************************************************************
 * Foundation for this comes from                                            *
 * Mert Kirimgeri (https://www.youtube.com/@MertKirimgeriGameDev)            *
 * Updates by Jacob Berman (https://www.youtube.com/@InexperiencedDeveloper) *
 *****************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace InexperiencedDeveloper.Dialogue
{
    /// <summary>
    /// Holds all saved information for nodes on DialogueGraph
    /// </summary>
    public class DialogueContainerSO : ScriptableObject
    {
        public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
        public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
        public List<UnityEventNodeData> UnityEventNodeData = new List<UnityEventNodeData>();
        public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
    }
}
