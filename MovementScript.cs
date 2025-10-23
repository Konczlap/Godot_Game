using Godot;
using System;

public partial class MovementScript : Node2D
{
	[Export] public float MaxForwardSpeed = 300f;     // maksymalna prędkość do przodu (px/s)
	[Export] public float MaxBackwardSpeed = -150f;   // maksymalna prędkość do tyłu
	[Export] public float Acceleration = 400f;        // moc przyspieszenia
	[Export] public float BrakePower = 600f;          // moc hamowania
	[Export] public float EngineBraking = 200f;       // hamowanie silnikiem
	[Export] public float RotationSpeed = 90f;        // maksymalna prędkość obrotu (stopnie/sek)
	
	private float _currentSpeed = 0f;                 // aktualna prędkość
	private float _rotationInput = 0f;                // wejście dla skrętu

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;

		HandleInput(dt);
		MoveCar(dt);
	}

	private void HandleInput(float dt)
	{
		bool forward = Input.IsActionPressed("move_forward");
		bool backward = Input.IsActionPressed("move_backward");
		bool brake = Input.IsActionPressed("brake");

		_rotationInput = 0f;
		if (Input.IsActionPressed("turn_left")) _rotationInput -= 1f;
		if (Input.IsActionPressed("turn_right")) _rotationInput += 1f;

		// Jazda do przodu
		if (forward && _currentSpeed < MaxForwardSpeed && !brake)
		{
			_currentSpeed += Acceleration * dt;
		}
		// Jazda do tyłu
		else if (backward && _currentSpeed > MaxBackwardSpeed && !brake)
		{
			_currentSpeed -= Acceleration * dt;
		}
		// Hamowanie
		else if (brake)
		{
			if (_currentSpeed > 0)
				_currentSpeed -= BrakePower * dt;
			else if (_currentSpeed < 0)
				_currentSpeed += BrakePower * dt;
		}
		// Hamowanie silnikiem (gdy nic nie wciskasz)
		else
		{
			if (_currentSpeed > 0)
				_currentSpeed -= EngineBraking * dt;
			else if (_currentSpeed < 0)
				_currentSpeed += EngineBraking * dt;
		}

		// Utrzymuj zero prędkości (niech auto nie "pełznie")
		if (Mathf.Abs(_currentSpeed) < 1f)
			_currentSpeed = 0f;
	}

	private void MoveCar(float dt)
	{
		// Obrót tylko jeśli samochód się porusza
		if (Mathf.Abs(_currentSpeed) > 1f)
		{
			float rotationAmount = _rotationInput * RotationSpeed * dt * (_currentSpeed / MaxForwardSpeed);
			RotationDegrees += rotationAmount;
		}

		// Ruch w przód/tył w lokalnym układzie
		Vector2 forwardDir = new Vector2(0, -1).Rotated(Rotation); // lokalny kierunek przodu
		Position += forwardDir * _currentSpeed * dt;
	}
}
