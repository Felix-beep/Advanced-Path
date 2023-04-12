using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scene2
{
    public class CameraActions : MonoBehaviour
    {
        private CameraInput InputMapping;
        public GameObject Player;
        public MapCameraController Script;

        // Start is called before the first frame update
        private void Awake()
        {
            InputMapping = new CameraInput();
        }

        public void OnEnable()
        {
            InputMapping.Enable();
            InputMapping.CameraActions.FocusOnPlayer.performed += JumpToPlayer; 
        }

        public void OnDisable()
        {
            InputMapping.Disable();
            InputMapping.CameraActions.FocusOnPlayer.performed -= JumpToPlayer;
        }

        public void JumpToPlayer(InputAction.CallbackContext obj)
        {
            Script.lastPosition = Player.transform.position;
            this.transform.position = Player.transform.position;
        }
    }
}
