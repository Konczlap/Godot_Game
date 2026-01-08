using Godot;
using System;

public partial class Sandbox : Node
{
	[Export] private CharacterBody2D player;
	[Export] private Gas gas;
	[Export] private PlayerMoney playerMoney;
	[Export] private DayNightCycle dayNightCycle;
	[Export] private VehicleManager vehicleManager;

	public override void _Ready()
	{
		// Pobierz singleton SaveManager
		var sm = GetNodeOrNull<SaveManager>("/root/SaveManager");
		if (sm == null)
		{
			GD.PrintErr("SaveManager singleton nie znaleziony!");
			return;
		}

		if (sm.StartNewGame)
		{
			GD.Print("🌱 Nowa gra!");
			sm.StartNewGame = false; // reset flagi
			// Tutaj zainicjuj stan nowej gry (domyślne wartości)
			player.GlobalPosition = new Vector2(346, 217); // przykładowo
			gas.SetFuel(100f);
			playerMoney.SetMoney(50f); // albo startowe 50$
			dayNightCycle.SetDayNumber(1);
			vehicleManager.LoadOwnedVehicles("");
			vehicleManager.LoadActiveVehicle((int)VehicleType.Personal);
			return; // WAŻNE — przerwij dalsze ładowanie zapisu
		}

		// Jeżeli nie new game — wczytujemy zapis
		if (sm.LoadSave())
		{
			player.GlobalPosition = sm.PlayerPosition;
			gas.SetFuel(sm.Fuel);
			playerMoney.SetMoney(sm.Money);
			dayNightCycle.SetDayNumber(sm.Day);
			vehicleManager.LoadOwnedVehicles(sm.OwnedVehicles);
			vehicleManager.LoadActiveVehicle(sm.ActiveVehicleId);
			GD.Print("📂 Save wczytany!");
		}
		else
		{
			GD.Print("▶️ Brak zapisu — start nowej gry z domyślnych wartości.");
			// ustaw domyślne wartości jeśli chcesz
		}
	}
}
