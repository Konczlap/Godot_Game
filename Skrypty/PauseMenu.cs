using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	[Export]
	public NodePath ResumeButtonPath = "Panel/ResumeButton";

	private Button resumeButton;

	public override void _Ready()
	{
		Visible = false;

		// Aby menu działało podczas pauzy
		ProcessMode = ProcessModeEnum.Always;

		// Poprawne sprawdzenie czy NodePath jest pusty
		if (!string.IsNullOrEmpty(ResumeButtonPath.ToString()))
		{
			resumeButton = GetNodeOrNull<Button>(ResumeButtonPath);

			if (resumeButton != null)
			{
				resumeButton.Pressed += OnResumeButtonPressed;
			}
		}
	}

	private void OnResumeButtonPressed()
	{
		GetTree().Paused = false;
		Visible = false;
	}

	public void OnContinuePressed()
	{
		OnResumeButtonPressed();
	}
}
