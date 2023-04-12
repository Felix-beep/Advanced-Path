using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapCameraController : MonoBehaviour
{
    private CameraInput cameraActions;
    private InputAction movement;
    private InputAction zoomKeyboard;
    public Transform cameraTransform;

    //horizontal motion
    [SerializeField]
    private float maxSpeed = 30f;
    private float speed;
    [SerializeField]
    private float acceleration = 10f;
    [SerializeField]
    private float damping = 15f;

    //vertical motion - zooming
    [SerializeField]
    private float stepSize = 5f;
    [SerializeField]
    private float zoomDampening = 5f;
    [SerializeField]
    private float minHeight = 5f;
    [SerializeField]
    private float maxHeight = 80f;
    //[serializeField]
    //private float zoomSpeed = 5f;

    // rotation
    [SerializeField]
    private float maxRotationSpeed = 0.5f;

    //screen edge motion
    [SerializeField]
    private float edgeTolerance = 0.05f;
    [SerializeField]
    private bool useScreenEdge = false;

    // values changed in functions
    // used to update Camera position, rotation, etc.
    private Vector3 targetPosition;

    private float zoomHeight;

    //used to track and maintain velocity
    private Vector3 horizontalVelocity;
    public Vector3 lastPosition;

    //tracks where the mouse dragging action started
    private Vector3 startDrag;

    private void Awake()
    {
        cameraActions = new CameraInput();
        cameraTransform = this.GetComponentInChildren<Camera>().transform;

        movement = cameraActions.CameraMovement.HorizontalMovement;

        zoomHeight = cameraTransform.localPosition.y;
        cameraTransform.LookAt(this.transform);

        lastPosition = this.transform.position;
    }

    private void OnEnable()
    {
        cameraActions.Enable();
    }

    private void OnDisable()
    {
        Disable();
    }

    public void Enable()
    {
        cameraActions.Enable();
    }

    public void Disable()
    {
        cameraActions.Disable();
    }

    private void Update()
    {
        //inputs
        GetKeyboardMovement();
        CheckMouseAtScreenEdge();
        DragCamera();
        ZoomCamera();
        RotateCamera();

        //move base and camera objects
        UpdateVelocity();
        UpdateBasePosition();
        UpdateCameraPosition();
    }

    private void UpdateVelocity()
    {
        horizontalVelocity = (this.transform.position - lastPosition) / Time.deltaTime;
        horizontalVelocity.y = 0f;
        lastPosition = this.transform.position;
    }

    private void GetKeyboardMovement()
    {
        Vector3 inputValue = movement.ReadValue<Vector2>().x * GetCameraRight()
                    + movement.ReadValue<Vector2>().y * GetCameraForward();

        inputValue = inputValue.normalized;

        if (inputValue.sqrMagnitude > 0.1f)
            targetPosition += inputValue;
    }

    private void DragCamera()
    {
        if (!Mouse.current.rightButton.isPressed)
            return;

        //create plane to raycast to
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Vector2 MouseVector = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(MouseVector);

        if (plane.Raycast(ray, out float distance))
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
                startDrag = ray.GetPoint(distance);
            else
                targetPosition += startDrag - ray.GetPoint(distance);
        }
    }

    private void CheckMouseAtScreenEdge()
    {
        //mouse position is in pixels
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 moveDirection = Vector3.zero;

        //horizontal scrolling
        if (mousePosition.x < edgeTolerance * Screen.width)
            moveDirection += -GetCameraRight();
        else if (mousePosition.x > (1f - edgeTolerance) * Screen.width)
            moveDirection += GetCameraRight();

        //vertical scrolling
        if (mousePosition.y < edgeTolerance * Screen.height)
            moveDirection += -GetCameraForward() * 2;
        else if (mousePosition.y > (1f - edgeTolerance) * Screen.height)
            moveDirection += GetCameraForward() * 2;

        if (useScreenEdge)
            targetPosition += moveDirection;
    }

    private void UpdateBasePosition()
    {
        if (targetPosition.sqrMagnitude > 0.1f)
        {
            //create a ramp up or acceleration
            speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * acceleration);
            transform.position += targetPosition * speed * Time.deltaTime;
        }
        else
        {
            speed = 0;
            //create smooth slow down
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime * damping);
            transform.position += horizontalVelocity * Time.deltaTime;
        }

        //reset for next frame
        targetPosition = Vector3.zero;
    }

    private void ZoomCamera()
    {
        float inputValue = -cameraActions.CameraZooming.Zoom.ReadValue<float>();

        Debug.Log(inputValue);

        if (Mathf.Abs(inputValue) > 0.1f)
        {
            zoomHeight = cameraTransform.localPosition.y + inputValue * stepSize;

            if (zoomHeight < minHeight)
                zoomHeight = minHeight;
            else if (zoomHeight > maxHeight)
                zoomHeight = maxHeight;
        }
    }

    private void UpdateCameraPosition()
    {
        //set zoom target
        Vector3 zoomTarget = new Vector3(cameraTransform.localPosition.x, zoomHeight, cameraTransform.localPosition.z);
        //add vector for forward/backward zoom
        //zoomTarget -= zoomSpeed * (zoomHeight - cameraTransform.localPosition.y) * Vector3.forward;

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, zoomTarget, Time.deltaTime * zoomDampening);
        cameraTransform.LookAt(this.transform);
    }

    private void RotateCamera()
    {
        if (!Mouse.current.middleButton.isPressed)
            return;

        float inputValue = cameraActions.CameraRotation.Rotate.ReadValue<float>();
        transform.rotation = Quaternion.Euler(0f, inputValue * maxRotationSpeed + transform.rotation.eulerAngles.y, 0f);
    }

    //gets the horizontal forward vector of the camera
    private Vector3 GetCameraForward()
    {
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        return forward;
    }

    //gets the horizontal right vector of the camera
    private Vector3 GetCameraRight()
    {
        Vector3 right = cameraTransform.right;
        right.y = 0f;
        return right;
    }
}
