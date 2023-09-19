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

    private float gravity;
    private float playerSpeed;
    private Vector3 movementDirection = Vector3.forward;

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

}
