/*****************************************************************************
 * Created by:                                                               *
 * Mert Kirimgeri (https://www.youtube.com/@MertKirimgeriGameDev)            *
 *****************************************************************************/

using System;

namespace InexperiencedDeveloper.Dialogue
{
    /// <summary>
    /// Used to save connections between nodes on the graph in the Dialogue System
    /// </summary>
    [Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string PortName;
        public string TargetNodeGuid;

        public NodeLinkData() { }
        public NodeLinkData(string baseGuid, string portName, string targetGuid)
        {
            BaseNodeGuid = baseGuid;
            PortName = portName;
            TargetNodeGuid = targetGuid;
        }
    }
}
