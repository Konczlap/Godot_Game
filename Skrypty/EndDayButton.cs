using Godot;
using System;

public partial class EndDayButton : Button
{
	public DayNightCycle Cycle;

	public override void _Pressed()
	{
		Cycle.StartNextDay();
	}
}
