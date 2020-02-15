using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;
    private float horz;
    private float vert;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

    }
    private void Update()
    {
        if (!rb)
            return;
        horz = Input.GetAxis("Horizontal") * speed;
        vert = Input.GetAxis("Vertical") * speed;
    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector3(horz * Time.fixedDeltaTime, rb.velocity.y, vert * Time.fixedDeltaTime);
    }
}
