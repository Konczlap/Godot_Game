// Car.cs
using Godot;
using System;

public partial class Car : Sprite2D  // ← ZMIENIONE z Node2D na Sprite2D
{
	[Export] public PointLight2D FrontLeftLight;
	[Export] public PointLight2D FrontRightLight;
	[Export] public PointLight2D BackLeftLight;
	[Export] public PointLight2D BackRightLight;
	[Export] public PointLight2D BackMiddleLight;
	
	private bool lightsOn = false;
	private bool spacePressed = false;
	private VehicleManager _vehicleManager;
	
	public override void _Ready()
	{
		_vehicleManager = VehicleManager.Instance;
		UpdateCarAppearance();
		UpdateLights();
	}
	
	public void UpdateCarAppearance()
	{
		if (_vehicleManager == null) return;
		
		var vehicleData = _vehicleManager.GetActiveVehicleData();
		Texture = GD.Load<Texture2D>(vehicleData.TexturePath);  // ← this.Texture zamiast _carSprite.Texture
		
		GD.Print($"✅ Pojazd zmieniony na: {vehicleData.Name}");
		GD.Print($"   Tekstura: {vehicleData.TexturePath}");
	}
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("toggle_lights"))
		{
			lightsOn = !lightsOn;
			UpdateLights();
		}
		
		if (Input.IsActionPressed("brake") && !spacePressed)
		{
			spacePressed = true;
			UpdateBrakeLights();
		}
		
		if (!Input.IsActionPressed("brake") && spacePressed)
		{
			spacePressed = false;
			UpdateBrakeLights();
		}
	}
	
	private void UpdateLights()
	{
		if (FrontLeftLight != null) FrontLeftLight.Visible = lightsOn;
		if (FrontRightLight != null) FrontRightLight.Visible = lightsOn;
		if (BackLeftLight != null) BackLeftLight.Visible = lightsOn;
		if (BackRightLight != null) BackRightLight.Visible = lightsOn;
		if (BackMiddleLight != null) BackMiddleLight.Visible = spacePressed && lightsOn;
	}
	
	private void UpdateBrakeLights()
	{
		if (lightsOn)
		{
			if (BackMiddleLight != null) BackMiddleLight.Visible = spacePressed;
		}
		else
		{
			if (BackLeftLight != null) BackLeftLight.Visible = spacePressed;
			if (BackRightLight != null) BackRightLight.Visible = spacePressed;
			if (BackMiddleLight != null) BackMiddleLight.Visible = spacePressed;
		}
	}
}
