// pausemenu.cs

using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	// Referencja tutaj może zostać, jeśli potrzebujesz jej do restartu
	[Export] public Minimap GameMinimap; 
	
	[ExportGroup("UI Buttons")]
	[Export] public NodePath ResumeButtonPath = "Panel/ResumeButton";
	[Export] public NodePath RestartButtonPath = "Panel/RestartButton";
	[Export] public NodePath MainMenuButtonPath = "Panel/MainMenuButton";
	
	[ExportGroup("Game References")]
	[Export] public MovementScript player;
	[Export] public Gas gas;
	[Export] public PlayerMoney playerMoney;
	[Export] public DayNightCycle dayNightCycle;
	[Export] public Delivery delivery;
	
	private Button resumeButton;
	private Button restartButton;
	private Button mainMenuButton;

	public override void _Ready()
	{
		Visible = false;
		ProcessMode = ProcessModeEnum.Always;

		if (!string.IsNullOrEmpty(ResumeButtonPath.ToString()))
		{
			resumeButton = GetNodeOrNull<Button>(ResumeButtonPath);
			if (resumeButton != null) resumeButton.Pressed += OnResumeButtonPressed;
		}
		
		if (!string.IsNullOrEmpty(RestartButtonPath.ToString()))
		{
			restartButton = GetNodeOrNull<Button>(RestartButtonPath);
			if (restartButton != null) restartButton.Pressed += OnRestartButtonPressed;
		}
		
		if (!string.IsNullOrEmpty(MainMenuButtonPath.ToString()))
		{
			mainMenuButton = GetNodeOrNull<Button>(MainMenuButtonPath);
			if (mainMenuButton != null) mainMenuButton.Pressed += OnMainMenuButtonPressed;
		}
	}

	// Usunęliśmy _Input stąd, by nie dublować PauseController

	private void OnResumeButtonPressed()
	{
		// Szukamy kontrolera, żeby użyć jego wspólnej logiki
		var controller = GetParent().GetNodeOrNull<PauseController>("PauseController");
		if (controller != null)
		{
			controller.TogglePause();
		}
		else
		{
			// Fallback, jeśli nie znajdzie kontrolera
			GetTree().Paused = false;
			Visible = false;
			if (GameMinimap != null) GameMinimap.Visible = true;
		}
	}
	
	private void OnRestartButtonPressed()
	{
		var sm = GetNodeOrNull<SaveManager>("/root/SaveManager");
		if (sm == null) return;
		
		sm.LoadSave();
		
		if (player != null) { player.GlobalPosition = sm.PlayerPosition; player.SetCurrentSpeed(0f); }
		if (gas != null) gas.SetFuel(sm.Fuel);
		if (playerMoney != null) playerMoney.SetMoney(sm.Money);
		if (dayNightCycle != null) { dayNightCycle.SetDayNumber(sm.Day); dayNightCycle.RestartDay(); }
		if (delivery != null) delivery.ResetPackages();
		
		OnResumeButtonPressed(); 
	}
	
	private void OnMainMenuButtonPressed()
	{
		GetTree().Paused = false;
		Visible = false;
		GetTree().ChangeSceneToFile("res://Sceny/main_menu.tscn");
	}
}
