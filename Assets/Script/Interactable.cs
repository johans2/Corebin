using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour {

    public bool isInteractable = true;

    public virtual void Interact(PlayerAvatar player, Action<GameObject> OnInteractionComplete) {}
}
