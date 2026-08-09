using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movePlayer : MonoBehaviour
{
    public float speed = 5f;
    private float rotationX, rotationY;
    private Rigidbody rb;
    private bool contactIsNormal = true;
    private float maxForce = 2000f;
    public float sensivityHor = 3;
    public float sensivityVert = 3;
    public float minVert = -60;
    public float maxVert = 60;


    [Header("Камера игрока")]
    public Camera playerCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnCollisionEnter(Collision temp)
    {
        if (temp.contacts[0].normal == Vector3.up || temp.gameObject.tag == "JumpIsAllowed")
        {
            contactIsNormal = true;
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(0, rb.velocity.y - 0.5f, 0);

        if (Input.GetKey(KeyCode.W))
        {
            movement += transform.forward * speed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            movement -= transform.forward * speed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            movement += transform.right * speed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            movement -= transform.right * speed;
        }

        Vector3 temp = new Vector3(movement.x, 0, movement.z).normalized * speed;
        rb.velocity = new Vector3(temp.x, movement.y, temp.z);

        if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButtonDown(1)) && contactIsNormal)
        {
            contactIsNormal = false;
            rb.AddForce(Vector3.up * maxForce, ForceMode.Impulse);
        }
    }

    void Update()
    {
        rotationX -= Input.GetAxis("Mouse Y") * sensivityVert;
        rotationX = Mathf.Clamp(rotationX, minVert, maxVert);
        rotationY += Input.GetAxis("Mouse X") * sensivityHor;

        transform.localEulerAngles = new Vector3(0, rotationY, 0);

        playerCamera.transform.localEulerAngles = new Vector3(rotationX, 0, 0);
    }


    //void FixedUpdate()
    //{
    //    float moveHorizontal = Input.GetAxis("Horizontal");
    //    float moveVertical = Input.GetAxis("Vertical");

    //    Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

    //    rb.MovePosition(rb.position + transform.TransformDirection(movement) * speed * Time.deltaTime);
    //}
}
