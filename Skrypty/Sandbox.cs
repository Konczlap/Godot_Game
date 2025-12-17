// Sandbox.cs
using Godot;
using System;

public partial class Sandbox : Node
{
	[Export] private CharacterBody2D player;
	[Export] private Gas gas;
	[Export] private PlayerMoney playerMoney;
	[Export] private DayNightCycle dayNightCycle;
	[Export] private MovementScript movementScript;
	[Export] private Delivery delivery;
	[Export] private Car car;
	
	public override void _Ready()
	{
		var sm = GetNodeOrNull<SaveManager>("/root/SaveManager");
		var vehicleManager = GetNodeOrNull<VehicleManager>("/root/VehicleManager");
		
		if (sm == null)
		{
			GD.PrintErr("SaveManager singleton nie znaleziony!");
			return;
		}
		
		if (vehicleManager == null)
		{
			GD.PrintErr("VehicleManager singleton nie znaleziony!");
			return;
		}
		
		if (sm.StartNewGame)
		{
			GD.Print("🌱 Nowa gra!");
			sm.StartNewGame = false;
			
			player.GlobalPosition = new Vector2(346, 217);
			gas.SetFuel(100f);
			playerMoney.SetMoney(50f);
			dayNightCycle.SetDayNumber(1);
			
			vehicleManager.LoadOwnedVehicles("0");
			vehicleManager.LoadActiveVehicle(0);
			
			ApplyVehicleStats();
			return;
		}
		
		if (sm.LoadSave())
		{
			player.GlobalPosition = sm.PlayerPosition;
			gas.SetFuel(sm.Fuel);
			playerMoney.SetMoney(sm.Money);
			dayNightCycle.SetDayNumber(sm.Day);
			
			vehicleManager.LoadOwnedVehicles(sm.OwnedVehicles);
			vehicleManager.LoadActiveVehicle(sm.ActiveVehicle);
			
			var moneyHUD = GetNodeOrNull<MoneyHUD>("../MoneyHUD");
		if (moneyHUD != null)
			moneyHUD.ForceUpdate();
		
		GD.Print($"📂 Save wczytany! Aktywny pojazd: {vehicleManager.GetActiveVehicle()}");
		}
		else
		{
			GD.Print("▶️ Brak zapisu — start nowej gry z domyślnych wartości.");
			
			player.GlobalPosition = new Vector2(346, 217);
			gas.SetFuel(100f);
			playerMoney.SetMoney(50f);
			dayNightCycle.SetDayNumber(1);
			
			vehicleManager.LoadOwnedVehicles("0");
			vehicleManager.LoadActiveVehicle(0);
		}
		
		ApplyVehicleStats();
	}
	
	private void ApplyVehicleStats()
	{
		var vehicleManager = GetNode<VehicleManager>("/root/VehicleManager");
		var vehicleData = vehicleManager.GetActiveVehicleData();
		
		if (movementScript != null)
		{
			movementScript.MaxForwardSpeed = vehicleData.MaxSpeed;
			movementScript.MaxBackwardSpeed = -vehicleData.MaxSpeed / 2f;
			GD.Print($"🚗 Ustawiono prędkość: {vehicleData.MaxSpeed}");
		}
		
		if (delivery != null)
		{
			delivery.MaxPackageAmount = vehicleData.PackageCapacity;
			GD.Print($"📦 Ustawiono pojemność: {vehicleData.PackageCapacity} paczek");
		}
		
		if (gas != null)
		{
		gas._fuelConsumptionRate = vehicleData.FuelConsumption;
		gas.UpdateFuelConsumption(); // DODAJ TĘ LINIJKĘ
		GD.Print($"⛽ Ustawiono spalanie: {vehicleData.FuelConsumption} L/s");
		}
		
		if (car != null)
		{
			car.UpdateCarAppearance();
			GD.Print($"🎨 Zaktualizowano wygląd pojazdu");
		}
		
		GD.Print($"✅ Załadowano pojazd: {vehicleData.Name}");
	}
}
