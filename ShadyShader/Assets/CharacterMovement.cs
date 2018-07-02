using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    public float speed = 10.0f;

    private float horizInput;
    private float vertInput;

    private CharacterController controller;

    private void Start()
    {
        controller = this.GetComponent<CharacterController>();
    }
    void Update ()
    {
        horizInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");


        Vector3 moveDirSide = transform.right * horizInput * speed;
        Vector3 moveDirForward = transform.forward * vertInput * speed;

        controller.SimpleMove(moveDirSide);
        controller.SimpleMove(moveDirForward);

    }
}
