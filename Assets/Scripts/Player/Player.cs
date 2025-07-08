using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using Cursor = UnityEngine.Cursor;

public class Player : MonoBehaviour
{
    [SerializeField] float movementSpeed = 0.1f;
    [SerializeField] float mouseLookSensitivity = 1000.0f;
    [SerializeField] float rotateSensitivity = 1.0f;

    float xRotation;

    public Camera cam;

    bool locked = true;
    public static Player instance;

    const float lookSpeedX = 1;
    const float lookSpeedY = 1;

    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
        UnlockCursor();
        instance = this;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        locked = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        locked  = false;
    }

    bool mobile_move_up = false;
    bool mobile_move_down = false;
    public void MoveUpToggle(bool value)
    {
        mobile_move_up = value;
    }

    public void MoveDownToggle(bool value)
    {
        mobile_move_down = value;
    }

    private void ApplyMouseLookRotation(float yawDelta, float pitchDelta)
    {
        xRotation -= pitchDelta;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.Rotate(Vector3.up * yawDelta, Space.World);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void Update()
    {

        Vector3 move = Vector3.zero;
        float rotYaw = 0;
        float rotPitch= 0;
     
        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.D)) move += transform.right;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;
        if (Input.GetKey(KeyCode.Space)) move += Vector3.up;
        if (Input.GetKey(KeyCode.LeftShift)) move -= Vector3.up;
        if (Input.GetKey(KeyCode.LeftAlt)) UnlockCursor();

        if (Input.GetKeyDown(KeyCode.G))
        {
            this.GetComponent<Collider>().enabled = !this.GetComponent<Collider>().enabled;
            this.GetComponent<Rigidbody>().useGravity = !this.GetComponent<Rigidbody>().useGravity;
            this.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }


        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                LockCursor();
            }
        }

        if (locked)
        {
            // Desktop: mouse look
            float mouseX = Input.GetAxis("Mouse X") * lookSpeedX;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeedY;

            rotYaw = mouseX;
            rotPitch = mouseY;
        }

        // apply move
        transform.Translate(move * movementSpeed * Time.unscaledDeltaTime, Space.World);
        ApplyMouseLookRotation(rotYaw, rotPitch);


    }



  


  
}
