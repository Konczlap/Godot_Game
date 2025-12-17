using Godot;
using System;

public partial class PauseController : Node
{
	[Export] public PauseMenu PauseMenu;
	[Export] public CanvasLayer GameMinimap;

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			TogglePause();
		}
	}

	public void TogglePause()
	{
		bool isCurrentlyPaused = GetTree().Paused;
		bool newPauseState = !isCurrentlyPaused;

		GetTree().Paused = newPauseState;

		if (PauseMenu != null)
		{
			PauseMenu.Visible = newPauseState;
		}

		if (GameMinimap == null)
		{
			GameMinimap = GetTree().Root.FindChild("Minimap", true, false) as CanvasLayer;
		}

		if (GameMinimap != null)
		{
			GameMinimap.Visible = !newPauseState;
		}
	}
}
