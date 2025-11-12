using Godot;
using System;

public partial class Car : Node2D
{
	[Export] public PointLight2D FrontLeftLight;
	[Export] public PointLight2D FrontRightLight;
	[Export] public PointLight2D BackLeftLight;
	[Export] public PointLight2D BackRightLight;

	private bool lightsOn = false;

	public override void _Ready()
	{
		SetLights(false);
	}

	public override void _Process(double delta)
	{
		// Po naciśnięciu L przełączamy stan
		if (Input.IsActionJustPressed("toggle_lights"))
		{
			lightsOn = !lightsOn;
			SetLights(lightsOn);
		}
	}

	private void SetLights(bool enabled)
	{
		if (FrontLeftLight != null)
			FrontLeftLight.Visible = enabled;

		if (FrontRightLight != null)
			FrontRightLight.Visible = enabled;
			
		if (BackLeftLight != null)
			BackLeftLight.Visible = enabled;
			
		if (BackRightLight != null)
			BackRightLight.Visible = enabled;
	}
}
