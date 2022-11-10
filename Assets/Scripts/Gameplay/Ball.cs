using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pong
{
    public class Ball : MonoBehaviour
    {
        public Action<PlayerData> OnLastPaddleTouchedChanged;
        
        [SerializeField] private float _maxSpeed;
        [SerializeField] private bool _destroyIfNotMoving;
        [SerializeField] private float _destroyIfNotMovingTime;
        private const float NotMovingVelocityThreshold = 0.01f;
        
        private float _sqrMaxSpeed => _maxSpeed * _maxSpeed;
        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;
        private bool _replaceWhenDestroyed = true;
        private PongGameManager _gameManager;
        private float _timeSpentNotMoving = 0;
        private bool _hasBeenLaunched;
        private PlayerData _lastPaddleTouched;
        // Start is called before the first frame update
        private void Awake()
        {
            _timeSpentNotMoving = 0;
            _hasBeenLaunched = false;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;//Let other balls knock off it it in the center, but it shouldn't move.
        }

        private void FixedUpdate()
        {
            float velocitySqrMagnitude = _rigidbody2D.velocity.sqrMagnitude;
            //set max speed
            if (velocitySqrMagnitude > _sqrMaxSpeed)
            {
                _rigidbody2D.velocity = _rigidbody2D.velocity.normalized * _maxSpeed;
                _timeSpentNotMoving = 0;
            }else if (velocitySqrMagnitude < NotMovingVelocityThreshold)
            {
                _timeSpentNotMoving += Time.fixedDeltaTime;
                if (_timeSpentNotMoving > _destroyIfNotMovingTime)
                {
                    if (_hasBeenLaunched && _destroyIfNotMoving)
                    {
                        _gameManager.OnBallRemovedFromPlay(_replaceWhenDestroyed);
                        RemoveBall();
                    }
                }
            }
            else
            {
                _timeSpentNotMoving = 0;
            }
        }

        public void SetPongGameManager(PongGameManager manager)
        {
            _gameManager = manager;
        }
        /// <summary>
        /// Fire the ball in a random direction.
        /// Property is speed, default is 60% of whatever maxSpeed is
        /// </summary>
        public void FireBall(float delay = 0)
        {
            float launchSpeed = _maxSpeed * 0.6f;
            if (delay <= 0)
            {
                LaunchBall(launchSpeed);
            }
            else
            {
                StartCoroutine(DoFireBallAfterDelay(launchSpeed, delay));
            }
        }
        public IEnumerator DoFireBallAfterDelay(float speed, float delay)
        {
            yield return new WaitForSeconds(delay);
            LaunchBall(speed);
        }

        public void LaunchBallInDirection(Vector2 ballFireDirection, float speed)
        {
            _rigidbody2D.constraints = RigidbodyConstraints2D.None;
            var dir = ballFireDirection;
            _rigidbody2D.velocity = dir * speed;
            _hasBeenLaunched = true;
        }
        private void LaunchBall(float speed)
        {
            _rigidbody2D.constraints = RigidbodyConstraints2D.None;
            var dir = GetRandomDirectionNotStraight();
            _rigidbody2D.velocity = dir * speed;
            _hasBeenLaunched = true;
        }

        public static Vector2 GetRandomDirectionNotStraight()
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            //Cross magnitude will be 1 when vectors are perp and 1 when they are in the same direction (-1 in opposite direction, but we are taking magnitude so that's perfect)
            //We want this random direction to NOT be close up or right
            // = we want hte cross product of this random direction and one (and right) to not be close to 0

            float rightCross = Vector3.Cross(randomDir, Vector2.right).magnitude;
            float upCross = Vector3.Cross(randomDir, Vector2.up).magnitude;
       
            if (rightCross > 0.333f && upCross > 0.01f)
            {
                return randomDir;
            }
            else
            {
                //Try again! Warning, if there's a bug with the math, this will create an infinite loop.
                return GetRandomDirectionNotStraight();
            }
        }
        public void BallEnteredScoreZone()
        {
            _gameManager.OnBallRemovedFromPlay(_replaceWhenDestroyed);
            RemoveBall();
        }

        public void SetPlayerLastHit(PlayerData player)
        {
            if (player != _lastPaddleTouched)
            {
                _lastPaddleTouched = player;
                _spriteRenderer.color = player.Color;
                OnLastPaddleTouchedChanged?.Invoke(player);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Paddle"))
            {
                var paddle = other.gameObject.GetComponent<Paddle>();
                if (paddle != null)
                {
                    SetPlayerLastHit(paddle.Player);
                }
            }
        }

        public void RemoveBall()
        {
            Destroy(gameObject);
        }


        public PlayerData GetLastPlayer()
        {
            return _lastPaddleTouched;
        }
    }
}