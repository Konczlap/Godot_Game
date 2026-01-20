using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	[Export] public NodePath ResumeButtonPath = "Panel/ResumeButton";
	[Export] public NodePath RestartButtonPath = "Panel/RestartButton";
	[Export] public NodePath MainMenuButtonPath = "Panel/MainMenuButton";
	
	[Export] public MovementScript player;
	[Export] public Gas gas;
	[Export] public PlayerMoney playerMoney;
	[Export] public DayNightCycle dayNightCycle;
	[Export] public Delivery delivery;
	[Export] public MessageHUD _messageHUD;
	[Export] public HintTutorial hintTutorial;
	[Export] public FuelWarningHUD fuelWarningHUD;
	
	private Button resumeButton;
	private Button restartButton;
	private Button mainMenuButton;
	
	//public override void _ExitTree()
	//{
		//GD.Print("PauseMenu usunięte");
	//}
	
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
		
		if (!string.IsNullOrEmpty(RestartButtonPath.ToString()))
		{
			restartButton = GetNodeOrNull<Button>(RestartButtonPath);

			if (restartButton != null)
			{
				restartButton.Pressed += OnRestartButtonPressed;
			}
		}
		
		if (!string.IsNullOrEmpty(MainMenuButtonPath.ToString()))
		{
			mainMenuButton = GetNodeOrNull<Button>(MainMenuButtonPath);

			if (mainMenuButton != null)
			{
				mainMenuButton.Pressed += OnMainMenuButtonPressed;
			}
		}
	}

	private void OnResumeButtonPressed()
	{
		GetTree().Paused = false;
		Visible = false;
		_messageHUD?.ShowLastMessage(new Color("#FFFFFF"));
		if(fuelWarningHUD.IsShowed)
			fuelWarningHUD?.ShowCommunicate();
		else
			return;
		
	}
	
	private void OnRestartButtonPressed()
	{
		var sm = GetNodeOrNull<SaveManager>("/root/SaveManager");
		if (sm == null)
		{
			GD.PrintErr("SaveManager singleton nie znaleziony!");
			return;
		}
		
		if (sm == null)
		{
			GD.PrintErr("❌ Brak SaveManager!");
			return;
		}
		
		sm.LoadSave();
		
		// ustawienie wartości w grze
		player.GlobalPosition = sm.PlayerPosition;
		player.SetCurrentSpeed(0f);
		gas.SetFuel(sm.Fuel);
		playerMoney.SetMoney(sm.Money);
		dayNightCycle.SetDayNumber(sm.Day);
		dayNightCycle.RestartDay();
		delivery.ResetPackages();
		var vm = GetNodeOrNull<VehicleManager>("/root/VehicleManager");
		if (vm != null)
			{
				vm.LoadOwnedVehicles(sm.OwnedVehicles);
				vm.LoadActiveVehicle(sm.ActiveVehicleId);
			}
		
		GD.Print("🔄 Gra zrestartowana od początku dnia!");
		hintTutorial.StartTutorialAgain();
		OnResumeButtonPressed(); // ukryj menu pauzy i odblokuj grę
	}
	
	private void OnMainMenuButtonPressed()
	{
		// Zawsze najpierw zdejmujemy pauzę
		GetTree().Paused = false;

		// (opcjonalnie) ukryj menu pauzy
		Visible = false;
		
		// 🔴 Odłącz menu od drzewa
		//QueueFree();

		// Przejście do Main Menu
		GetTree().ChangeSceneToFile("res://Sceny/main_menu.tscn");
	}

	public void OnContinuePressed()
	{
		OnResumeButtonPressed();
	}
}
