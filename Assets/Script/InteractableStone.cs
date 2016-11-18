using UnityEngine;
using System.Collections;
using System;

public class InteractableStone : Interactable {

    public float regrowTime = 15f;
    public GameObject stone;
    public GameObject stonePick;
    
    public override void Interact(PlayerAvatar player, Action<GameObject> OnInteractionComplete) {
        isInteractable = false;
        GameObject pick = (GameObject)Instantiate(stonePick, player.ObjectHolder.position, player.ObjectHolder.rotation);

        LeanTween.rotateLocal(pick, new Vector3(60, 0, 0), 0.1f).setLoopPingPong(5).setOnComplete(() => {
            Destroy(pick);
            OnInteractionComplete(stone);
            ReGrow();
        });
    }

    private void ReGrow() {
        transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, Vector3.one, regrowTime).setOnComplete(() => {
            isInteractable = true;
        });
    }

}
