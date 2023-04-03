using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))] // using rigidbody for movement
public class PlayerController : MonoBehaviour
{
    [Header("Mouse Controls")]
    public Vector2 mouseSensitivity;
    private Vector2 rotation;

    [Header("Object References")]
    public GameLogic gameLogic;
    private Camera _mainCamera;
    public Camera firstPersonCamera;
    private GameObject _player;
    private Rigidbody _rb;
    public Slider healthSlider;

    [Header("Player Variables")]
    public int currentHp;
    public uint maxHp = 100; // maxHp should be a positive integer, uint
    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpPower = 6.0f;
    public float playerHeight = 2;
    private bool jumpReady;
    private bool doubleJumpReady;
    private bool enablePlayerMovementControls, enablePlayerCameraControls; // lets us disable camera/movement controls which can be useful for certain animations or cutscenes
    public bool canRegenHp = true;
    private float defaultFov;

    [Header("Player Skills")]
    public bool unlockDoubleJump;

    [Header("Player Inventory")]
    public List<ItemBase> playerItems = new List<ItemBase>();
    public List<Weapon> playerWeapons = new List<Weapon>();
    public Weapon currentWeapon;

    //[HideInInspector]
    public ItemBase itemToPickup; // needs to be public but doesn't need to show in inspector
    public Weapon weaponToPickup; // same as above. check the section for adding items to inventory and if this isn't used, delete this line.

    [Header("Misc")]
    public LayerMask groundLayer;
    private bool isGrounded;
    private bool canZoom;
    public float timeToZoom = 0.1f; // how long does it take to zoom in/out
    public float zoomedFov = 40f; // fov when zoomed all the way in
    private Coroutine zoomRoutine;

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
            mouseSensitivity.x = 100;
        if (mouseSensitivity.y == 0)
            mouseSensitivity.y = 100;

        // set gameLogic references
        enablePlayerMovementControls = gameLogic.enablePlayerMovementControls;
        enablePlayerCameraControls = gameLogic.enablePlayerCameraControls;

        jumpReady = true;
        doubleJumpReady = true;

        if (maxHp <= 0)
            maxHp = 100;
        if (currentHp <= 0)
            currentHp = (int)maxHp;

        if (playerWeapons.Count <= 0)
        {
            Debug.LogError("No weapons in weapon list. There should always be at least one.");
        }

        if (currentWeapon == null) // need to always have a weapon in the weapon list.
            currentWeapon = playerWeapons[0];

        StartCoroutine(PassiveRegen());

        defaultFov = _mainCamera.fieldOfView;

        canZoom = true;
    }

    private void Update()
    {
        Time.timeScale = gameLogic.timeScale;
        if (currentHp <= 0)
        {
            StartCoroutine(PlayerDeath());
            canRegenHp = false;
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
            HandleCamera();
            HandleActions();
            firstPersonCamera.fieldOfView = _mainCamera.fieldOfView;
        }
        if (gameLogic.timeScale == 0)
        {
            gameLogic.enablePlayerCameraControls = false;
        }

        healthSlider.value = currentHp;
        healthSlider.maxValue = maxHp;
    }

    private void FixedUpdate()
    {
        if (currentHp <= 0)
        {

        }
        else
        {
            HandleMovement();
        }
    }

    private void LateUpdate()
    {
        // ensure the two booleans match
        enablePlayerMovementControls = gameLogic.enablePlayerMovementControls;
        enablePlayerCameraControls = gameLogic.enablePlayerCameraControls;
    }

    // movement controls
    private void HandleMovement()
    {
        float rotateAmount = Input.GetAxis("Horizontal");
        float moveAmount = Input.GetAxis("Vertical");

        if (transform != null && enablePlayerMovementControls)
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
                        _rb.position += transform.forward * (sprintSpeed) * 0.65f * Time.deltaTime;
                        _rb.position += -transform.right * (sprintSpeed) * 0.65f * Time.deltaTime;
                    }
                    // sprinting forward and right
                    else if (Input.GetKey(KeyCode.D))
                    {
                        _rb.position += transform.forward * (sprintSpeed) * 0.65f * Time.deltaTime;
                        _rb.position += transform.right * (sprintSpeed) * 0.65f * Time.deltaTime;
                    }
                    // sprinting forward
                    else
                    {
                        _rb.position += transform.forward * (sprintSpeed) * Time.deltaTime;
                    }
                }
                // walk handler
                else if (Input.GetKey(KeyCode.LeftAlt))
                {
                    // walking forward and left
                    if (Input.GetKey(KeyCode.A))
                    {
                        _rb.position += transform.forward * (walkSpeed) * 0.65f * Time.deltaTime;
                        _rb.position += -transform.right * (walkSpeed) * 0.65f * Time.deltaTime;
                    }
                    // walking forward and right
                    else if (Input.GetKey(KeyCode.D))
                    {
                        _rb.position += transform.forward * (walkSpeed) * 0.65f * Time.deltaTime;
                        _rb.position += transform.right * (walkSpeed) * 0.65f * Time.deltaTime;
                    }
                    // walking forward
                    else
                    {
                        _rb.position += transform.forward * (walkSpeed) * Time.deltaTime;
                    }
                }
                // run handler
                else
                {
                    // running forward and left
                    if (Input.GetKey(KeyCode.A))
                    {
                        _rb.position += transform.forward * (runSpeed) * 0.65f * Time.deltaTime;
                        _rb.position += -transform.right * (runSpeed) * 0.65f * Time.deltaTime;
                    }
                    // running forward and right
                    else if (Input.GetKey(KeyCode.D))
                    {
                        _rb.position += transform.forward * (runSpeed) * 0.65f * Time.deltaTime;
                        _rb.position += transform.right * (runSpeed) * 0.65f * Time.deltaTime;
                    }
                    // running forward
                    else
                    {
                        _rb.position += transform.forward * (runSpeed) * Time.deltaTime;
                    }
                }
            }
            // backwards movement
            else if (Input.GetKey(KeyCode.S))
            {
                // running backwards and left
                if (Input.GetKey(KeyCode.A))
                {
                    _rb.position += -transform.forward * (runSpeed) * 0.65f * Time.deltaTime;
                    _rb.position += -transform.right * (runSpeed) * 0.65f * Time.deltaTime;
                }
                // running backwards and right
                else if (Input.GetKey(KeyCode.D))
                {
                    _rb.position += -transform.forward * (runSpeed) * 0.65f * Time.deltaTime;
                    _rb.position += transform.right * (runSpeed) * 0.65f * Time.deltaTime;
                }
                // running backwards
                else
                {
                    _rb.position += -transform.forward * (runSpeed) * Time.deltaTime;
                }
            }
            // running left-only
            else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                _rb.position += -transform.right * (runSpeed) * Time.deltaTime;
            }
            // running right-only
            else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                _rb.position += transform.right * (runSpeed) * Time.deltaTime;
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

        if (enablePlayerCameraControls)
        {
            rotation.y += mouseX;

            rotation.x -= mouseY;
            rotation.x = Mathf.Clamp(rotation.x, -90f, 90f);

            _mainCamera.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
            transform.rotation = Quaternion.Euler(0, rotation.y, 0);
        }
    }

    private void HandleActions()
    {
        // Interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            // pickup item
            if (itemToPickup != null && itemToPickup.canBePickedUp && itemToPickup.readyToBePickedUp && !itemToPickup.type.ToUpper().Equals("WEAPON"))
            {
                playerItems.Add(itemToPickup);
                itemToPickup.gameObject.SetActive(false);
                if (itemToPickup.pickupText != null)
                    itemToPickup.pickupText.gameObject.SetActive(false);
                itemToPickup = null;
            }
            else if (itemToPickup != null && itemToPickup.canBePickedUp && itemToPickup.readyToBePickedUp && itemToPickup.type.ToUpper().Equals("WEAPON"))
            {
                playerWeapons.Add((Weapon)itemToPickup);
                itemToPickup.gameObject.SetActive(false);
                if (itemToPickup.pickupText != null)
                    itemToPickup.pickupText.gameObject.SetActive(false);
                itemToPickup = null;
            }
        }

        // Slow down time (editor only)
        if (UnityEditor.EditorApplication.isPlaying) // this will prevent the project from building, remove before build
        {
            if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus))
            {
                gameLogic.timeScale += 0.05f;
            }
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.Underscore))
            {
                if (gameLogic.timeScale >= 0.05f)
                    gameLogic.timeScale -= 0.05f;
            }
        }

        // Zoom
        if (canZoom)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (zoomRoutine != null)
                {
                    StopCoroutine(zoomRoutine);
                    zoomRoutine = null;
                }

                zoomRoutine = StartCoroutine(HandleZoom(true));
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (zoomRoutine != null)
                {
                    StopCoroutine(zoomRoutine);
                    zoomRoutine = null;
                }

                zoomRoutine = StartCoroutine(HandleZoom(false));
            }
        }
    }

    /* HandleInventory should control what weapons
     * are displayed for the player.
     * Need to cycle through the weapons list whenever the player
     * switches weapons.
     * HandleInventory should also control things like keys in the inventory.*/
    private void HandleInventory()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            // cycle weapon up
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            // cycle weapon down
        }
    }

    private IEnumerator Jump()
    {
        if (jumpReady && isGrounded && enablePlayerMovementControls)
        {
            _rb.velocity = (Vector2.down * 0);
            _rb.AddForce(Vector2.up * (jumpPower), ForceMode.Impulse);

            jumpReady = false;
        }
        yield return null;
    }

    private IEnumerator DoubleJump()
    {
        if (unlockDoubleJump && doubleJumpReady && !isGrounded && enablePlayerMovementControls)
        {
            _rb.velocity = (Vector2.down * 0);
            _rb.AddForce(Vector2.up * (jumpPower), ForceMode.Impulse);

            doubleJumpReady = false;
        }
        yield return null;
    }

    private IEnumerator PassiveRegen()
    {
        while (true)
        {
            if (currentHp < (int)maxHp && canRegenHp)
                currentHp += 1;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator HandleZoom(bool enterZoom)
    {
        float desiredFov = enterZoom ? zoomedFov : defaultFov; // enterZoom is checked as an if statement. if true, desiredFov = zoomedFov. if false, desiredFov = defaultFov
        float startingFov = _mainCamera.fieldOfView;
        float timeElapsed = 0;

        Debug.Log("Enter ToggleZoom");

        while (timeElapsed < timeToZoom)
        {
            Debug.Log("Enter ToggleZoom loop");
            _mainCamera.fieldOfView = Mathf.Lerp(startingFov, desiredFov, (timeElapsed / timeToZoom));
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _mainCamera.fieldOfView = desiredFov;
        zoomRoutine = null;
    }

    private IEnumerator PlayerDeath()
    {
        Time.timeScale = 0; // pauses the game, can instead pause the editor but that won't work for an actual BUILD of a unity game
        gameLogic.enablePlayerCameraControls = false;
        gameLogic.enablePlayerMovementControls = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        yield return null;
    }
}
