using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Rigidbody _rb;
    private float _horz;
    private float _vert;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

    }
    private void Update()
    {
        if (!_rb)
            return;
        _horz = Input.GetAxis("Horizontal") * speed;
        _vert = Input.GetAxis("Vertical") * speed;
    }
    private void FixedUpdate()
    {
        _rb.velocity = new Vector3(_horz * Time.fixedDeltaTime, _rb.velocity.y, _vert * Time.fixedDeltaTime);
    }
}
