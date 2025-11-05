using Godot;
using System;

public partial class MovementScript : Node2D
{
	[Export] public float MaxForwardSpeed = 300f;     // (75) maksymalna prędkość do przodu (px/s)
	[Export] public float MaxBackwardSpeed = -150f;   // (-37.5) maksymalna prędkość do tyłu
	[Export] public float Acceleration = 400f;        // (150) moc przyspieszenia
	[Export] public float BrakePower = 600f;          // (225) moc hamowania
	[Export] public float EngineBraking = 200f;       // (10) hamowanie silnikiem
	[Export] public float RotationSpeed = 90f;        // (90) maksymalna prędkość obrotu (stopnie/sek)
	
	private float _currentSpeed = 0f;                 // aktualna prędkość
	private float _rotationInput = 0f;                // wejście dla skrętu
	
	private bool IsStanding = true;

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;

		HandleInput(dt);
		MoveCar(dt);
		UpdateStandingState(); // czy auto stoi?
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
			_currentSpeed += Acceleration * dt;
		// Jazda do tyłu
		else if (backward && _currentSpeed > MaxBackwardSpeed && !brake)
			_currentSpeed -= Acceleration * dt;
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
	
	private void UpdateStandingState()
	{
		if (_currentSpeed < 0.5 && _currentSpeed >= 0 || _currentSpeed > -0.5 && _currentSpeed <= 0)
			IsStanding = true;
		else
			IsStanding = false;
	}
	
	public bool GetIsStanding()
	{
		return IsStanding;
	}
}
