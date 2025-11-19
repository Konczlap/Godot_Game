using Godot;
using System;

public partial class TimeHUD : CanvasLayer
{
	[Export] public DayNightCycle TimeSource; // podłącz w Inspectorze swój node DayNightCycle

	private Label _timeLabel;

	public override void _Ready()
	{
		_timeLabel = GetNode<Label>("TimeHUDControl/TimeLabel");

		if (TimeSource == null)
			GD.PushWarning("TimeHUD: TimeSource (DayNightCycle) nie podłączony w Inspectorze.");
	}

	public override void _Process(double delta)
	{
		if (TimeSource == null) return;

		int hour = TimeSource.GetTimeHour();
		int minute = TimeSource.GetTimeMinute();

		// Formatuj zawsze dwucyfrowo, np. 06:05
		string formatted = $"{hour:00}:{minute:00}";
		_timeLabel.Text = formatted;
	}
}
