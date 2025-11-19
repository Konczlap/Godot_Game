using Godot;
using System;

public partial class Main : Node2D
{
	[Export] public DayNightCycle DayNightCycle;
	[Export] public CanvasLayer SummaryView;
	private EndDayButton EndButton;

	public override void _Ready()
	{
		EndButton = SummaryView.GetNode<EndDayButton>("Control/PodsumowaniePanel/KoniecDniaButton");
		GD.Print(EndButton.Name);
		EndButton.Cycle = DayNightCycle;
	}
}
