using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;

public class AvatarEventListener : MonoBehaviour {
    public ParticleSystem footStepDust;
    public ParticlePool particlePool;
    public Transform leftFootTrans;
    public Transform rightFootTrans;

    public void RunRightFootSound() {
        MasterAudio.PlaySound3DAtTransform ("RunRight", transform);
    }

    public void RunLeftFootSound() {
        MasterAudio.PlaySound3DAtTransform ("RunLeft", transform);
    }

    public void RunFastRightFootSound() {
        MasterAudio.PlaySound3DAtTransform ("RunLeft", transform);
        particlePool.showParticleSystemAt(footStepDust.name, rightFootTrans.position);
    }

    public void RunFastLeftFootSound() {
        MasterAudio.PlaySound3DAtTransform ("RunLeft", transform);
        particlePool.showParticleSystemAt(footStepDust.name, leftFootTrans.position);
    }

    public void WalkLeftSound() {
        MasterAudio.PlaySound3DAtTransform ("WalkLeft", transform);

    }

    public void WalkRightSound() {
        MasterAudio.PlaySound3DAtTransform ("WalkRight", transform);

    }

    public void SlideSound() {
        MasterAudio.PlaySound3DAtTransform ("Slide", transform);

    }

    public void AvatarSlideStart() {
        particlePool.showParticleSystemAt(footStepDust.name, leftFootTrans.position);
        particlePool.showParticleSystemAt(footStepDust.name, rightFootTrans.position);
    }



}
