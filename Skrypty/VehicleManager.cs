using Godot;
using System;
using System.Collections.Generic;

public partial class VehicleManager : Node
{
	// ====== DANE POJAZDÓW ======
	[Export] public VehicleData Personal;
	[Export] public VehicleData Kombi;
	[Export] public VehicleData Truck;

	// ====== SYSTEMY GRY ======
	[Export] private MovementScript movement;
	[Export] private Gas gas;
	[Export] private Delivery delivery;
	[Export] private Sprite2D vehicleSprite;

	public VehicleData CurrentVehicle { get; private set; }

	private HashSet<VehicleType> owned = new();

	// ======================
	// INIT
	// ======================
	//public override void _Ready()
	//{
		//owned.Add(VehicleType.Personal);
		//EquipVehicle(Personal);
	//}

	// ======================
	// LOGIKA
	// ======================

	public bool Owns(VehicleType type) => owned.Contains(type);

	public bool BuyVehicle(VehicleData data, PlayerMoney money)
	{
		if (Owns(data.Type))
		{
			EquipVehicle(data);
			return true;
		}

		if (!money.SpendMoney(data.Price))
			return false;

		owned.Add(data.Type);
		EquipVehicle(data);
		return true;
	}

	public void EquipVehicle(VehicleData data)
	{
		if (data == null)
			return;

		CurrentVehicle = data;
		ApplyStats();
	}

	private void ApplyStats()
	{
		if (CurrentVehicle == null)
			return;

		GD.Print($"movement = {movement}");
		GD.Print($"gas = {gas}");
		GD.Print($"delivery = {delivery}");
		// ===== RUCH =====
		if (movement != null)
		{
			movement.MaxForwardSpeed  = CurrentVehicle.MaxForwardSpeed;
			movement.MaxBackwardSpeed = CurrentVehicle.MaxBackwardSpeed;
			movement.Acceleration     = CurrentVehicle.Acceleration;
			movement.BrakePower       = CurrentVehicle.BrakePower;
			movement.EngineBraking    = CurrentVehicle.EngineBraking;
			movement.RotationSpeed    = CurrentVehicle.RotationSpeed;
		}

		// ===== PALIWO =====
		if (gas != null)
			gas.SetFuelConsumptionRate(CurrentVehicle.FuelConsumptionRate);

		// ===== PACZKI =====
		if (delivery != null)
			delivery.SetMaxPackages(CurrentVehicle.MaxPackageAmount);

		// ===== WYGLĄD =====
		if (vehicleSprite != null && CurrentVehicle.Sprite != null)
			vehicleSprite.Texture = CurrentVehicle.Sprite;

		GD.Print($"🚗 Equipped: {CurrentVehicle.Type}");
	}

	// ======================
	// ZAPIS / ODCZYT
	// ======================

	public string GetOwnedVehiclesString()
	{
		var list = new List<string>();
		foreach (var v in owned)
			list.Add(((int)v).ToString());

		return string.Join(",", list);
	}

	public int GetActiveVehicleId()
	{
		return (int)CurrentVehicle.Type;
	}

	public void LoadOwnedVehicles(string data)
	{
		owned.Clear();

		// 🛡 ZAWSZE dodaj osobówkę jako bazę
		owned.Add(VehicleType.Personal);
	
		if (string.IsNullOrEmpty(data))
		{
			owned.Add(VehicleType.Personal);
			return;
		}

		var parts = data.Split(',');
		foreach (var part in parts)
		{
			if (int.TryParse(part, out int id))
				owned.Add((VehicleType)id);
		}

		if (owned.Count == 0)
			owned.Add(VehicleType.Personal);
	}

	public void LoadActiveVehicle(int id)
	{
		var type = (VehicleType)id;

		if (!owned.Contains(type))
			type = VehicleType.Personal;

		VehicleData data = type switch
		{
			VehicleType.Personal => Personal,
			VehicleType.Kombi => Kombi,
			VehicleType.Truck => Truck,
			_ => Personal
		};

		EquipVehicle(data);
	}
}
