using Godot;
using System;

public partial class PauseController : Node
{
	[Export] public PauseMenu PauseMenu;

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			TogglePause();
		}
	}

	private void TogglePause()
	{
		bool paused = GetTree().Paused;

		GetTree().Paused = !paused;
		PauseMenu.Visible = !paused;
	}
}
