using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform cameraPivot;

    [Header("Movimiento")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 6.5f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float jumpHeight = 1.1f;
    [SerializeField] private float gravity = -20f;

    [Header("Cámara")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float gamepadSensitivity = 120f;
    [SerializeField] private float minPitch = -85f;
    [SerializeField] private float maxPitch = 85f;
    [SerializeField] private bool lockCursor = true;

    private CharacterController controller;

    private PlayerInput playerInput;

    private Vector3 horizontalVelocity;
    private float verticalVelocity;
    private float pitch;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraPivot == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) cameraPivot = cam.transform;
        }

        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        if (lockCursor) SetCursorLocked(true);
    }

    private void OnDisable()
    {

        SetCursorLocked(false);
    }

    private void Update()
    {
        // Escape libera el cursor, click izquierdo lo vuelve a bloquear
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            SetCursorLocked(false);
        if (lockCursor && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            SetCursorLocked(true);

        HandleLook();
        HandleMovement();
    }

    private void HandleLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        Vector2 look = playerInput.actions["Look"].ReadValue<Vector2>();

        bool usingMouse = playerInput.actions["Look"].activeControl != null &&
                          playerInput.actions["Look"].activeControl.device is Mouse;

        Vector2 delta = usingMouse
            ? look * mouseSensitivity
            : look * gamepadSensitivity * Time.deltaTime;

        // Yaw: gira todo el cuerpo
        transform.Rotate(Vector3.up * delta.x);

        // Pitch: solo la cámara, limitado
        pitch = Mathf.Clamp(pitch - delta.y, minPitch, maxPitch);
        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleMovement()
    {
        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
        float targetSpeed = playerInput.actions["Sprint"].IsPressed() ? sprintSpeed : walkSpeed;

        Vector3 targetVelocity =
            (transform.right * input.x + transform.forward * input.y) * targetSpeed;

        // Aceleración suave
        horizontalVelocity = Vector3.MoveTowards(
            horizontalVelocity, targetVelocity, acceleration * Time.deltaTime * targetSpeed);

        // Gravedad y salto
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f; // pega al suelo

        if (controller.isGrounded && playerInput.actions["Jump"].WasPressedThisFrame())
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = horizontalVelocity + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    private static void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
