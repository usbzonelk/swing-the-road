using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using PlayerColorManager;
using System.Collections.Generic;

namespace TempleRun.Player
{

    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float initialPlayerSpeed = 4f;
        [SerializeField] private float maximumPlayerSpeed = 30f;
        [SerializeField] private float playerSpeedIncreaseRate = 0.1f;
        [SerializeField] private float jumpHeight = 1.0f;
        [SerializeField] private float initialGravityValue = -9.81f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask turnLayer;
        [SerializeField] private Renderer playRenderer;

        [SerializeField]
        private UnityEvent<Vector3> turnEvent;
        [SerializeField]
        private UnityEvent<int> gameOverEvent;
        [SerializeField]
        private UnityEvent<int> scoreUpdateEvent;
        [SerializeField]
        private float scoreMultiplier = 10f;

        public string playerColor;
        public string[] colors = { "red", "blue", "yellow", "black" };
        public List<Material> playerColorMaterials = new List<Material>();

        private float gravity;
        private float playerSpeed;
        private Vector3 movementDirection = Vector3.forward;
        private Vector3 playerVelocity;

        private PlayerInput playerInput;
        private InputAction turnAction;

        private CharacterController controller;

        private bool GameIsOn = false;
        private float score = 0;

        public void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();

            turnAction = playerInput.actions["Turn"];

            int colorIndex = Random.Range(0, (colors.Length - 1));
            playerColor = colors[colorIndex];
            PlayerColorManager.PlayerColorManager.setPlayerColor(colorIndex, playRenderer, playerColorMaterials);
        }

        public void Start()
        {
            gravity = initialGravityValue;
            playerSpeed = initialPlayerSpeed;

        }

        private void OnEnable()
        {
            turnAction.performed += PlayerTurn;

        }

        private void OnDisable()
        {
            turnAction.performed -= PlayerTurn;
        }

        private void PlayerTurn(InputAction.CallbackContext context)
        {
            Vector3? turnPosition = CheckTurn(context.ReadValue<float>());
            if (!turnPosition.HasValue)
            {
                controller.Move(playerSpeed * Time.deltaTime * context.ReadValue<float>() * transform.right);
                return;
            }
            Vector3 targetDirection = Quaternion.AngleAxis(90 * context.ReadValue<float>(), Vector3.up) * movementDirection;
            turnEvent.Invoke(targetDirection);
            Turn(context.ReadValue<float>(), turnPosition.Value);
        }

        private void Turn(float turnValue, Vector3 turnPosition)
        {
            Vector3 tempPlayerPos = new Vector3(turnPosition.x, transform.position.y, turnPosition.z);
            controller.enabled = false;
            transform.position = tempPlayerPos;
            controller.enabled = true;

            Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90 * turnValue, 0);
            transform.rotation = targetRotation;
            movementDirection = transform.forward.normalized;

        }
        private Vector3? CheckTurn(float turnValue)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, .1f, turnLayer);
            if (hitColliders.Length > 0)
            {
                Tile tile = hitColliders[0].transform.parent.GetComponent<Tile>();
                TileType type = tile.type;
                if ((type == TileType.LEFT && turnValue == -1) || (type == TileType.RIGHT && turnValue == 1) || (type == TileType.SIDEWAYS))
                {
                    return tile.pivot.position;
                }
            }
            return null;

        }

        private void Update()
        {
            score += scoreMultiplier * Time.deltaTime;
            scoreUpdateEvent.Invoke((int)score);

            if (!IsGrounded(2f) && GameIsOn)
            {
                GameOver(); return;
            }

            controller.Move(transform.forward * playerSpeed * Time.deltaTime);
            if (IsGrounded() && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }
            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
            GameIsOn = true;
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

        public void GameOver()
        {
            gameOverEvent.Invoke((int)score);
            gameObject.SetActive(false);
            Debug.Log("gameover");
        }

    }
}
