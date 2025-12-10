using Godot;
using System;

public partial class TimeHUD : CanvasLayer
{
	[Export] public DayNightCycle TimeSource;

	private Label _timeLabel;
	private Label _dayLabel;   // <-- NOWE

	public override void _Ready()
	{
		_timeLabel = GetNode<Label>("TimeHUDControl/TimeLabel");
		_dayLabel = GetNode<Label>("TimeHUDControl/DayLabel"); // <-- NOWE

		if (TimeSource == null)
			GD.PushWarning("TimeHUD: TimeSource (DayNightCycle) nie podłączony w Inspectorze.");
	}

	public override void _Process(double delta)
	{
		if (TimeSource == null) 
			return;

		// --- ZEGAR ---
		int hour = TimeSource.GetTimeHour();
		int minute = TimeSource.GetTimeMinute();
		_timeLabel.Text = $"{hour:00}:{minute:00}";

		// --- DZIEŃ ---
		int day = TimeSource.GetDayNumber();
		_dayLabel.Text = $"Dzień {day}";
	}
}
