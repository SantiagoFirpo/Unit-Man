using System;
using UnitMan.Source.Config;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnitMan.Source.UI
{
    public class MazeEditorController : MonoBehaviour
    {
        private InputMaps _inputMap;
        private Vector2 _mousePosition;

        [SerializeField]
        private Tilemap wallTilemap;
        
        private Vector3Int _wallOrigin;

        private void Awake()
        {
            _inputMap = new InputMaps();
            _inputMap.Enable();
            _inputMap.UI.Point.performed += OnMouseMove;
            _inputMap.UI.Click.performed += OnClick;
        }

        private void OnDisable()
        {
            _inputMap.UI.Point.performed -= OnMouseMove;
            _inputMap.UI.Click.performed -= OnClick;
        }

        private void OnMouseMove(InputAction.CallbackContext context)
        {
            _mousePosition = context.ReadValue<Vector2>();
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            
        }
    }
}
