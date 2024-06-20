/*****************************************************************************
 * Created by:                                                               *
 * Mert Kirimgeri (https://www.youtube.com/@MertKirimgeriGameDev)            *
 *****************************************************************************/

using System;
using UnityEngine;

namespace InexperiencedDeveloper.Dialogue
{
    /// <summary>
    /// Dialogue data container for the runtime/serialization side of the Dialogue System
    /// </summary>
    [Serializable]
    public class DialogueNodeData : NodeData
    {
        public string DialogueText;

        public DialogueNodeData() { }
        public DialogueNodeData(string guid, string text, Vector2 pos)
        {
            Guid = guid;
            DialogueText = text;
            Position = pos;
        }
    }
}