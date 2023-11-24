using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float initialPlayerSpeed = 4f;
    [SerializeField] private float maximumPlayerSpeed = 30f;
    [SerializeField] private float playerSpeedIncreaseRate = 0.1f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float initialGravityValue = -9.81f;
    [SerializeField] private LayerMask groundLayer;

    private float gravity;
    private float playerSpeed;
    private Vector3 movementDirection = Vector3.forward;
    private Vector3 playerVelocity;

    private PlayerInput playerInput;
    private InputAction slideAction;
    private InputAction turnAction;
    private InputAction jumpAction;

    private CharacterController controller;

    public void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();

        turnAction = playerInput.actions["Turn"];
        jumpAction = playerInput.actions["Jump"];
        slideAction = playerInput.actions["Slide"];


    }

    public void Start()
    {
        gravity = initialGravityValue;
        playerSpeed = initialPlayerSpeed;

    }

    private void OnEnable()
    {
        turnAction.performed += PlayerTurn;
        jumpAction.performed += PlayerJump;
        slideAction.performed += PlayerSlide;
    }

    private void OnDisable()
    {
        turnAction.performed -= PlayerTurn;
        jumpAction.performed -= PlayerJump;
        slideAction.performed -= PlayerSlide;
    }

    private void PlayerTurn(InputAction.CallbackContext context) { }
    private void PlayerJump(InputAction.CallbackContext context) { }

    private void PlayerSlide(InputAction.CallbackContext context) { }

    private void Update()
    {
        controller.Move(transform.forward * playerSpeed * Time.deltaTime);
        if (IsGrounded() && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

    }

    private bool IsGrounded(float length = .2f)
    {
        Vector3 raycastOriginFirst = transform.position;
        raycastOriginFirst.y -= controller.height / 2f;
        raycastOriginFirst.y += 0.1f;

        Vector3 raycastOriginSecond = raycastOriginFirst;
        raycastOriginFirst -= transform.forward * 0.2f;
        raycastOriginSecond += transform.forward * .2f;

        if ((Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, groundLayer)) ||
                (Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, groundLayer))
                )
        {

            return true;
        }
        return false;
    }

}
