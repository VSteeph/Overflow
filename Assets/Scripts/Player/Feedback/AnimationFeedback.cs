using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFeedback : MonoBehaviour
{
    public PlayerController player;
    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {

        player.onLand += OnLand;
        player.OnJump += OnJump;
        player.OnFall += Onfall;
    }

    public void OnLand()
    {
        animator.SetBool(AnimationConstants.JumpAnimatorParam, false);
        animator.SetBool(AnimationConstants.FreeFalldAnimatorParam, false);
        //Was in GroundCheck Might need to tweak it
        animator.SetBool(AnimationConstants.GroundedAnimatorParam, true);
    }

    public void OnJump()
    {
        animator.SetBool(AnimationConstants.JumpAnimatorParam, true);
    }

    public void Onfall()
    {
        animator.SetBool(AnimationConstants.FreeFalldAnimatorParam, true);
    }
}
