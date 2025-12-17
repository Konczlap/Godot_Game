// VehicleManager.cs
using Godot;
using System.Collections.Generic;

public partial class VehicleManager : Node
{
	private static VehicleManager _instance;
	public static VehicleManager Instance => _instance;
	
	private HashSet<VehicleType> _ownedVehicles = new HashSet<VehicleType>();
	private VehicleType _activeVehicle = VehicleType.Car;
	
	public override void _EnterTree()
	{
		if (_instance != null && _instance != this)
		{
			QueueFree();
			return;
		}
		_instance = this;
	}
	
	public override void _Ready()
	{
		// Gracz zawsze startuje z osobowym
		_ownedVehicles.Add(VehicleType.Car);
		_activeVehicle = VehicleType.Car;
	}
	
	public bool OwnsVehicle(VehicleType type)
	{
		return _ownedVehicles.Contains(type);
	}
	
	public void PurchaseVehicle(VehicleType type)
	{
		_ownedVehicles.Add(type);
		GD.Print($"✅ Zakupiono pojazd: {type}");
	}
	
	public VehicleType GetActiveVehicle()
	{
		return _activeVehicle;
	}
	
	public void SetActiveVehicle(VehicleType type)
	{
		if (_ownedVehicles.Contains(type))
		{
			_activeVehicle = type;
			GD.Print($"🚗 Aktywny pojazd: {type}");
		}
		else
		{
			GD.PrintErr($"❌ Nie posiadasz pojazdu: {type}");
		}
	}
	
	public VehicleData GetActiveVehicleData()
	{
		return VehicleData.GetVehicleData(_activeVehicle);
	}
	
	public List<VehicleType> GetOwnedVehicles()
	{
		return new List<VehicleType>(_ownedVehicles);
	}
	
	// Metody do zapisu/odczytu
	public string GetOwnedVehiclesString()
	{
		var list = new List<string>();
		foreach (var vehicle in _ownedVehicles)
		{
			list.Add(((int)vehicle).ToString());
		}
		return string.Join(",", list);
	}
	
	public void LoadOwnedVehicles(string data)
	{
		_ownedVehicles.Clear();
		
		if (string.IsNullOrEmpty(data))
		{
			_ownedVehicles.Add(VehicleType.Car);
			return;
		}
		
		var parts = data.Split(',');
		foreach (var part in parts)
		{
			if (int.TryParse(part, out int vehicleId))
			{
				_ownedVehicles.Add((VehicleType)vehicleId);
			}
		}
		
		// Upewnij się że gracz ma przynajmniej osobowy
		if (_ownedVehicles.Count == 0)
		{
			_ownedVehicles.Add(VehicleType.Car);
		}
	}
	
	public int GetActiveVehicleId()
	{
		return (int)_activeVehicle;
	}
	
	public void LoadActiveVehicle(int vehicleId)
	{
		_activeVehicle = (VehicleType)vehicleId;
		
		// Zabezpieczenie - jeśli nie posiadamy tego pojazdu, ustaw na osobowy
		if (!_ownedVehicles.Contains(_activeVehicle))
		{
			_activeVehicle = VehicleType.Car;
		}
	}
}
