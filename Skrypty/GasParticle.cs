using Godot;
using System;

public partial class GasParticle : Node2D
{
	[Export] public MovementScript Movement;
	[Export] public NodePath SmokePath;

	[Export] public float NormalLifetime = 0.8f;
	[Export] public float StopLifetime = 0.01f;

	private Node _smoke;

	public override void _Ready()
	{
		if (SmokePath != null && HasNode(SmokePath))
			_smoke = GetNode(SmokePath);
		else
			_smoke = GetNode("Smoke");

		if (_smoke != null)
		{
			_smoke.Set("emitting", false);
			_smoke.Set("visible", false);
		}
	}

	public override void _Process(double delta)
	{
		if (Movement == null || _smoke == null)
			return;

		bool moving = !Movement.GetIsStanding();

		if (moving)
		{
			_smoke.Set("lifetime", NormalLifetime);
			_smoke.Set("emitting", true);
			_smoke.Set("visible", true);
		}
		else
		{
			_smoke.Set("lifetime", StopLifetime);
			_smoke.Set("emitting", false);
			_smoke.Set("visible", false);
		}
	}
}
