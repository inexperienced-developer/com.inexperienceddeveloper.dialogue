/*****************************************************************************
 * Based on DialogueNodeData by:                                             *
 * Mert Kirimgeri (https://www.youtube.com/@MertKirimgeriGameDev)            *
 * Created by Jacob Berman (https://www.youtube.com/@InexperiencedDeveloper) *
 *****************************************************************************/

using System;
using UnityEngine;

namespace InexperiencedDeveloper.Dialogue
{
    /// <summary>
    /// Serializes information to call an event from a node on the graph in the
    /// Dialogue System
    /// </summary>
    [Serializable]
    public class UnityEventNodeData : NodeData
    {
        // TODO: change to index
        public string EventTitle;

        public UnityEventNodeData() { }
        public UnityEventNodeData(string guid, string title, Vector2 pos)
        {
            Guid = guid;
            EventTitle = title;
            Position = pos;
        }
    }
}

