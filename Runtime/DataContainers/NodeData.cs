/*****************************************************************************
 * Created by Jacob Berman (https://www.youtube.com/@InexperiencedDeveloper) *
 * General data container to work with ScriptableObject-based Dialogue       *
 * System which was created by:                                              *
 * Mert Kirimgeri (https://www.youtube.com/@MertKirimgeriGameDev)            *
 *****************************************************************************/

using System;
using UnityEngine;

namespace InexperiencedDeveloper.Dialogue
{
    /// <summary>
    /// Generic runtime data for Nodes, to provide general serialization with
    /// GUID and node's graph position data
    /// </summary>
    [Serializable]
    public class NodeData
    {
        public string Guid;
        public Vector2 Position;
    }
}
