using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;

    public float speed;
    private float _inputX;
    private float _inputY;

    private Vector2 _moveInput;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        
    }

    private void Update()
    {
        PlayerControl();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void PlayerControl()
    {
        _inputX = Input.GetAxisRaw("Horizontal");
        _inputY = Input.GetAxisRaw("Vertical");
        // modify the speed if move diagonally
        if (_inputY != 0 && _inputX != 0)
        {
            _inputX *= 0.71f;
            _inputY *= 0.71f;
        }
        _moveInput = new Vector2(_inputX, _inputY);
    }

    private void Move()
    {
        _rb.MovePosition(_rb.position + _moveInput * speed * Time.deltaTime);
    }
}
