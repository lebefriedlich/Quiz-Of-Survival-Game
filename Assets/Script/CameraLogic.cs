using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    public Transform Player;
    public Transform ViewPoint;
    public float RotationSpeed;
    public GameObject TPSCamera;
    bool TPSMode = true;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void Update()
    {
        // Get the input axes for horizontal and vertical movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the viewing direction based on the player and camera positions
        Vector3 viewDir = Player.position - new Vector3(transform.position.x, Player.position.y, transform.position.z);
        ViewPoint.forward = viewDir.normalized;

        // If in third-person mode (TPSMode)
        if (TPSMode)
        {
            // Calculate input direction based on camera orientation and player input
            Vector3 InputDir = ViewPoint.forward * verticalInput + ViewPoint.right * horizontalInput;

            // If the input direction is not zero, smoothly rotate the player to face the direction of movement
            if (InputDir != Vector3.zero)
            {
                Player.forward = Vector3.Slerp(Player.forward, InputDir.normalized, Time.deltaTime * RotationSpeed);
            }
        }
    }

    public void CameraModeChanger(bool TPS)
    {
        TPSMode = TPS;

        // Toggle the cameras based on the selected mode
        if (TPS)
        {
            TPSCamera.SetActive(true);
        }
    }
}