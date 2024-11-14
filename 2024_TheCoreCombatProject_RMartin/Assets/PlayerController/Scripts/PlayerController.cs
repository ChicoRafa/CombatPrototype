using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float linearAcceleration = 50f;
    [SerializeField] private float maxRunSpeed = 5f;
    [SerializeField] private float maxWalkSpeed = 2f;
    [SerializeField] private float decelerationFactor = 10f;

    [Header("Jump")]
    [SerializeField] float jumpSpeed = 5f;

    public enum OrientationMode
    {
        MovementDirection,
        CameraDirection,
        FaceToTarget
    }
    [Header("Orientation")]
    [SerializeField] OrientationMode orientationMode = OrientationMode.MovementDirection;
    [SerializeField] float angularVelocity = 360f;
    [SerializeField] Transform target;

    [Header("Combat")] 
    [SerializeField] private Transform hitCollidersParent;
    
    [Header("Inputs Movement")]
    [SerializeField] InputActionReference moveInputActionReference;
    [FormerlySerializedAs("jump")] [SerializeField] InputActionReference jumpInputActionReference;
    [SerializeField] InputActionReference walkInputActionReference;


    Animator animator;
    CharacterController characterController;
    Camera mainCamera;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        moveInputActionReference.action.Enable();

        moveInputActionReference.action.started += OnMove;
        moveInputActionReference.action.performed += OnMove;
        moveInputActionReference.action.canceled += OnMove;

        jumpInputActionReference.action.Enable();
        jumpInputActionReference.action.performed += OnJump;
        
        walkInputActionReference.action.Enable();
        walkInputActionReference.action.started += OnWalk;
        walkInputActionReference.action.canceled += OnWalk;
        
        foreach (AnimationEventForwarder animationEventForwarder in GetComponentsInChildren<AnimationEventForwarder>())
        {
            animationEventForwarder.onAnimationAttackEvent.AddListener(OnAnimationEvent);
        }
    }
    
    private void OnDisable()
    {
        moveInputActionReference.action.Disable();

        moveInputActionReference.action.started -= OnMove;
        moveInputActionReference.action.performed -= OnMove;
        moveInputActionReference.action.canceled -= OnMove;

        jumpInputActionReference.action.Disable();
        jumpInputActionReference.action.performed -= OnJump;
        
        walkInputActionReference.action.Disable();
        walkInputActionReference.action.started -= OnWalk;
        walkInputActionReference.action.canceled -= OnWalk;
        
        foreach (AnimationEventForwarder animationEventForwarder in GetComponentsInChildren<AnimationEventForwarder>())
        {
            animationEventForwarder.onAnimationAttackEvent.RemoveListener(OnAnimationEvent);
        }
    }
    #region Input Events
    //Inputs
    Vector3 rawStickValue;
    private void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 stickValue = ctx.ReadValue<Vector2>();

        rawStickValue = new Vector3(stickValue.x, 0, stickValue.y);
    }

    bool mustJump = false;
    private void OnJump(InputAction.CallbackContext ctx)
    {
        mustJump = true;
    }
    
    bool isWalking = false;
    private void OnWalk(InputAction.CallbackContext ctx)
    {
        isWalking = ctx.ReadValueAsButton();
    }
    #endregion

    private void Update()
    {
        Vector3 compositeMovement = Vector3.zero;
        compositeMovement += UpdateMovementOnPlane();
        compositeMovement += UpdateVerticalMovement();

        characterController.Move(compositeMovement);

        UpdateOrientation();
        UpdateAnimation();
    }

    #region Movement
    Vector3 velocityOnPlane = Vector3.zero;
    private Vector3 UpdateMovementOnPlane()
    {
        // Deceleration
        Vector3 decelerationOnPlane = -velocityOnPlane * (decelerationFactor * Time.deltaTime);
        velocityOnPlane += decelerationOnPlane;
        // Acceleration
        Vector3 acceleration = (mainCamera.transform.forward * rawStickValue.z) + (mainCamera.transform.right * rawStickValue.x);
        float accelerationLength = acceleration.magnitude;
        Vector3 projectedAcceleration = Vector3.ProjectOnPlane(acceleration, Vector3.up).normalized * accelerationLength;
        Vector3 deltaAccelerationOnPlane = projectedAcceleration * (linearAcceleration * Time.deltaTime);

        // Account for max speed
        float maxSpeed = CalculateMaxSpeed();
        float currentSpeed = velocityOnPlane.magnitude;
        float attainableVelocity = Mathf.Max(currentSpeed, maxSpeed);
        velocityOnPlane += deltaAccelerationOnPlane;
        velocityOnPlane = Vector3.ClampMagnitude(velocityOnPlane, attainableVelocity);
        
        return velocityOnPlane * Time.deltaTime;
    }

    const float gravity = -9.8f;
    float verticalVelocity = 0f;
    private Vector3 UpdateVerticalMovement()
    {
        if (characterController.isGrounded)
        { verticalVelocity = 0f; }

        if (mustJump)
        { 
            mustJump = false;
            if (characterController.isGrounded)
            {   verticalVelocity = jumpSpeed;   }
        }

        verticalVelocity += gravity * Time.deltaTime;
        return Vector3.up * (verticalVelocity * Time.deltaTime);
    }
    #endregion

    #region "Orientation"
    Vector3 lastMovementDirection = Vector3.zero;
    private void UpdateOrientation()
    {
        Vector3 desiredDirection = CalculateDesiredDirection();
        RotateToDesiredDirection(desiredDirection);

        Vector3 CalculateDesiredDirection()
        {
            Vector3 desiredDirection = Vector3.zero;

            switch (orientationMode)
            {
                case OrientationMode.MovementDirection:
                    if (rawStickValue.magnitude < 0.01f)
                    {
                        desiredDirection = lastMovementDirection;
                    }
                    else
                    {
                        desiredDirection = velocityOnPlane;
                        lastMovementDirection = desiredDirection;
                    }
                    break;
                case OrientationMode.CameraDirection:
                    desiredDirection = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
                    break;
                case OrientationMode.FaceToTarget:
                    desiredDirection = Vector3.ProjectOnPlane(target.position - transform.position, Vector3.up);
                    break;
            }

            return desiredDirection;
        }

        void RotateToDesiredDirection(Vector3 desiredDirection)
        {
            float angularDistance = Vector3.SignedAngle(transform.forward, desiredDirection, Vector3.up);
            float angleToApply = angularVelocity * Time.deltaTime;
            angleToApply = Mathf.Min(angleToApply, Mathf.Abs(angularDistance));

            Quaternion rotationToApply =
                Quaternion.AngleAxis(
                    angleToApply * Mathf.Sign(angularDistance),
                    Vector3.up);
            transform.rotation = rotationToApply * transform.rotation;
        }
    }
    #endregion
    private float CalculateMaxSpeed()
    {
        return isWalking ? maxWalkSpeed : maxRunSpeed;
    }
    
    
    void UpdateAnimation()
    {
        float maxSpeed = maxRunSpeed;
        Vector3 localVelocity = transform.InverseTransformDirection(velocityOnPlane);
        animator.SetFloat("HorizontalVelocity", localVelocity.x / maxSpeed);
        animator.SetFloat("ForwardVelocity", localVelocity.z / maxSpeed);

        float jumpProgress = Mathf.InverseLerp(jumpSpeed, -jumpSpeed, verticalVelocity);
        
        animator.SetFloat("JumpProgress", characterController.isGrounded ? 1f : jumpProgress);
        animator.SetBool("IsGrounded", characterController.isGrounded);
    }
    private void OnAnimationEvent(string hitColliderName)
         {
             hitCollidersParent.Find(hitColliderName)?.gameObject.SetActive(true);
         }
}