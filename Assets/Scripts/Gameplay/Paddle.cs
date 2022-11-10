using System;
using Pong.Powerups;
using UnityEngine;

namespace Pong
{
	/// <summary>
	/// Connects the gameObject to the data.
	/// </summary>
	public class Paddle : MonoBehaviour
	{
		public PlayerData Player => _playerData;
		[SerializeField] private PlayerData _playerData;

		[SerializeField] private float frictionExtraModifier;
		[SerializeField] private float paddleEdgeMaxAngle;

		private Vector3 _startPosition;
		public PaddleMovement PaddleMovement => _paddleMovement;
		public Vector2 CurrentVelocity => _rigidbody2D.velocity;

		private PaddleMovement _paddleMovement;

		private Rigidbody2D _rigidbody2D;
		private SpriteRenderer _spriteRenderer;
		private BoxCollider2D _collider; 
		
		private Ball _lastBallHit;

		private void Awake()
		{
			_startPosition = transform.position;
			_collider = GetComponentInChildren<BoxCollider2D>();
			_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
			_rigidbody2D = GetComponent<Rigidbody2D>();
			_paddleMovement = GetComponent<PaddleMovement>();
			_playerData.RegisterPaddle(this);
		}

		private void Start()
		{
			_spriteRenderer.color = _playerData.Color;
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
			if (state == GameState.Countdown)
			{
				ResetPosition();
			}
		}

		public void ResetPosition()
		{
			transform.position = _startPosition;
		}

		public void OnCollisionEnter2D(Collision2D other)
		{
			//todo: this needs to be completely rewritten.
			//
			
			//if ball
			if (other.gameObject.CompareTag("Ball"))
			{
				//track the last ball we hit.
				var ball = other.rigidbody.GetComponent<Ball>();
				if (ball != null)
				{
					_lastBallHit = ball;
				}
				//Add a bit of extra force from the "friction" of our movement
				var ballRB = other.rigidbody;
				// ballRB.AddForce(_rigidbody2D.velocity * frictionExtraModifier);
				
				var contact = other.GetContact(0);
				var paddleOffsetDir = (Vector3)contact.point - ((Vector3)_rigidbody2D.centerOfMass + transform.position);
				Debug.DrawLine(((Vector3)_rigidbody2D.centerOfMass + transform.position),contact.point,Color.red,1f);

				if (Mathf.Abs(contact.normal.x) > Mathf.Epsilon)
				{
					//dont do anything when we hit the side of the paddle.
					return;
				}
				// ballRB.AddForce(paddleOffsetDir.normalized*directionExtraModifier);
				
				//Rotate angle depending on position of block
				var originalVel = new Vector2(contact.rigidbody.velocity.x, contact.rigidbody.velocity.y);
				float paddleFactor = paddleOffsetDir.x/(_collider.bounds.size.x/2);//signed
				float sign = Mathf.Sign(paddleOffsetDir.x);
				float currentAngle = Vector2.Angle(Vector2.up, originalVel);
				float maxRotation = (currentAngle + paddleEdgeMaxAngle) < 85 ? (paddleEdgeMaxAngle) : 85 - currentAngle;
				float rotationAngle = Mathf.Abs(paddleFactor) * maxRotation;
				rotationAngle = -rotationAngle * sign;
				Vector2 newVel = originalVel.Rotate(rotationAngle);

				//Add force depending on 'friction'. THis speeds the balls up over time, regular bouncing doesnt.
				float signedSpeedFactor = _paddleMovement.GetCurrentSpeed()/_paddleMovement.CalculateSpeed();
				var force = Vector2.right * signedSpeedFactor * frictionExtraModifier;
				
				ballRB.velocity = newVel;
				
				//add force after changing velocity :p
				ballRB.AddForce(force,ForceMode2D.Impulse);	
			}
		}
	}
}