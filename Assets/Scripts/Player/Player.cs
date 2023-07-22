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
    private Animator[] _animators;
    private bool _isMoving;
    private bool _isRunning = true;
    private bool _isDisabled;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animators = GetComponentsInChildren<Animator>();
    }

    private void Update()
    {
        SwitchMoveMode();
        if (!_isDisabled)
        {
            PlayerControl();
        }
        else
        {
            _isMoving = false;
        }
        SwitchAnimation();
    }

    private void FixedUpdate()
    {
        if (!_isDisabled)
        {
            Move();
        }
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.MoveToPosition -= OnMoveToPosition;
        EventHandler.MouseClickedEvent -= OnMouseClickedEvent;
    }

    private void OnMouseClickedEvent(Vector3 arg1, ItemDetails arg2)
    {
        //first play the animation then execute the event
        //player animation
        EventHandler.CallExecuteActionAfterAnimation(arg1, arg2);
    }

    private void OnMoveToPosition(Vector3 obj)
    {
        transform.position = obj;
    }

    private void OnAfterSceneLoadedEvent()
    {
        _isDisabled = false;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        _isDisabled = true;
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

        if (!_isRunning)
        {
            _inputX *= 0.5f;
            _inputY *= 0.5f;
        }
        _moveInput = new Vector2(_inputX, _inputY);
        _isMoving = _moveInput != Vector2.zero;
    }

    private void Move()
    {
        _rb.MovePosition(_rb.position + _moveInput * speed * Time.deltaTime);
    }

    private void SwitchAnimation()
    {
        foreach (var anim in _animators)
        {
            anim.SetBool("IsMoving", _isMoving);
            if (_isMoving)
            {
                anim.SetFloat("InputX", _inputX);
                anim.SetFloat("InputY", _inputY);
            }
        }
    }

    private void SwitchMoveMode()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            _isRunning = !_isRunning;
        }
    }
}
