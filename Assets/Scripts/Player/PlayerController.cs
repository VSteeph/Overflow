using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Parametres Gameplay")]
    public PlayerConfig config;

    [Header("Parametres Systeme")]
    public Rigidbody rb;
    public PlayerInputControl input;
    public CapsuleCollider col;
    public Animator animator;

    // player
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    public bool IsGrounded;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    //state
    private bool hasAnimator;

    private void Start()
    {
        hasAnimator = TryGetComponent(out animator);
    }

    private void FixedUpdate()
    {
        JumpAndGravity();
        GroundCheck();
        Move();
    }

    private void LateUpdate()
    {
        
    }

    #region Movement
    private void Move()
    {
        rb.velocity = new Vector3(0, _verticalVelocity, 0);
    }

    private void JumpAndGravity()
    {
        if(IsGrounded)
        {
            _fallTimeoutDelta = config.FallTimeout;
            //Animator State
            if (hasAnimator)
            {
                animator.SetBool(AnimationConstants.JumpAnimatorParam, false);
                animator.SetBool(AnimationConstants.FreeFalldAnimatorParam, false);
            }
            // Reset Gravity
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(config.JumpHeight * -2f * config.Gravity);

                // update animator if using character
                if (hasAnimator)
                {
                    animator.SetBool(AnimationConstants.JumpAnimatorParam, true);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
        }
        else
        {
            input.jump = false;
            //Si la chute suffisament longtemps, on passe en animation chute
            if(hasAnimator)
            {
                if (_fallTimeoutDelta >= 0.0f)
                    _fallTimeoutDelta -= Time.deltaTime;
                else
                    animator.SetBool(AnimationConstants.FreeFalldAnimatorParam, true);
            }
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < config.terminalVelocity)
        {
            _verticalVelocity += config.Gravity * Time.fixedDeltaTime * config.GravityMultiplier;
        }
    }

    private void GroundCheck()
    {
        // set sphere position, with offset
        Vector3 groundSphere = new Vector3(transform.position.x, transform.position.y - PhysicConfig.GroundedOffset, transform.position.z);
        IsGrounded = Physics.CheckSphere(groundSphere, col.radius, PhysicConfig.GroundLayers, QueryTriggerInteraction.Ignore);

        // Predictable if
        if (hasAnimator)
        {
            animator.SetBool(AnimationConstants.GroundedAnimatorParam, IsGrounded);
        }
    }
    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
            
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        Gizmos.color = IsGrounded ? transparentGreen : transparentRed;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - PhysicConfig.GroundedOffset, transform.position.z), col.radius);
    }
    #endregion
}
