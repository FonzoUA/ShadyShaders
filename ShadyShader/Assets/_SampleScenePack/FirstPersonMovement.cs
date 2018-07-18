using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public float mouseSensitivity = 1.0f;
    public GameObject player;

    private Transform PlayerBodyTransform;  

    private float xAxisClamp = 0.00f;
    private float mouseX;
    private float mouseY;
    private float rotAmountX;
    private float rotAmountY;

    private void Start()
    {
        PlayerBodyTransform = player.GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        RotateCamera();
        MoveBody();

        if (Input.GetKeyUp(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;
    }

    private void RotateCamera()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        xAxisClamp -= rotAmountY;

        rotAmountX = mouseX * mouseSensitivity;
        rotAmountY = mouseY * mouseSensitivity;

        Vector3 targetRot = transform.rotation.eulerAngles;

        targetRot.x -= rotAmountY;
        targetRot.z = 0;

        if (xAxisClamp > 50)
        {
            xAxisClamp = 50;
            targetRot.x = 50;
        }
        else if (xAxisClamp < -60)
        {
            xAxisClamp = -60;
            targetRot.x = 300;
        }

        transform.rotation = Quaternion.Euler(targetRot);

    }

    private void MoveBody()
    {
       
        Vector3 targetRotBody = PlayerBodyTransform.rotation.eulerAngles;


        targetRotBody.y += rotAmountX;

        PlayerBodyTransform.rotation = Quaternion.Euler(targetRotBody);


    }

}
