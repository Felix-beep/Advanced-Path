using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraActions : MonoBehaviour
{
    private CameraInput InputMapping;
    public GameObject Player;

    private Vector3 StartingVector;

    // Start is called before the first frame update
    private void Awake()
    {
        InputMapping = new CameraInput();
    }

    private void Start()
    {
        StartingVector = Player.transform.position - this.transform.position;
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
        this.transform.position = Player.transform.position - StartingVector;
    }
}
