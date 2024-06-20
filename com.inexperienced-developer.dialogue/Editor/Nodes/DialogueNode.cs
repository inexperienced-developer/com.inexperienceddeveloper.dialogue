/*****************************************************************************
 * Foundation for this comes from                                            *
 * Mert Kirimgeri (https://www.youtube.com/@MertKirimgeriGameDev)            *
 * Updates by Jacob Berman (https://www.youtube.com/@InexperiencedDeveloper) *
 *****************************************************************************/

namespace InexperiencedDeveloper.Dialogue.Editor
{
    /// <summary>
    /// Used to store text in the dialogue system
    /// </summary>
    public class DialogueNode : CustomNode
    {
        public string DialogueText;
        public bool EntryPoint = false;
    }
}
