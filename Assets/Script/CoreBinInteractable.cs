using UnityEngine;
using System.Collections;
using System;
using RGCommon;
using CakewalkIoC.Signal;
using DarkTonic;

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

                InteractableChicken animal = loot.GetComponent<Interactable>() as InteractableChicken;
                if(animal != null) {
                    PlayAnimalSound(animal.type);

                }


                Destroy(loot);
                explosion.Emit(10);
                GainExpSignal.Dispatch(expGain);
                OnInteractionComplete(null);  
            });
        });

    }

    private void PlayAnimalSound(AnimalType type) {
        Debug.Log(type);
        switch(type) {
            case AnimalType.None:
                break;
        case AnimalType.Cow:
            DarkTonic.MasterAudio.MasterAudio.PlaySound3DAtTransform ("Cow01", transform);
                
                break;
            case AnimalType.Chicken:
            DarkTonic.MasterAudio.MasterAudio.PlaySound3DAtTransform ("ChickenBirds01", transform);

                break;
            case AnimalType.Cheep:
            DarkTonic.MasterAudio.MasterAudio.PlaySound3DAtTransform ("Sheep01", transform);

                break;
            default:
                break;
        }

    }
}
