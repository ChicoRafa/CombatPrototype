using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")] [SerializeField] private float speed = 5f;
    private Vector3 movement;
    private Vector3 rawMovement;
    
    [Header("Orientation")]
    [SerializeField] float angularVelocity = 360f;
    [SerializeField] private OrientationMode orientationMode = OrientationMode.CameraDirection;
    [SerializeField] Transform target;
    public enum OrientationMode
    {
        MovementDirection,
        CameraDirection,
        FaceToTarget
    }
    Vector3 projectedMovement = Vector3.zero;
    Vector3 lastMovementDirection = Vector3.zero;
    
    [Header("Input Actions")] [SerializeField]
    InputActionReference movementInputAction;

    [SerializeField] InputActionReference jumpInputAction;
    [SerializeField] InputActionReference attackInputAction;
    [SerializeField] InputActionReference nextPrevWeaponInputAction;

    public Animator animator;
    CharacterController characterController;
    Camera mainCamera;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        UpdateMovement();
        UpdateOrientation();
        UpdateAnimation();
    }

    private void UpdateMovement()
    {
        movement = (mainCamera.transform.forward * rawMovement.z) + (mainCamera.transform.right * rawMovement.x);
        float movementLength = movement.magnitude;
        //we don't want to move the character up or down
        projectedMovement = Vector3.ProjectOnPlane(movement, Vector3.up).normalized * movementLength;
        movement = projectedMovement;
        //here we move the character
        characterController.Move(movement * (speed * Time.deltaTime));
    }

    private void UpdateOrientation()
    {
        //here we calculate the desired direction
        Vector3  desiredDirection = CalculateDesiredDirection();
        //here we set the desired direction
        RotateToDesiredDirection(desiredDirection);
    }

    private void UpdateAnimation()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(projectedMovement);
        animator.SetFloat("HorizontalVelocity", localVelocity.x);
        animator.SetFloat("ForwardVelocity", localVelocity.z);
    }

    private Vector3 CalculateDesiredDirection()
    {
        Vector3 desiredDirection = Vector3.zero;
        switch (orientationMode)
        {
            case OrientationMode.MovementDirection:
                if (rawMovement.magnitude < 0.01f)
                {
                    desiredDirection = lastMovementDirection;
                }
                else
                {
                    desiredDirection = projectedMovement;
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

    private void RotateToDesiredDirection(Vector3 desiredDirection)
    {
        float angularDistance = Vector3.SignedAngle(transform.forward, desiredDirection, Vector3.up);
        float angleToApply = angularVelocity * Time.deltaTime;
        angleToApply = Mathf.Min(angleToApply, Mathf.Abs(angularDistance));
        
        Quaternion rotationToApply = Quaternion.AngleAxis(angleToApply * Mathf.Sign(angularDistance), Vector3.up);

        transform.rotation = rotationToApply * transform.rotation;
    }

    private void OnEnable()
    {
        movementInputAction.action.Enable();

        movementInputAction.action.started += OnMove;
        movementInputAction.action.performed += OnMove;
        movementInputAction.action.canceled += OnMove;
    }

    private void OnDisable()
    {
        movementInputAction.action.Disable();

        movementInputAction.action.started -= OnMove;
        movementInputAction.action.performed -= OnMove;
        movementInputAction.action.canceled -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext callbackContext)
    {
        Vector2 stickValue = callbackContext.ReadValue<Vector2>();
        
        //here we set the movement vector
        rawMovement = (Vector3.forward * stickValue.y) + (Vector3.right * stickValue.x); float movementLength = movement.magnitude;
    }
}