using UnityEngine;
using System.Collections;
using RGCommon;
using System;
using DarkTonic.MasterAudio;

[RequireComponent(typeof(Animator))]
public class AvatarAnimationController : MonoBehaviour {
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float playbackSpeedWalk;
    [HideInInspector] public float playbackSpeedRun;
    [HideInInspector] public float playbackSpeedFastRun;
    [HideInInspector] public float animationSpeedPercentage;

    Animator animator;

    GameObject backpack;
    GameObject backpackThrow;
    GameObject bigBackpack;

    void Awake() {
        animator = Find.ComponentOnGameObject<Animator>(this);
        backpack = Find.ChildByName(this, "Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/WorldSmallBackpack");
        backpackThrow = Find.ChildByName(this, "SmallBackpack_Throw");
        bigBackpack = Find.ChildByName(this, "WorldCollectionBackpack");
    }

    public void StartTouch() {
        animator.SetBool("InputTouching", true);
    }

    public void EndTouch() {
        animator.SetBool("InputTouching", false);
    }

    public void SetVelocity(float velocity, float maxVelocity) {

        animationSpeedPercentage = Mathf.Clamp01(velocity / 4.0f) * 100.0f;

        playbackSpeedWalk = velocity / 0.5f;
        playbackSpeedRun = velocity / 2.0f;
        playbackSpeedFastRun = velocity / 4.0f;

        animator.SetFloat("WalkSpeedMultiplier", playbackSpeedWalk);
        animator.SetFloat("RunSpeedMultiplier", playbackSpeedRun);
        animator.SetFloat("FastRunSpeedMultiplier", playbackSpeedFastRun);
        animator.SetFloat("Velocity", animationSpeedPercentage);
    }

	public void Jump() {
		//animator.SetTrigger("Jump");

	}

    internal void EnterBackpack() {
        animator.SetTrigger("EnterBackpack");
        MasterAudio.PlaySound3DAtTransformAndForget ("BackPackScenario1", transform);
    }

    public void ThrowBackpack() {
        backpack.SetActive(false);
        backpackThrow.SetActive(true);
    }

    public void SpawnBigBackpack() {
        backpackThrow.SetActive(false);
        bigBackpack.SetActive(true);

    }
}
