using Godot;
using System;

public partial class SpeedHUD : CanvasLayer
{
	[Export] public MovementScript CarScript;  // przypnij node samochodu

	private Label _speedLabel;

	public override void _Ready()
	{
		_speedLabel = GetNode<Label>("SpeedHUDControl/SpeedLabel");

		if (CarScript == null)
			GD.PushWarning("SpeedHUD: CarScript nie podłączony w Inspectorze!");
	}

	public override void _Process(double delta)
	{
		if (CarScript == null) return;

		// Pobieramy prędkość pojazdu
		float speed = Math.Abs(CarScript.Velocity.Length()); // px/s
		int displaySpeed = (int)speed; // zaokrąglamy do liczby całkowitej

		// Wyświetlamy w Labelu
		_speedLabel.Text = $"{displaySpeed} km/h";
	}
}
