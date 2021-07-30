using System;
using UnitMan.Source.Config;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnitMan.Source.UI
{
    public class MazeEditorController : MonoBehaviour
    {
        private InputMaps _inputMap;

        private void Awake()
        {
            _inputMap = new InputMaps();
            _inputMap.Enable();
            _inputMap.UI.Click.performed += OnClick;
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            Debug.Log("hi");
            Debug.Log(context.ReadValue<Vector2>());
        }
    }
}
