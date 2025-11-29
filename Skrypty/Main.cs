using Godot;
using System;

public partial class Main : Node2D
{
	[Export] public DayNightCycle DayNightCycle;
	[Export] public CanvasLayer SummaryView;
	[Export] public CharacterBody2D Player; 
	
	private EndDayButton EndButton;
	private CanvasLayer _minimap;

	public override void _Ready()
	{
		_minimap = GetNode<CanvasLayer>("Minimap");
		EndButton = SummaryView.GetNode<EndDayButton>("Control/PodsumowaniePanel/KoniecDniaButton");
		
		GD.Print(EndButton.Name);
		EndButton.Cycle = DayNightCycle;
		
		if (_minimap != null && Player != null)
		{
			var minimapScript = _minimap as Minimap;
			if (minimapScript != null)
			{
				minimapScript.player_node = Player;
			}
			else
			{
				GD.PrintErr("Minimap node does not have a Minimap script attached!");
			}
		}
		else if (Player == null)
		{
			GD.PrintErr("Main: Gracz nie jest przypisany! Przypisz węzeł gracza w inspektorze.");
		}
	}
}
