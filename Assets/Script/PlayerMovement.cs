using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    
    private Rigidbody2D _rb;
    private Vector2 _moveVector;

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _moveVector.x = Input.GetAxis("Horizontal");
        _moveVector.y = Input.GetAxis("Vertical");
        _rb.MovePosition(_rb.position + _moveVector * _speed * Time.deltaTime);
    }
}
