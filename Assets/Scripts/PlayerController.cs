using System.Collections;
using System.Collections.Generic;
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
    public Slider jetpackFuelSlider;

    [Header("Player Variables")]
    public int currentHp;
    public uint maxHp = 100; // maxHp should be a positive integer, uint
    public float walkSpeed = 1.5f;
    public float runSpeed = 3.5f;
    public float sprintSpeed = 5.5f;
    public float jumpPower = 6.0f;
    public float jetpackPower = 1.0f;
    public int jetpackFuel;
    public uint maxJetpackFuel = 100;
    public int fuelRegenAmount = 2;
    public float playerHeight = 2;
    private bool jumpReady;
    private bool doubleJumpReady;
    private bool enablePlayerMovementControls, enablePlayerCameraControls; // lets us disable camera/movement controls which can be useful for certain animations or cutscenes
    public bool canRegenHp = true;
    public bool canRegenFuel = true;
    private float defaultFov;
    public bool canSprint = true;
    public int stamina;
    public int maxStamina = 100;
    private Vector3 moveDirection;
    private Transform orientation;
    public float groundDrag = 5.0f;
    public float airDragReduction = 2.5f;
    private bool isSprinting = false;
    private bool isWalking = false;

    [Header("Player Skills")]
    public bool unlockDoubleJump;
    public bool unlockJetpack;
    public bool unlockJetpackDash;

    [Header("Player Inventory")]
    public List<ItemBase> playerItems = new List<ItemBase>();
    public List<Weapon> playerWeapons = new List<Weapon>();
    public Weapon currentWeapon;
    private int currentWeaponIndex = 0;

    [HideInInspector]
    public ItemBase itemToPickup; // needs to be public but doesn't need to show in inspector

    [Header("Misc")]
    public LayerMask groundLayer;
    private bool isGrounded;
    private bool canZoom;
    public float timeToZoom = 0.1f; // how long does it take to zoom in/out
    public float zoomedFov = 40f; // fov when zoomed all the way in
    private Coroutine zoomRoutine;
    public bool enableJetpack = true; // the player may not want to always have the jetpack enabled
    private bool isJumping = false;

    private void Start()
    {
        // object references initialized before anything else
        _mainCamera = Camera.main;
        _player = this.gameObject;
        _rb = GetComponent<Rigidbody>();
        orientation = this.transform;

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

        StartCoroutine(HealthRegen());
        StartCoroutine(FuelRegen());

        defaultFov = _mainCamera.fieldOfView;

        canZoom = true;

        _rb.freezeRotation = true;
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
            if (Input.GetKey(KeyCode.Space) && isGrounded && jumpReady)
            {
                isJumping = true;
            }
            else if (Input.GetKeyDown(KeyCode.Space) && !isGrounded && doubleJumpReady && unlockDoubleJump)
            {
                StartCoroutine(DoubleJump());
            }
            else
            {
                isJumping = false;
            }

            HandleActions();
            HandleInventory();

            firstPersonCamera.fieldOfView = _mainCamera.fieldOfView;
        }
        if (gameLogic.timeScale == 0)
        {
            gameLogic.enablePlayerCameraControls = false;
        }

        if (jetpackFuel > (int)maxJetpackFuel)
            jetpackFuel = (int)maxJetpackFuel;

        if (healthSlider != null)
        {
            healthSlider.value = currentHp;
            healthSlider.maxValue = maxHp;
        }

        if (jetpackFuelSlider != null)
        {
            jetpackFuelSlider.value = jetpackFuel;
            jetpackFuelSlider.maxValue = maxJetpackFuel;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            canRegenFuel = true;
        }

        if (isGrounded)
        {
            _rb.drag = groundDrag;
        }
        else
        {
            _rb.drag = 0;
        }

        // Speed limits
        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        if (flatVel.magnitude > runSpeed && !isSprinting)
        {
            Vector3 limitedVel = flatVel.normalized * runSpeed;
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
        }

        if (!isSprinting)
        {
            if (!isWalking && flatVel.magnitude > runSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * runSpeed;
                _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
            }
            else if (isWalking && flatVel.magnitude > walkSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * walkSpeed;
                _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
            }
        }
        else if (isSprinting && flatVel.magnitude > sprintSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * sprintSpeed;
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
        }
    }

    private void FixedUpdate()
    {
        if (currentHp <= 0)
        {

        }
        else
        {
            HandleMovement();

            // jetpack
            if (Input.GetKey(KeyCode.Space) && !isGrounded && !unlockDoubleJump)
            {
                StartCoroutine(HandleJetpack());
            }
            else if (Input.GetKey(KeyCode.Space) && !isGrounded && !doubleJumpReady)
            {
                StartCoroutine(HandleJetpack());
            }

            // need to set transform rotation BEFORE camera rotation (prevents camera jitter)
            transform.rotation = Quaternion.Euler(0, rotation.y, 0);
        }
    }

    private void LateUpdate()
    {
        // ensure the two booleans match
        enablePlayerMovementControls = gameLogic.enablePlayerMovementControls;
        enablePlayerCameraControls = gameLogic.enablePlayerCameraControls;

        // need to set camera rotation in late update (prevents camera jitter)
        HandleCamera();
    }

    // movement controls
    private void HandleMovement()
    {
        if (transform != null && enablePlayerMovementControls)
        {
            // handle sprint
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isSprinting = true;
            }
            else
            {
                isSprinting = false;
            }

            // handle walk
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                isWalking = true;
            }
            else
            {
                isWalking = false;
            }

            // forward movement direction set
            if (Input.GetKey(KeyCode.W))
            {
                // forward and left
                if (Input.GetKey(KeyCode.A))
                {
                    moveDirection = orientation.forward + -orientation.right;
                }
                // forward and right
                else if (Input.GetKey(KeyCode.D))
                {
                    moveDirection = orientation.forward + orientation.right;
                }
                // forward only
                else
                {
                    moveDirection = orientation.forward;
                }
            }
            // backwards movement direction set
            else if (Input.GetKey(KeyCode.S))
            {
                // backwards and left
                if (Input.GetKey(KeyCode.A))
                {
                    moveDirection = -orientation.forward + -orientation.right;
                }
                // backwards and right
                else if (Input.GetKey(KeyCode.D))
                {
                    moveDirection = -orientation.forward + orientation.right;
                }
                // backwards only
                else
                {
                    moveDirection = -orientation.forward;
                }
            }
            // left only
            else if (Input.GetKey(KeyCode.A))
            {
                moveDirection = -orientation.right;
            }
            // right only
            else if (Input.GetKey(KeyCode.D))
            {
                moveDirection = orientation.right;
            }
            // stop movement
            else
            {
                moveDirection = orientation.forward * 0 + orientation.right * 0;
            }

            // take our moveDirection and apply it to our current speed (multiplied by 10 so we actually get decent movement (you don't need to multiply by 10, you will just need to set the speeds way higher in the inspector))
            if (isGrounded)
            {
                if (!isSprinting)
                {
                    if (!isWalking)
                        _rb.AddForce(moveDirection.normalized * runSpeed * 10, ForceMode.Force);
                    else
                        _rb.AddForce(moveDirection.normalized * walkSpeed * 10, ForceMode.Force);
                }
                else if (isSprinting)
                {
                    _rb.AddForce(moveDirection.normalized * sprintSpeed * 10, ForceMode.Force);
                }
            }
            else if (!isGrounded)
            {
                if (!isSprinting)
                {
                    if (!isWalking)
                        _rb.AddForce(moveDirection.normalized * runSpeed * 10 * airDragReduction, ForceMode.Force);
                    else
                        _rb.AddForce(moveDirection.normalized * walkSpeed * 10 * airDragReduction, ForceMode.Force);
                }
                else if (isSprinting)
                {
                    _rb.AddForce(moveDirection.normalized * sprintSpeed * 10 * airDragReduction, ForceMode.Force);
                }
            }
        }

        // jump
        if (jumpReady && isGrounded && enablePlayerMovementControls && isJumping)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);

            jumpReady = false;
            isJumping = false;
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
        if (UnityEditor.EditorApplication.isPlaying) // this will prevent the project from building, remove the slow-time code before building. this feature should not be in a build
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

        // toggle jetpack
        if (Input.GetKeyDown(KeyCode.J))
        {
            enableJetpack = !enableJetpack;
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
            if (currentWeaponIndex < playerWeapons.Count - 1) // only executes if we are not at the last index
            {
                // play unequip animation
                // disable currentWeapon gameObject
                currentWeaponIndex += 1;
                currentWeapon = playerWeapons[currentWeaponIndex];
                // enable currentWeapon gameObject
                // play equip animation
                Debug.Log($"Equipped {currentWeapon.name}!");
            }
            else // otherwise we set the currentWeapon to our first weapon (we don't need this if we don't want to let the player scroll all the way through the list endlessly)
            {
                // play unequip animation
                // disable currentWeapon gameObject
                currentWeaponIndex = 0;
                currentWeapon = playerWeapons[currentWeaponIndex];
                // enable currentWeapon gameObject
                // play equip animation
                Debug.Log($"Equipped {currentWeapon.name}!");
            }

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            // cycle weapon down
            if (currentWeaponIndex > 0) // only executes if we are not at the first index
            {
                // play unequip animation
                // disable currentWeapon gameObject
                currentWeapon = playerWeapons[currentWeaponIndex - 1];
                currentWeaponIndex -= 1;
                // enable currentWeapon gameObject
                // play equip animation
                Debug.Log($"Equipped {currentWeapon.name}!");
            }
            else // otherwise we set the currentWeapon to our last weapon (same as above)
            {
                // play unequip animation
                // disable currentWeapon gameObject
                currentWeaponIndex = playerWeapons.Count - 1;
                currentWeapon = playerWeapons[currentWeaponIndex];
                // enable currentWeapon gameObject
                // play equip animation
                Debug.Log($"Equipped {currentWeapon.name}!");
            }
        }
    }

    private IEnumerator DoubleJump()
    {
        if (unlockDoubleJump && doubleJumpReady && !isGrounded && enablePlayerMovementControls)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);

            doubleJumpReady = false;
        }
        yield return null;
    }

    private IEnumerator HandleJetpack()
    {
        if (jetpackFuel > 0 && enableJetpack && unlockJetpack)
        {
            canRegenFuel = false;

            _rb.velocity = (_rb.velocity * 0.965f); // reduces velocity as the jetpack is used, provides a "cushion" when used while falling

            _rb.AddForce(Vector2.up * 0.095f, ForceMode.Impulse); // increases takeoff speed, and allows momentum change when falling

            _rb.AddForce(Vector2.up * (jetpackPower), ForceMode.Acceleration);

            jetpackFuel -= 1;
        }

        yield return null;
    }

    private IEnumerator FuelRegen()
    {
        while (true)
        {
            if (jetpackFuel < maxJetpackFuel && canRegenFuel)
            {
                jetpackFuel += fuelRegenAmount;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator HealthRegen()
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

        while (timeElapsed < timeToZoom)
        {
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
