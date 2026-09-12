using UnityEngine;

public class MazeFPController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float mouseSensitivity = 2f;
    public float normalRadius = 0.8f;

    public Transform cameraPivot;
    public Transform leftArmPivot;
    public Transform rightArmPivot;
    public Transform leftLegPivot;
    public Transform rightLegPivot;
    public float limbSwingAngle = 30f;
    public float walkCycleSpeed = 6f;

    public AudioSource footstepSource;
    public AudioClip footstepClip;

    private CharacterController controller;
    private float yaw;
    private float pitch;
    private float walkCyclePhase;
    private int lastStepIndex = -1;

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

        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) move += forwardHoriz;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) move -= forwardHoriz;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) move -= rightHoriz;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) move += rightHoriz;

        bool isMoving = move.sqrMagnitude > 0.0001f;
        if (move.sqrMagnitude > 1f) move.Normalize();

        controller.radius = normalRadius;
        controller.Move(move * moveSpeed * Time.deltaTime);

        AnimateWalk(isMoving);
    }

    void AnimateWalk(bool isMoving)
    {
        if (isMoving)
        {
            walkCyclePhase += Time.deltaTime * walkCycleSpeed;
        }
        else
        {
            walkCyclePhase = Mathf.MoveTowards(walkCyclePhase, Mathf.Round(walkCyclePhase / Mathf.PI) * Mathf.PI, Time.deltaTime * walkCycleSpeed);
        }

        float swing = Mathf.Sin(walkCyclePhase) * limbSwingAngle;

        if (leftLegPivot != null) leftLegPivot.localRotation = Quaternion.Euler(swing, 0f, 0f);
        if (rightLegPivot != null) rightLegPivot.localRotation = Quaternion.Euler(-swing, 0f, 0f);
        if (leftArmPivot != null) leftArmPivot.localRotation = Quaternion.Euler(-swing, 0f, 0f);
        if (rightArmPivot != null) rightArmPivot.localRotation = Quaternion.Euler(swing, 0f, 0f);

        if (isMoving)
        {
            int stepIndex = Mathf.FloorToInt(walkCyclePhase / Mathf.PI);
            if (stepIndex != lastStepIndex)
            {
                lastStepIndex = stepIndex;
                if (footstepSource != null && footstepClip != null)
                {
                    footstepSource.pitch = (stepIndex % 2 == 0) ? 1.0f : 0.94f;
                    footstepSource.PlayOneShot(footstepClip);
                }
            }
        }
        else
        {
            lastStepIndex = -1;
        }
    }
}
