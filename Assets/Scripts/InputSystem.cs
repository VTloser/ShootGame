using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputSystem : MonoBehaviour
{
    private static InputSystem instance;

    public UnityAction _MouseButtonDown;
    public UnityAction _MouseButton;
    public UnityAction _MouseButtonUp;

    public UnityAction _LeftShiftDown;
    public UnityAction _LeftShiftUp;

    public Vector2 _Input;

    public static InputSystem Instance
    {
        get
        {
            if (instance == null)
                instance = new GameObject("InputSystem").AddComponent<InputSystem>();
            return instance;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _MouseButtonDown?.Invoke();
        }
        if (Input.GetMouseButton(1))
        {
            _MouseButton?.Invoke();
        }
        if (Input.GetMouseButtonUp(1))
        {
            _MouseButtonUp?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _LeftShiftDown?.Invoke();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _LeftShiftUp?.Invoke();
        }

        _Input = new Vector2(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"));
    }
}
