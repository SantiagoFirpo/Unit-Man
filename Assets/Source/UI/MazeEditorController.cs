using System;
using UnitMan.Source.Config;
using UnitMan.Source.Utilities.Pathfinding;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace UnitMan.Source.UI
{
    public class MazeEditorController : MonoBehaviour
    {
        private Maze _currentWorkingMaze;
        private InputMaps _inputMap;
        private Vector2 _mousePosition;

        [SerializeField]
        private Tilemap _wallTilemap;

        [SerializeField]
        private TileBase wallRuleTile;
        
        private Vector3Int _wallOrigin;
        private Camera _mainCamera;
        private bool _isClicking = false;

        private void Awake()
        {
            _inputMap = new InputMaps();
            _inputMap.Enable();
            _inputMap.UI.Point.performed += OnMouseMove;
            _inputMap.UI.Click.performed += OnClickUpdated;
        }

        private void Start()
        {
            _wallOrigin = _wallTilemap.origin;
            _mainCamera = Camera.main;
            // wallRuleTile = _wallTilemap.GetTile(Vector3Int.zero);
        }

        private void OnDisable()
        {
            _inputMap.UI.Point.performed -= OnMouseMove;
            _inputMap.UI.Click.performed -= OnClickUpdated;
        }

        private void OnMouseMove(InputAction.CallbackContext context)
        {
            _mousePosition = context.ReadValue<Vector2>();
        }

        private void Update()
        {
            Vector3Int mousePositionOnWallTileset = LevelGridController.Vector2ToVector3Int
                                                                        (_mainCamera.ScreenToWorldPoint
                                                                        (_mousePosition))  - _wallOrigin;
            if (!_isClicking || _wallTilemap.GetTile(mousePositionOnWallTileset) == wallRuleTile) return;
            _wallTilemap.SetTile(mousePositionOnWallTileset, wallRuleTile);
        }

        private void OnClickUpdated(InputAction.CallbackContext context)
        {
            _isClicking = !_isClicking;
        }
    }
    
    
    
}
