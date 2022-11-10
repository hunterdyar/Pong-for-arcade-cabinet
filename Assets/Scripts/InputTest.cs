using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    [SerializeField] private InputAction _action;
    [SerializeField] private string path;

    // Start is called before the first frame update
    void Start()
    {
        _action.Enable();
        _action.AddBinding(path);
    }

    // Update is called once per frame
    void Update()
    {
        if (_action.WasPerformedThisFrame())
        {
            Debug.Log(_action.ReadValueAsObject());
        }
    }
}
