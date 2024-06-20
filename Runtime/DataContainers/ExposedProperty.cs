/*****************************************************************************
 * Created by:                                                               *
 * Mert Kirimgeri (https://www.youtube.com/@MertKirimgeriGameDev)            *
 *****************************************************************************/

using System;

namespace InexperiencedDeveloper.Dialogue
{
    /// <summary>
    /// Basic string property used for variable serialization and to populate the
    /// blackboard on the DialogueGraph
    /// </summary>
    [Serializable]
    public class ExposedProperty
    {
        public string PropertyName = "name";
        public string PropertyValue = "value";
    }
}
