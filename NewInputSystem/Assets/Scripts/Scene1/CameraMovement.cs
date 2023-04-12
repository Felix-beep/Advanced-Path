using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scene2
{
    public class CameraMovement : MonoBehaviour
    {
        private CameraInput InputMapping;
        public Transform CameraTransform;

        private Vector3 targetPosition;

        // variables for movement
        public float speed;
        public float maxSpeed = 30f;
        public float acceleration = 1f;
        public Vector3 horizontalVelocity;
        public float damping = 2f;

        private void Awake()
        {
            InputMapping = new CameraInput();
        }

        public void OnEnable()
        {
            InputMapping.Enable();
        }

        public void OnDisable()
        {
            InputMapping.Disable();
        }

        public void Update()
        {
            GetMovement();
            UpdatePosition();
        }

        private void GetMovement()
        {
            // get the Vector 2 from InputActionMapping
            Vector2 InputVector = InputMapping.CameraMovement.HorizontalMovement.ReadValue<Vector2>();

            // convert it to Vector 3 in relationship to the camera4
            Vector3 MovementVector = InputVector.x * GetCameraRight() + InputVector.y * GetCameraForward();

            // normalize it, so the effective movement is always the same
            MovementVector = MovementVector.normalized;

            // check if its big enough to not be an accidental input
            if (MovementVector.sqrMagnitude > 0.1f)
            {
                targetPosition += MovementVector;
            }
        }

        private void UpdatePosition()
        {
            if (targetPosition.sqrMagnitude > 0.1f)
            {
                // set the direction you want to move to
                horizontalVelocity = targetPosition;

                //create a smooth ramp up
                speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * acceleration);
            }
            else
            {
                //create smooth slow down
                speed = Mathf.Lerp(speed, 0, Time.deltaTime * damping);

            }

            transform.position += horizontalVelocity * speed * Time.deltaTime; ;

            //reset for next frame
            targetPosition = Vector3.zero;
            if (speed < 1f) speed = 0;
        }


        // Util functions
        //gets the horizontal forward vector of the camera
        private Vector3 GetCameraForward()
        {
            Vector3 forward = CameraTransform.forward;
            forward.y = 0f;
            return forward;
        }

        //gets the horizontal right vector of the camera
        private Vector3 GetCameraRight()
        {
            Vector3 right = CameraTransform.right;
            right.y = 0f;
            return right;
        }
    }
}
