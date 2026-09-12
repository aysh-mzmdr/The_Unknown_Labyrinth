using UnityEngine;

public class MazeFPController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float mouseSensitivity = 2f;
    public float normalRadius = 0.8f;

    public Transform cameraPivot;
    public Animator animator;

    private CharacterController controller;
    private float yaw;
    private float pitch;
    private float currentSpeed;
    private Vector3 lastMoveDirection = Vector3.forward;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        controller.radius = normalRadius;
    }

    void Start()
    {
        yaw = transform.eulerAngles.y;
        pitch = 0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool locked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = locked;
        }

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (cameraPivot != null) cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        Vector3 forwardHoriz = transform.forward;
        Vector3 rightHoriz = transform.right;

        Vector3 inputDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) inputDir += forwardHoriz;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) inputDir -= forwardHoriz;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) inputDir -= rightHoriz;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) inputDir += rightHoriz;

        bool isMoving = inputDir.sqrMagnitude > 0.0001f;
        if (isMoving)
        {
            inputDir.Normalize();
            lastMoveDirection = inputDir;
        }

        bool forwardHeld = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool backHeld = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        bool backTapReleased = (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow)) && forwardHeld;
        bool forwardTapReleased = (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow)) && backHeld;
        bool hardStop = backTapReleased || forwardTapReleased;

        if (hardStop)
        {
            currentSpeed = 0f;
            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
                animator.Play("Standing Idle", 0, 0f);
            }
        }
        else
        {
            if (animator != null) animator.SetBool("IsMoving", isMoving);
            currentSpeed = isMoving ? moveSpeed : 0f;
        }

        controller.radius = normalRadius;
        if (currentSpeed > 0.0001f)
        {
            controller.Move(lastMoveDirection * currentSpeed * Time.deltaTime);
        }
    }
}
