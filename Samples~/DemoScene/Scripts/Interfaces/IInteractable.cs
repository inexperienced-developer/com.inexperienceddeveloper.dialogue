using UnityEngine;

public interface IInteractable
{
    Transform transform { get; }
    /// <summary>
    /// Returns true if camera should focus on interactable or not
    /// </summary>
    /// <returns></returns>
    bool Interact();
}
