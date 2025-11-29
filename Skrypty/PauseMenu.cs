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

		ProcessMode = ProcessModeEnum.Always;

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
