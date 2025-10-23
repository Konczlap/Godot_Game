using Godot;
using System;

public partial class MovementScript : Node
{
	public override void _PhysicsProcess(double delta)
	{
		var direction = Vector2.Zero;
		if (Input.IsActionPressed("move_right"))
		{
			direction.X += 1.0f;
		}
		if (Input.IsActionPressed("move_left"))
		{
			direction.X -= 1.0f;
		}
		if (Input.IsActionPressed("move_back"))
		{
			/*Notice how we are working with the vector's X and Z axes.
			In 3D, the XZ plane is the ground plane.*/
			direction.Y += 1.0f;
		}
		if (Input.IsActionPressed("move_forward"))
		{
			direction.Y -= 1.0f;
		}
	}
}
