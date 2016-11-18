using UnityEngine;
using System.Collections;
using System;
using RGCommon;
using CakewalkIoC.Signal;

public class CoreBinInteractable : Interactable {

    public int expGain = 34;
    public static Signal<int> GainExpSignal = new Signal<int>();

    private Transform throwTarget1;
    private Transform throwTarget2;
    private ParticleSystem explosion;

    void Awake() {
        throwTarget1 = Find.ComponentOnChild<Transform>(this, "ThrowTarget1");
        throwTarget2 = Find.ComponentOnChild<Transform>(this, "ThrowTarget2");
        explosion = Find.ComponentOnChild<ParticleSystem>(this, "ParticleSplash");
    }
    
    public override void Interact(PlayerAvatar player, Action<GameObject> OnInteractionComplete) {
        GameObject loot = player.CurrentLoot;

        LeanTween.rotate(loot, throwTarget1.rotation.eulerAngles, 0.2f);
        LeanTween.move(loot, throwTarget1.position, 0.2f).setOnComplete(() => {
            LeanTween.rotate(loot, throwTarget2.rotation.eulerAngles, 0.2f);
            LeanTween.move(loot, throwTarget2.position, 0.2f).setOnComplete(() => {
                // Gain exp!
                Destroy(loot);
                explosion.Emit(10);
                GainExpSignal.Dispatch(expGain);
                OnInteractionComplete(null);  
            });
        });

    }

}
