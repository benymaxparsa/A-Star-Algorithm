using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 2;
    [SerializeField] private float zoomSpeed = 10;   
    private float _horz;
    private float _vert;
    private float _zoom;
    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        _horz = Input.GetAxis("Horizontal") * speed;
        _vert = Input.GetAxis("Vertical") * speed;
        _zoom = Input.GetAxis("Mouse ScrollWheel")*zoomSpeed;
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize+_zoom,10,200);
        transform.Translate(_horz,_vert,0);
    }
}
