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
    //tool animation control
    private float _mouseX;
    private float _mouseY;
    private bool _useTool;

    //public string GUID => GetComponent<DataGUID>().guid;
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

    private void OnMouseClickedEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        //first play the animation then execute the event
        if (_useTool)
            return;

        if (itemDetails.itemType != ItemType.Seed && itemDetails.itemType != ItemType.Commodity && itemDetails.itemType != ItemType.Furniture)
        {
            _mouseX = mouseWorldPos.x - transform.position.x;
            //_mouseY = mouseWorldPos.y - (transform.position.y + 0.85f);
            _mouseY = mouseWorldPos.y - transform.position.y;

            if (Mathf.Abs(_mouseX) > Mathf.Abs(_mouseY))
                _mouseY = 0;
            else
                _mouseX = 0;

            StartCoroutine(UseToolRoutine(mouseWorldPos, itemDetails));
        }
        else
        {
            EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        }
    }
    private IEnumerator UseToolRoutine(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        _useTool = true;
        _isDisabled = true;
        yield return null;
        foreach (var anim in _animators)
        {
            anim.SetTrigger("UseTool");
            //人物的面朝方向
            anim.SetFloat("InputX", _mouseX);
            anim.SetFloat("InputY", _mouseY);
        }
        yield return new WaitForSeconds(0.45f);
        EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        yield return new WaitForSeconds(0.25f);
        //等待动画结束
        _useTool = false;
        _isDisabled = false;
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
            anim.SetFloat("MouseX", _mouseX);
            anim.SetFloat("MouseY", _mouseY);
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
