using Godot;
using System;

public partial class Car : Node2D
{
	[Export] public PointLight2D FrontLeftLight;
	[Export] public PointLight2D FrontRightLight;
	[Export] public PointLight2D BackLeftLight;
	[Export] public PointLight2D BackRightLight;
	[Export] public PointLight2D BackMiddleLight;

	private bool lightsOn = false; // F
	private bool spacePressed = false; // Space

	public override void _Ready()
	{
		UpdateLights();
	}

	public override void _Process(double delta)
	{
		// Toggle F
		if (Input.IsActionJustPressed("toggle_lights"))
		{
			lightsOn = !lightsOn;
			UpdateLights();
		}

		// Space wciśnięte
		if (Input.IsActionPressed("brake") && !spacePressed)
		{
			spacePressed = true;
			UpdateBrakeLights();
		}
		// Space puszczone
		if (!Input.IsActionPressed("brake") && spacePressed)
		{
			spacePressed = false;
			UpdateBrakeLights();
		}
	}

	private void UpdateLights()
	{
		// Przednie światła
		if (FrontLeftLight != null) FrontLeftLight.Visible = lightsOn;
		if (FrontRightLight != null) FrontRightLight.Visible = lightsOn;

		// Tylne boczne
		if (BackLeftLight != null) BackLeftLight.Visible = lightsOn;
		if (BackRightLight != null) BackRightLight.Visible = lightsOn;

		// Środkowe tylne zależne od Space
		if (BackMiddleLight != null) BackMiddleLight.Visible = spacePressed && lightsOn;
	}

	private void UpdateBrakeLights()
	{
		if (lightsOn)
		{
			// F włączone → Space włącza tylko środkowe tylne
			if (BackMiddleLight != null) BackMiddleLight.Visible = spacePressed;
		}
		else
		{
			// F wyłączone → Space włącza wszystkie tylne
			if (BackLeftLight != null) BackLeftLight.Visible = spacePressed;
			if (BackRightLight != null) BackRightLight.Visible = spacePressed;
			if (BackMiddleLight != null) BackMiddleLight.Visible = spacePressed;
		}
	}
}
