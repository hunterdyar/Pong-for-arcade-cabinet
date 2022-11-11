using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pong
{
    public class PaddleMovement : MonoBehaviour
    {
        [Header("Config")] [SerializeField] private float baseSpeed;
        private Rigidbody2D _rigidbody2D;
        private BoxCollider2D _box;
        private PaddleInput _input;
        private Vector2 _desiredVelocity;
        public Vector2 DesiredVelocity => _desiredVelocity;
        private Bounds _worldBounds;
        private bool _inputActive = true;
        private float _calculatedRealMaxSpeed => CalculateSpeed(); //Speed after various calculations, powerups, etc.
        private float _currentSpeed;
        
        [SerializeField] private bool _isConfused;
        public bool IsConfused { get => _isConfused; set => _isConfused = value; }
        
        public float CalculateSpeed()
        {
            return baseSpeed; //modifiers!
        }

        // public event Action<InputAction.CallbackContext> onActionTriggered
        private void Awake()
        {
            _box = GetComponentInChildren<BoxCollider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            
        }

        private void OnEnable()
        {
            PongGameManager.OnGameStateChange += OnGameStateChange;
            
        }

        private void OnDisable()
        {
            PongGameManager.OnGameStateChange -= OnGameStateChange;

        }

        private void OnGameStateChange(GameState state)
        {
            if (state == GameState.Gameplay)
            {
                ActivateInput();
            }
            else
            {
                DeactivateInput();
            }
        }


        private void Start()
        {
            _worldBounds = Utility.GetXYScreenBoundsInWorldSpace();
        }


        public void Move(float value)
        {
            if (_inputActive)
            {
                if (_isConfused)
                {
                    _desiredVelocity = new Vector2(-value * _calculatedRealMaxSpeed, 0);
                }
                else
                {
                    _desiredVelocity = new Vector2(value * _calculatedRealMaxSpeed, 0);
                }
            }
        }

        public void ActivateInput()
        {
            _inputActive = true;
        }

        public void DeactivateInput()
        {
            _inputActive = false;
        }

        public float GetCurrentSpeed()
        {
            return _currentSpeed;
        }

        private void FixedUpdate()
        {
            if (!_inputActive)
            {
                return;
            }

            Vector3 testPoint;
            
            //get a point on the right or left edge of the box, depending on what direction we are moving
            if (_desiredVelocity.x > 0)
            {
                testPoint = _box.bounds.center + _box.bounds.extents;
            }
            else if (_desiredVelocity.x < 0)
            {
                testPoint = _box.bounds.center - _box.bounds.extents;
            }
            else
            {
                _currentSpeed = 0;
                //move zero!
                return;
            }

            //This will let the player move up to 1 tick out of bounds.
            //I prefer that then getting almost to the bounds and not going over
            //because you notice the sliver.

            //test if our point is still inside of the screen
            //we can't use _worldBounds.ContainsPoint because z positions for the worldBounds aren't set correctly.
            if (testPoint.x < _worldBounds.max.x && testPoint.x > _worldBounds.min.x)
            {
                _currentSpeed = _desiredVelocity.x;
                _rigidbody2D.MovePosition(_rigidbody2D.position + _desiredVelocity * Time.fixedDeltaTime);
            }
            else
            {
                _currentSpeed = 0;
            }
        }
    }
}