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
	public string OwnedVehicles { get; set; } = "";
	public int ActiveVehicleId { get; set; } = 0;
	
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
		
		OwnedVehicles = loaded.ContainsKey("owned_vehicles")
		? loaded["owned_vehicles"].ToString()
		: "";

		ActiveVehicleId = loaded.ContainsKey("active_vehicle")
		? Convert.ToInt32((double)loaded["active_vehicle"])
		: 0;

		GD.Print($"📂 Save wczytany! {loaded}");
		return true;
	}

	public void SaveGame(
		MovementScript player,
		Gas gas,
		PlayerMoney money,
		DayNightCycle dayNight,
		VehicleManager vehicleManager   // ⬅ NOWY PARAMETR
	)
	{
		PlayerPosition = player.GlobalPosition;
		Fuel = gas.GetFuel();
		Money = money.GetMoney();
		Day = dayNight.GetDayNumber() + 1;

		OwnedVehicles = vehicleManager.GetOwnedVehiclesString();
		ActiveVehicleId = vehicleManager.GetActiveVehicleId();

		var saveData = new Dictionary
		{
			{ "day", Day },
			{ "player_x", (double)PlayerPosition.X },
			{ "player_y", (double)PlayerPosition.Y },
			{ "fuel", (double)Fuel },
			{ "money", (double)Money },

			// 🚗 NOWE
			{ "owned_vehicles", OwnedVehicles },
			{ "active_vehicle", ActiveVehicleId }
		};

		using var file = FileAccess.Open(saveFile, FileAccess.ModeFlags.Write);
		file.StoreString(Json.Stringify(saveData, "\t"));

		GD.Print("💾 Save zapisany!");
	}
	
	public void SaveNewGame()
	{
		Day = 1;
		PlayerPosition = new Vector2(346, 217);
		Fuel = 100f;
		Money = 50f;

		OwnedVehicles = "0"; // albo ID startowego auta
		ActiveVehicleId = 0;

		var saveData = new Dictionary
		{
			{ "day", Day },
			{ "player_x", (double)PlayerPosition.X },
			{ "player_y", (double)PlayerPosition.Y },
			{ "fuel", (double)Fuel },
			{ "money", (double)Money },
			{ "owned_vehicles", OwnedVehicles },
			{ "active_vehicle", ActiveVehicleId }
		};

		using var file = FileAccess.Open(saveFile, FileAccess.ModeFlags.Write);
		file.StoreString(Json.Stringify(saveData, "\t"));

		GD.Print("🆕 Nowa gra zapisana!");
	}
}
