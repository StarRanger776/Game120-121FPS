using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // using rigidbody for movement
public class PlayerController : MonoBehaviour
{
    [Header("Mouse Controls")]
    public Vector2 mouseSensitivity;
    private Vector2 rotation;

    [Header("Object References")]
    public GameLogic gameLogic;
    private Camera _mainCamera;
    private GameObject _player;
    private Rigidbody _rb;

    [Header("Player Variables")]
    public int currentHp;
    public uint maxHp = 100; // maxHp should be a positive integer, uint
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpPower = 6.0f;
    public float playerHeight = 2;
    [Header("")]
    public bool jumpReady;
    public bool doubleJumpReady;
    private bool enablePlayerMovementControls, enablePlayerCameraControls; // lets us disable camera/movement controls which can be useful for certain animations or cutscenes

    [Header("Player Skills")]
    public bool unlockDoubleJump;

    [Header("Player Inventory")]
    public List<Item> inventory = new List<Item>();

    [Header("Misc")]
    public LayerMask groundLayer;
    private bool isGrounded;

    private void Start()
    {
        // object references initialized before anything else
        _mainCamera = Camera.main;
        _player = this.gameObject;
        _rb = GetComponent<Rigidbody>();

        // lock cursor to game window center and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // default settings for mouse sensitivity
        if (mouseSensitivity.x == 0)
            mouseSensitivity.x = 250;
        if (mouseSensitivity.y == 0)
            mouseSensitivity.y = 250;

        // set gameLogic references
        enablePlayerMovementControls = gameLogic.enablePlayerMovementControls;
        enablePlayerCameraControls = gameLogic.enablePlayerCameraControls;

        jumpReady = true;
        doubleJumpReady = true;

        if (maxHp <= 0)
            maxHp = 100;
        if (currentHp <= 0)
            currentHp = (int)maxHp;
    }

    private void Update()
    {
        if (currentHp <= 0)
        {
            StartCoroutine(PlayerDeath());
        }
        else
        {
            if (Input.GetKey(KeyCode.Space) && isGrounded)
            {
                StartCoroutine(Jump());
            }
            if (Input.GetKeyDown(KeyCode.Space) && !isGrounded)
            {
                StartCoroutine(DoubleJump());
            }
        }
    }

    private void FixedUpdate()
    {
        HandleCamera();
        HandleMovement();
    }

    private void LateUpdate()
    {
        // ensures the two variables match
        enablePlayerMovementControls = gameLogic.enablePlayerMovementControls;
        enablePlayerCameraControls = gameLogic.enablePlayerCameraControls;
    }

    // movement controls
    private void HandleMovement()
    {
        float rotateAmount = Input.GetAxis("Horizontal");
        float moveAmount = Input.GetAxis("Vertical");

        if (transform != null)
        {
            // forward movement
            if (Input.GetKey(KeyCode.W))
            {
                // sprinting
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    // sprinting forward and left
                    if (Input.GetKey(KeyCode.A))
                    {
                        _rb.position += transform.forward * (sprintSpeed) * 0.65f * Time.fixedDeltaTime;
                        _rb.position += -transform.right * (sprintSpeed) * 0.65f * Time.fixedDeltaTime;
                    }
                    // sprinting forward and right
                    else if (Input.GetKey(KeyCode.D))
                    {
                        _rb.position += transform.forward * (sprintSpeed) * 0.65f * Time.fixedDeltaTime;
                        _rb.position += transform.right * (sprintSpeed) * 0.65f * Time.fixedDeltaTime;
                    }
                    // sprinting forward
                    else
                    {
                        _rb.position += transform.forward * (sprintSpeed) * Time.fixedDeltaTime;
                    }
                }
                // walk handler
                else
                {
                    // walking forward and left
                    if (Input.GetKey(KeyCode.A))
                    {
                        _rb.position += transform.forward * (walkSpeed) * 0.65f * Time.fixedDeltaTime;
                        _rb.position += -transform.right * (walkSpeed) * 0.65f * Time.fixedDeltaTime;
                    }
                    // walking forward and right
                    else if (Input.GetKey(KeyCode.D))
                    {
                        _rb.position += transform.forward * (walkSpeed) * 0.65f * Time.fixedDeltaTime;
                        _rb.position += transform.right * (walkSpeed) * 0.65f * Time.fixedDeltaTime;
                    }
                    // walking forward
                    else
                    {
                        _rb.position += transform.forward * (walkSpeed) * Time.fixedDeltaTime;
                    }
                }
            }
            // backwards movement
            else if (Input.GetKey(KeyCode.S))
            {
                // walking backwards and left
                if (Input.GetKey(KeyCode.A))
                {
                    _rb.position += -transform.forward * (walkSpeed) * 0.65f * Time.fixedDeltaTime;
                    _rb.position += -transform.right * (walkSpeed) * 0.65f * Time.fixedDeltaTime;
                }
                // walking backwards and right
                else if (Input.GetKey(KeyCode.D))
                {
                    _rb.position += -transform.forward * (walkSpeed) * 0.65f * Time.fixedDeltaTime;
                    _rb.position += transform.right * (walkSpeed) * 0.65f * Time.fixedDeltaTime;
                }
                // walking backwards
                else
                {
                    _rb.position += -transform.forward * (walkSpeed) * Time.fixedDeltaTime;
                }
            }
            // left-only movement
            else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                _rb.position += -transform.right * (walkSpeed) * Time.fixedDeltaTime;
            }
            // right-only movement
            else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                _rb.position += transform.right * (walkSpeed) * Time.fixedDeltaTime;
            }
        }

        // reset jump checks, can add layers to check if needed
        isGrounded = Physics.Raycast(_player.transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        if (isGrounded)
        {
            jumpReady = true;
            doubleJumpReady = true;
        }
    }

    // camera controls
    private void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.fixedDeltaTime * mouseSensitivity.x;
        float mouseY = Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * mouseSensitivity.y;

        rotation.y += mouseX;

        rotation.x -= mouseY;
        rotation.x = Mathf.Clamp(rotation.x, -90f, 90f);

        _mainCamera.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
        transform.rotation = Quaternion.Euler(0, rotation.y, 0);
    }

    private void HandleActions()
    {
        
    }

    private IEnumerator Jump()
    {
        if (jumpReady && isGrounded)
        {
            _rb.velocity = (Vector2.down * 0);
            _rb.AddForce(Vector2.up * (jumpPower), ForceMode.Impulse);

            jumpReady = false;
            yield return null;
        }
    }

    private IEnumerator DoubleJump()
    {
        if (unlockDoubleJump && doubleJumpReady && !isGrounded)
        {
            _rb.velocity = (Vector2.down * 0);
            _rb.AddForce(Vector2.up * (jumpPower), ForceMode.Impulse);

            doubleJumpReady = false;
            yield return null;
        }
    }

    private IEnumerator PlayerDeath()
    {
        Time.timeScale = 0; // pauses the game, can instead pause the editor but that won't work for an actual BUILD of a unity game
        yield return null;
    }
}
