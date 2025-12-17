// MovementScript.cs
using Godot;
using System;

public partial class MovementScript : CharacterBody2D
{
	[Export] private Collisions _collisions;
	
	public float MaxForwardSpeed = 300f;
	public float MaxBackwardSpeed = -150f;
	[Export] public float Acceleration = 400f;
	[Export] public float BrakePower = 600f;
	[Export] public float EngineBraking = 200f;
	[Export] public float RotationSpeed = 90f;
	[Export] private AudioStreamPlayer2D _engineDrive;
	[Export] private AudioStreamPlayer2D _brakeSound;
	[Export] private AudioStreamPlayer2D _refuelSound;
	
	private float _currentSpeed = 0f;
	private float _rotationInput = 0f;
	private VehicleManager _vehicleManager;
	
	public bool CanMove = true;
	private bool IsStanding = true;

	public override void _Ready()
	{
		_vehicleManager = VehicleManager.Instance;
		UpdateVehicleStats();
	}

	public void UpdateVehicleStats()
	{
		if (_vehicleManager == null) return;
		
		var vehicleData = _vehicleManager.GetActiveVehicleData();
		MaxForwardSpeed = vehicleData.MaxSpeed;
		MaxBackwardSpeed = -vehicleData.MaxSpeed / 2f;
		
		GD.Print($"🚗 Statystyki zaktualizowane:");
		GD.Print($"   MaxSpeed: {MaxForwardSpeed}");
		GD.Print($"   BackwardSpeed: {MaxBackwardSpeed}");
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;

		HandleInput(dt);
		MoveCar(dt);
		UpdateStandingState();
		_collisions.CheckCollisionStop(this);
		UpdateEngineSound();
		UpdateBrakeSound();
	}

	private void HandleInput(float dt)
	{
		if (!CanMove)
		{
			_currentSpeed = 0f;
			IsStanding = true;
			return;
		}
		
		bool forward = Input.IsActionPressed("move_forward");
		bool backward = Input.IsActionPressed("move_backward");
		bool brake = Input.IsActionPressed("brake");

		_rotationInput = 0f;
		if (Input.IsActionPressed("turn_left")) _rotationInput -= 1f;
		if (Input.IsActionPressed("turn_right")) _rotationInput += 1f;

		if (forward && _currentSpeed < MaxForwardSpeed && !brake)
			_currentSpeed += Acceleration * dt;
		else if (backward && _currentSpeed > MaxBackwardSpeed && !brake)
			_currentSpeed -= Acceleration * dt;
		else if (brake)
		{
			if (_currentSpeed > 0)
				_currentSpeed -= BrakePower * dt;
			else if (_currentSpeed < 0)
				_currentSpeed += BrakePower * dt;
		}
		else
		{
			if (_currentSpeed > 0)
				_currentSpeed -= EngineBraking * dt;
			else if (_currentSpeed < 0)
				_currentSpeed += EngineBraking * dt;
		}

		if (Mathf.Abs(_currentSpeed) < 1f)
			_currentSpeed = 0f;
	}

	private void MoveCar(float dt)
	{
		if (Mathf.Abs(_currentSpeed) > 1f)
		{
			float rotationAmount = _rotationInput * RotationSpeed * dt * (_currentSpeed / MaxForwardSpeed);
			RotationDegrees += rotationAmount;
		}

		Vector2 forwardDir = new Vector2(0, -1).Rotated(Rotation);
		Velocity = forwardDir * _currentSpeed;
		MoveAndSlide();
	}
	
	private void UpdateStandingState()
	{
		if (_currentSpeed < 0.5 && _currentSpeed >= 0 || _currentSpeed > -0.5 && _currentSpeed <= 0)
			IsStanding = true;
		else
			IsStanding = false;
	}
	
	private void UpdateEngineSound()
	{
		float absSpeed = Mathf.Abs(_currentSpeed);
		float minVolume = -25f;
		float maxVolume = 0f;
		float fadeSpeed = 20f;

		if (absSpeed > 5f)
		{
			if (!_engineDrive.Playing)
				_engineDrive.Play();

			float speedRatio = Mathf.Clamp(absSpeed / MaxForwardSpeed, 0f, 1f);
			float targetVolume = Mathf.Lerp(minVolume, maxVolume, speedRatio);

			_engineDrive.VolumeDb = Mathf.MoveToward(
				_engineDrive.VolumeDb,
				targetVolume,
				fadeSpeed * (float)GetPhysicsProcessDeltaTime()
			);

			_engineDrive.PitchScale = Mathf.Lerp(0.9f, 1.3f, speedRatio);
		}
		else
		{
			_engineDrive.VolumeDb = Mathf.MoveToward(
				_engineDrive.VolumeDb,
				minVolume,
				fadeSpeed * (float)GetPhysicsProcessDeltaTime()
			);

			if (_engineDrive.VolumeDb <= minVolume + 1f)
			{
				_engineDrive.Stop();
			}
		}
	}
	
	private void UpdateBrakeSound()
	{
		bool isBraking = Input.IsActionPressed("brake");
		float absSpeed = Mathf.Abs(_currentSpeed);

		if (isBraking && absSpeed > 30f)
		{
			if (!_brakeSound.Playing)
				_brakeSound.Play();
		}
		else
		{
			if (_brakeSound.Playing)
				_brakeSound.Stop();
		}
	}

	public void SetCurrentSpeed(float speed)
	{
		_currentSpeed = speed;
	}
	 
	public bool GetIsStanding()
	{
		return IsStanding;
	}
	
	public void StartRefuel()
	{
		if (!_refuelSound.Playing)
			_refuelSound.Play();
	}

	public void StopRefuel()
	{
		if (_refuelSound.Playing)
			_refuelSound.Stop();
	}
}
