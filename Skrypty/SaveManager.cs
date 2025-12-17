// SaveManager.cs
using Godot;
using System;
using Godot.Collections;

public partial class SaveManager : Node
{
	private string saveFile = "user://player_log.txt";

	public int Day { get; set; } = 1;
	public Vector2 PlayerPosition { get; set; } = Vector2.Zero;
	public float Fuel { get; set; } = 100f;
	public float Money { get; set; } = 0f;
	public string OwnedVehicles { get; set; } = "0"; // "0" = tylko Car
	public int ActiveVehicle { get; set; } = 0; // 0 = Car
	
	public bool StartNewGame = false;
	
	public bool LoadSave()
	{
		if (!FileAccess.FileExists(saveFile))
		{
			GD.Print("🚫 Brak save!");
			return false;
		}

		using var file = FileAccess.Open(saveFile, FileAccess.ModeFlags.Read);
		string content = file.GetAsText();

		var parsed = Json.ParseString(content);

		if (parsed.VariantType != Variant.Type.Dictionary)
		{
			GD.PrintErr("❌ Save uszkodzony!");
			return false;
		}

		var loaded = (Dictionary)parsed;

		Day = Convert.ToInt32((double)loaded["day"]);
		PlayerPosition = new Vector2(
			Convert.ToSingle((double)loaded["player_x"]),
			Convert.ToSingle((double)loaded["player_y"])
		);

		Fuel = Convert.ToSingle((double)loaded["fuel"]);
		Money = Convert.ToSingle((double)loaded["money"]);
		
		// Nowe pola - z fallbackiem dla starych zapisów
		if (loaded.ContainsKey("owned_vehicles"))
		{
			OwnedVehicles = loaded["owned_vehicles"].ToString();
		}
		else
		{
			OwnedVehicles = "0"; // Tylko Car
		}
		
		if (loaded.ContainsKey("active_vehicle"))
		{
			ActiveVehicle = Convert.ToInt32((double)loaded["active_vehicle"]);
		}
		else
		{
			ActiveVehicle = 0; // Car
		}

		GD.Print($"📂 Save wczytany! Day: {Day}, Money: {Money}, Active Vehicle: {ActiveVehicle}");
		return true;
	}

	public void SaveGame(MovementScript player, Gas gas, PlayerMoney money, DayNightCycle dayNight)
	{
		PlayerPosition = player.GlobalPosition;
		Fuel = gas.GetFuel();
		Money = money.GetMoney();
		Day = dayNight.GetDayNumber() + 1;
		
		// Pobierz dane z VehicleManager
		var vehicleManager = GetNodeOrNull<VehicleManager>("/root/VehicleManager");
		if (vehicleManager != null)
		{
			OwnedVehicles = vehicleManager.GetOwnedVehiclesString();
			ActiveVehicle = vehicleManager.GetActiveVehicleId();
		}

		var saveData = new Dictionary
		{
			{ "day", Day },
			{ "player_x", (double)PlayerPosition.X },
			{ "player_y", (double)PlayerPosition.Y },
			{ "fuel", (double)Fuel },
			{ "money", (double)Money },
			{ "owned_vehicles", OwnedVehicles },
			{ "active_vehicle", ActiveVehicle }
		};

		using var file = FileAccess.Open(saveFile, FileAccess.ModeFlags.Write);
		file.StoreString(Json.Stringify(saveData, "\t"));

		GD.Print($"💾 Save zapisany! Owned: {OwnedVehicles}, Active: {ActiveVehicle}");
	}
}
