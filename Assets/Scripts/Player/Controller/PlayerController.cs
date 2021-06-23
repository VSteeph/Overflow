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
    [SerializeField]
    private GameObject _mainCamera;

    // player
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _horizontalVelocity;
    private float _speed;
    public bool IsGrounded;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    //state

    //delegate
    public System.Action onMove;
    public System.Action onLand;
    public System.Action OnJump;
    public System.Action OnFall;


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

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        if (input.direction == Vector2.zero)
        {
            _speed = CalculateSpeed(false);
        }
        else
        {
            _speed = CalculateSpeed(true);
        }
          

        // normalise input direction
        Vector3 inputDirection = new Vector3(input.direction.x, 0.0f, input.direction.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        if (input.direction != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, config.RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        var relativeMovement = targetDirection.normalized * (_speed) + new Vector3(0f, _verticalVelocity, 0f);

        // à passer en World Space
        rb.velocity = relativeMovement;

        // update animator if using character
        //_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        /*if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }*/
    }

    private void JumpAndGravity()
    {
        if (IsGrounded)
        {

            // Reset Gravity
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
                _fallTimeoutDelta = config.FallTimeout;
                onLand?.Invoke();
            }

            // Jump
            if (input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(config.JumpHeight * -2f * config.Gravity);
                OnJump?.Invoke();

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
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                OnFall?.Invoke();
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
    }

    private float CalculateSpeed(bool isRunning)
    {
        var speed = config.MoveSpeed;
        // a reference to the players current horizontal velocity => RECUPERER EN LOCAL SPACE pour pas avoir la gravité
        float currentHorizontalSpeed = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z).magnitude;
        float speedOffset = 0.1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < config.MaxSpeed - speedOffset || currentHorizontalSpeed > config.MaxSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, config.MaxSpeed, Time.deltaTime * config.SpeedChangeRate);

            if (_speed < config.MoveSpeed)
            {
                _speed = config.MoveSpeed;
            }
            else
            {
                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
        }

        return speed;
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
