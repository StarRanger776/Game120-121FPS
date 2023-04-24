using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))] // using rigidbody for movement
[RequireComponent(typeof(CapsuleCollider))] // using capsule collider for crouching and hitbox
public class PlayerController : MonoBehaviour
{
    [Header("Mouse Controls")]
    public Vector2 mouseSensitivity;

    [Header("Object References")]
    public GameLogic gameLogic;
    private Camera _mainCamera;
    public Camera firstPersonCamera;
    private GameObject _player;
    private Rigidbody _rb;
    public Slider healthSlider;
    public Slider jetpackFuelSlider;
    public GameObject flashlight;
    public GameObject rightArmOnly;
    public GameObject bothArms;

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
    private float playerHeight = 2;
    private bool jumpReady;
    private bool doubleJumpReady;
    private bool enablePlayerMovementControls, enablePlayerCameraControls, enablePlayerGravity; // lets us disable camera/movement controls which can be useful for certain animations or cutscenes
    public bool canRegenHp = false; // regen Hp over time
    public bool canRegenFuel = true; // global fuel regen, never gets automatically enabled or disabled
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
    [HideInInspector]
    public Vector2 rotation;
    private bool flashlightEnabled = false;
    public float dashSpeed = 10f;
    public float gravityMultiplier = 1.0f;

    [Header("Player Skills")]
    public bool unlockDoubleJump;
    public bool unlockJetpack;
    public bool unlockJetpackDash;

    [Header("Player Inventory")]
    public List<ItemBase> playerItems = new List<ItemBase>();
    public List<Weapon> playerWeapons = new List<Weapon>();
    public Weapon currentWeapon;
    private int currentWeaponIndex = 0;
    public int currentPistolAmmo;
    public int currentRifleAmmo;
    public int currentLaserAmmo;
    public uint maxPistolAmmo = 50;
    public uint maxRifleAmmo = 60;
    public uint maxLaserAmmo = 50;
    public int currentMoney = 0;
    public int moneyEarned = 0;

    [HideInInspector]
    public ItemBase itemToPickup; // needs to be public but doesn't need to show in inspector
    [HideInInspector]
    public Weapon weaponToPickup;

    [Header("Misc")]
    public LayerMask groundLayer;
    private bool isGrounded;
    private bool canZoom;
    public float timeToZoom = 0.1f; // how long does it take to zoom in/out
    public float zoomedFov = 40f; // fov when zoomed all the way in
    public float timeToCrouch = 0.1f;
    private Coroutine zoomRoutine;
    private Coroutine crouchRoutine;
    private Coroutine dashRoutine;
    private Coroutine refuelRoutine;
    private Coroutine regenRoutine;
    private bool enableJetpack = true; // the player may not want to always have the jetpack enabled
    private bool jetpackInUse = false;
    private bool isJumping = false;
    // private bool isCrouching = false;
    [HideInInspector]
    public bool isReloading = false;
    public bool readyToDash = true;
    public bool isRegeningHp = false;
    public bool isRegeningFuel = false;

    private void Start()
    {
        // object references initialized before anything else
        _mainCamera = Camera.main;
        _player = this.gameObject;
        _rb = GetComponent<Rigidbody>();
        orientation = this.transform;

        defaultFov = _mainCamera.fieldOfView;

        // default settings for mouse sensitivity
        if (mouseSensitivity.x == 0)
            mouseSensitivity.x = 100;
        if (mouseSensitivity.y == 0)
            mouseSensitivity.y = 100;

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

        canZoom = true;

        _rb.freezeRotation = true;
        _rb.useGravity = false;
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
            HandleCamera();

            // dash
            if (unlockJetpackDash && Input.GetKeyDown(KeyCode.X))
            {
                StartCoroutine(HandleDash());
            }

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

        if (currentPistolAmmo > maxPistolAmmo)
            currentPistolAmmo = (int)maxPistolAmmo;
        if (currentRifleAmmo > maxRifleAmmo)
            currentRifleAmmo = (int)maxRifleAmmo;
        if (currentLaserAmmo > maxLaserAmmo)
            currentLaserAmmo = (int)maxLaserAmmo;

        if (currentPistolAmmo < 0)
            currentPistolAmmo = 0;
        if (currentRifleAmmo < 0)
            currentRifleAmmo = 0;
        if (currentLaserAmmo < 0)
            currentLaserAmmo = 0;

        // new gravity
        if (enablePlayerGravity)
        {
            if (!jetpackInUse || jetpackFuel <= 0)
                _rb.AddForce(new Vector3(0, -1.0f, 0) * _rb.mass * (gravityMultiplier * 987) * Time.deltaTime);
        }

        // match weapon to item
        if (itemToPickup != null && itemToPickup.type.ToUpper().Equals("WEAPON"))
        {
            weaponToPickup = (Weapon)itemToPickup;
        }
        else if (itemToPickup == null)
        {
            weaponToPickup = null;
        }

        // switches the hands to the correct layout
        if (currentWeapon.isTwoHanded)
        {
            rightArmOnly.SetActive(false);
            bothArms.SetActive(true);
        }
        else if (!currentWeapon.isTwoHanded)
        {
            bothArms.SetActive(false);
            rightArmOnly.SetActive(true);
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
            if (Input.GetKey(KeyCode.Space) && !isGrounded && !unlockDoubleJump && enablePlayerMovementControls)
            {
                StartCoroutine(HandleJetpack());
            }
            else if (Input.GetKey(KeyCode.Space) && !isGrounded && !doubleJumpReady && enablePlayerMovementControls)
            {
                StartCoroutine(HandleJetpack());
            }
            else if (!Input.GetKey(KeyCode.Space))
            {
                jetpackInUse = false;
            }

            // need to set transform rotation BEFORE camera rotation (prevents camera jitter)
            transform.rotation = Quaternion.Euler(0, rotation.y, 0);

            // fuel regen
            if (canRegenFuel)
            {
                if (!isRegeningFuel)
                {
                    StartCoroutine(FuelRegen());
                }
            }

            // health regen
            if (canRegenHp)
            {
                if (!isRegeningHp)
                {
                    StartCoroutine(HealthRegen());
                }
            }
        }
    }

    private void LateUpdate()
    {
        // ensure the booleans match
        enablePlayerMovementControls = gameLogic.enablePlayerMovementControls;
        enablePlayerCameraControls = gameLogic.enablePlayerCameraControls;
        enablePlayerGravity = gameLogic.enablePlayerGravity;

        // need to set camera rotation in late update (prevents camera jitter)
        _mainCamera.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
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
        if (jumpReady && isGrounded && enablePlayerMovementControls && isJumping && enablePlayerMovementControls)
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
            jetpackInUse = false;
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
        }
    }

    private void HandleActions()
    {
        // Interact
        if (Input.GetKeyDown(KeyCode.E) && enablePlayerMovementControls)
        {
            // pickup item
            if (itemToPickup != null && itemToPickup.canBePickedUp && itemToPickup.readyToBePickedUp && itemToPickup.type.ToUpper().Equals("WEAPON"))
            {
                playerWeapons.Add(weaponToPickup.weaponToUseOnPlayer);
                if (itemToPickup.pickupText != null)
                    Destroy(itemToPickup.pickupText.gameObject);
                weaponToPickup.weaponToActivateOnPlayer.SetActive(true);
                if (itemToPickup.transform.parent != null)
                {
                    Destroy(itemToPickup.transform.parent.gameObject);
                }
                else
                {
                    Destroy(itemToPickup.gameObject);
                }
                itemToPickup = null;
            }
            else if (itemToPickup != null && itemToPickup.canBePickedUp && itemToPickup.readyToBePickedUp && itemToPickup.type.ToUpper().Equals("BASIC HEALTH PACK"))
            {
                currentHp += 20;
                itemToPickup.gameObject.SetActive(false);
                if (itemToPickup.pickupText != null)
                    itemToPickup.pickupText.gameObject.SetActive(false);
                itemToPickup = null;
            }
            else if (itemToPickup != null && itemToPickup.canBePickedUp && itemToPickup.readyToBePickedUp && itemToPickup.type.ToUpper().Equals("LARGE HEALTH PACK"))
            {
                currentHp += 100;
                itemToPickup.gameObject.SetActive(false);
                if (itemToPickup.pickupText != null)
                    itemToPickup.pickupText.gameObject.SetActive(false);
                itemToPickup = null;
            }
            else if (itemToPickup != null && itemToPickup.canBePickedUp && itemToPickup.readyToBePickedUp && itemToPickup.type.ToUpper().Equals("JET REFUEL"))
            {
                jetpackFuel = (int)maxJetpackFuel;
                itemToPickup.gameObject.SetActive(false);
                if (itemToPickup.pickupText != null)
                    itemToPickup.pickupText.gameObject.SetActive(false);
                itemToPickup = null;
            }
            else if (itemToPickup != null && itemToPickup.canBePickedUp && itemToPickup.readyToBePickedUp && itemToPickup.type.ToUpper().Equals("SMALL PISTOL AMMO PICKUP"))
            {
                currentPistolAmmo += 4;
                Destroy(itemToPickup.gameObject);
                if (itemToPickup.pickupText != null)
                    Destroy(itemToPickup.pickupText.gameObject);
                itemToPickup = null;
            }
            else if (itemToPickup != null && itemToPickup.canBePickedUp && itemToPickup.readyToBePickedUp && itemToPickup.type.ToUpper().Equals("MEDIUM PISTOL AMMO PICKUP"))
            {
                currentPistolAmmo += 8;
                Destroy(itemToPickup.gameObject);
                if (itemToPickup.pickupText != null)
                    Destroy(itemToPickup.pickupText.gameObject);
                itemToPickup = null;
            }
            else if (itemToPickup != null && itemToPickup.canBePickedUp && itemToPickup.readyToBePickedUp && itemToPickup.type.ToUpper().Equals("LARGE PISTOL AMMO PICKUP"))
            {
                currentPistolAmmo += 12;
                Destroy(itemToPickup.gameObject);
                if (itemToPickup.pickupText != null)
                    Destroy(itemToPickup.pickupText.gameObject);
                itemToPickup = null;
            }
            else if (itemToPickup != null && itemToPickup.canBePickedUp && itemToPickup.readyToBePickedUp && itemToPickup.type.ToUpper().Equals("HUGE PISTOL AMMO PICKUP"))
            {
                currentPistolAmmo += 18;
                Destroy(itemToPickup.gameObject);
                if (itemToPickup.pickupText != null)
                    Destroy(itemToPickup.pickupText.gameObject);
                itemToPickup = null;
            }
            else if (itemToPickup != null && itemToPickup.canBePickedUp && itemToPickup.readyToBePickedUp && itemToPickup.type.ToUpper().Equals("SMALL RIFLE AMMO PICKUP"))
            {
                currentRifleAmmo += 12;
                Destroy(itemToPickup.gameObject);
                if (itemToPickup.pickupText != null)
                    Destroy(itemToPickup.pickupText.gameObject);
                itemToPickup = null;
            }
            else if (itemToPickup != null && itemToPickup.canBePickedUp && itemToPickup.readyToBePickedUp && itemToPickup.type.ToUpper().Equals("MEDIUM RIFLE AMMO PICKUP"))
            {
                currentRifleAmmo += 18;
                Destroy(itemToPickup.gameObject);
                if (itemToPickup.pickupText != null)
                    Destroy(itemToPickup.pickupText.gameObject);
                itemToPickup = null;
            }
            else if (itemToPickup != null && itemToPickup.canBePickedUp && itemToPickup.readyToBePickedUp && itemToPickup.type.ToUpper().Equals("LARGE RIFLE AMMO PICKUP"))
            {
                currentRifleAmmo += 24;
                Destroy(itemToPickup.gameObject);
                if (itemToPickup.pickupText != null)
                    Destroy(itemToPickup.pickupText.gameObject);
                itemToPickup = null;
            }
            else if (itemToPickup != null && itemToPickup.canBePickedUp && itemToPickup.readyToBePickedUp && itemToPickup.type.ToUpper().Equals("HUGE RIFLE AMMO PICKUP"))
            {
                currentRifleAmmo += 32;
                Destroy(itemToPickup.gameObject);
                if (itemToPickup.pickupText != null)
                    Destroy(itemToPickup.pickupText.gameObject);
                itemToPickup = null;
            }
            else if (itemToPickup != null && itemToPickup.canBePickedUp && itemToPickup.readyToBePickedUp) // create other pickups ABOVE this pickup. this should be last ALWAYS
            {
                if (itemToPickup.transform.parent != null)
                {
                    DontDestroyOnLoad(itemToPickup.transform.parent);
                }
                else
                {
                    DontDestroyOnLoad(itemToPickup);
                }
                playerItems.Add(itemToPickup);
                itemToPickup.gameObject.SetActive(false);
                if (itemToPickup.pickupText != null)
                    itemToPickup.pickupText.gameObject.SetActive(false);
                itemToPickup = null;
            }
        }

        // toggle flashlight
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (flashlightEnabled)
                flashlight.SetActive(false);
            else if (!flashlightEnabled)
                flashlight.SetActive(true);

            flashlightEnabled = !flashlightEnabled;
        }

        // Slow down time (editor only)
        if (true) // this will prevent the project from building, remove the slow-time code before building. this feature should not be in a build
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

        // crouching
        if (Input.GetKeyDown(KeyCode.LeftControl) && enablePlayerMovementControls)
        {
            if (crouchRoutine != null)
            {
                StopCoroutine(crouchRoutine);
                crouchRoutine = null;
            }

            crouchRoutine = StartCoroutine(HandleCrouch(true));
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (crouchRoutine != null)
            {
                StopCoroutine(crouchRoutine);
                crouchRoutine = null;
            }

            crouchRoutine = StartCoroutine(HandleCrouch(false));
        }

        // weapon shoot
        if (Input.GetMouseButton(0) && currentWeapon != null && enablePlayerMovementControls)
        {
            if (currentWeapon.loadedAmmo > 0)
                currentWeapon.Shoot();
            else
            {
                if (!isReloading)
                    StartCoroutine(Reload(currentWeapon.type));
            }
        }

        // reload
        if (Input.GetKeyDown(KeyCode.R) && enablePlayerMovementControls)
        {
            if (!isReloading)
                StartCoroutine(Reload(currentWeapon.type));
        }
    }

    /* HandleInventory should control what weapons
     * are displayed for the player.
     * Need to cycle through the weapons list whenever the player
     * switches weapons.
     * HandleInventory should also control things like keys in the inventory.*/
    private void HandleInventory()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && enablePlayerMovementControls)
        {
            currentWeapon.readyToShoot = true;
            // cycle weapon up
            if (currentWeaponIndex < playerWeapons.Count - 1) // only executes if we are not at the last index
            {
                currentWeaponIndex += 1;
                currentWeapon = playerWeapons[currentWeaponIndex];
                Debug.Log($"Equipped {currentWeapon.name}!");
            }
            else // otherwise we set the currentWeapon to our first weapon (we don't need this if we don't want to let the player scroll all the way through the list endlessly)
            {
                currentWeaponIndex = 0;
                currentWeapon = playerWeapons[currentWeaponIndex];
                Debug.Log($"Equipped {currentWeapon.name}!");
            }

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && enablePlayerMovementControls)
        {
            currentWeapon.readyToShoot = true;
            // cycle weapon down
            if (currentWeaponIndex > 0) // only executes if we are not at the first index
            {
                currentWeapon = playerWeapons[currentWeaponIndex - 1];
                currentWeaponIndex -= 1;
                Debug.Log($"Equipped {currentWeapon.name}!");
            }
            else // otherwise we set the currentWeapon to our last weapon (same as above)
            {
                currentWeaponIndex = playerWeapons.Count - 1;
                currentWeapon = playerWeapons[currentWeaponIndex];
                Debug.Log($"Equipped {currentWeapon.name}!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
    }

    public IEnumerator Reload(string weaponType)
    {
        isReloading = true;
        Weapon weaponToReload = currentWeapon;

        if (weaponType.ToUpper().Equals("PISTOL"))
        {
            currentPistolAmmo += currentWeapon.loadedAmmo;
            currentWeapon.loadedAmmo = 0;
        }
        else if (weaponType.ToUpper().Equals("RIFLE"))
        {
            currentRifleAmmo += currentWeapon.loadedAmmo;
            currentWeapon.loadedAmmo = 0;
        }

        yield return new WaitForSeconds(currentWeapon.reloadTime);

        if (weaponToReload == currentWeapon)
        {
            if (weaponType.ToUpper().Equals("PISTOL"))
            {
                if (currentPistolAmmo + currentWeapon.loadedAmmo > (int)currentWeapon.maxAmmoBeforeReload)
                {
                    currentPistolAmmo -= (int)currentWeapon.maxAmmoBeforeReload;
                    currentWeapon.loadedAmmo = (int)currentWeapon.maxAmmoBeforeReload;
                }
                else
                {
                    currentWeapon.loadedAmmo = currentPistolAmmo;
                    currentPistolAmmo = 0;
                }
            }
            else if (weaponType.ToUpper().Equals("RIFLE"))
            {
                if (currentRifleAmmo + currentWeapon.loadedAmmo > (int)currentWeapon.maxAmmoBeforeReload)
                {
                    currentRifleAmmo -= (int)currentWeapon.maxAmmoBeforeReload;
                    currentWeapon.loadedAmmo = (int)currentWeapon.maxAmmoBeforeReload;
                }
                else
                {
                    currentWeapon.loadedAmmo = currentRifleAmmo;
                    currentRifleAmmo = 0;
                }
            }
            else if (weaponType.ToUpper().Equals("LASER"))
            {
                if (currentLaserAmmo + currentWeapon.loadedAmmo > (int)currentWeapon.maxAmmoBeforeReload)
                {
                    currentLaserAmmo -= (int)currentWeapon.maxAmmoBeforeReload;
                    currentWeapon.loadedAmmo = (int)currentWeapon.maxAmmoBeforeReload;
                }
                else
                {
                    currentWeapon.loadedAmmo = currentLaserAmmo;
                    currentLaserAmmo = 0;
                }
            }
        }
        else
        {
            isReloading = false;
            yield return null;
        }

        isReloading = false;
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
            jetpackInUse = true;

            _rb.velocity = (_rb.velocity * 0.97f); // reduces velocity as the jetpack is used, provides a "cushion" when used while falling

            _rb.AddForce(Vector2.up * 0.095f, ForceMode.Impulse); // increases takeoff speed, and allows momentum change when falling

            _rb.AddForce(Vector2.up * (jetpackPower), ForceMode.Acceleration);

            jetpackFuel -= 1;
        }

        yield return null;
    }

    private IEnumerator FuelRegen()
    {
        while (canRegenFuel)
        {
            isRegeningFuel = true;

            if (jetpackFuel < maxJetpackFuel && !jetpackInUse && canRegenFuel)
            {
                jetpackFuel += fuelRegenAmount;
            }
            yield return new WaitForSeconds(0.05f);
            isRegeningFuel = false;
        }
    }

    private IEnumerator HealthRegen()
    {
        while (canRegenHp)
        {
            isRegeningHp = true;

            if (currentHp < (int)maxHp && canRegenHp)
                currentHp += 1;
            yield return new WaitForSeconds(0.5f);
            isRegeningHp = false;
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

    private IEnumerator HandleCrouch(bool enterCrouch)
    {
        float desiredHeight = enterCrouch ? playerHeight / 2 : playerHeight;
        float startingHeight = playerHeight;
        float timeElapsed = 0;

        while (timeElapsed < timeToCrouch)
        {
            _player.GetComponent<CapsuleCollider>().height = Mathf.Lerp(startingHeight, desiredHeight, (timeElapsed / timeToCrouch));
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _player.GetComponent<CapsuleCollider>().height = desiredHeight;
        crouchRoutine = null;
    }

    private IEnumerator HandleDash()
    {
        if (readyToDash)
        {
            _rb.AddForce(moveDirection.normalized * dashSpeed * 10, ForceMode.Impulse);
            readyToDash = false;

            if (dashRoutine != null)
            {
                StopCoroutine(dashRoutine);
                dashRoutine = null;
            }

            StartCoroutine(ResetDash(true));
        }

        yield return null;
    }

    private IEnumerator ResetDash(bool resetDash)
    {
        yield return new WaitForSeconds(0.75f);

        readyToDash = resetDash;
    }

    private IEnumerator PlayerDeath()
    {
        SceneManager.LoadScene("DeathScreen");
        Time.timeScale = 0; // pauses the game, can instead pause the editor but that won't work for an actual BUILD of a unity game
        gameLogic.enablePlayerCameraControls = false;
        gameLogic.enablePlayerMovementControls = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Destroy(_mainCamera.gameObject);
        Destroy(this.gameObject);
        Destroy(FindObjectOfType<PlayerUI>().gameObject);
        Destroy(FindObjectOfType<FirstPerson>().gameObject);
        Destroy(gameLogic.gameObject);
        yield return null;
    }
}