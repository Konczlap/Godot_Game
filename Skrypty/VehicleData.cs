// VehicleData.cs
using Godot;

public enum VehicleType
{
	Car,
	Kombi,
	Truck
}

public struct VehicleData
{
	public VehicleType Type;
	public string Name;
	public float Price;
	public float MaxSpeed;
	public int PackageCapacity;
	public float FuelConsumption;
	public string TexturePath;
	
	public VehicleData(VehicleType type, string name, float price, float maxSpeed, int packageCapacity, float fuelConsumption, string texturePath)
	{
		Type = type;
		Name = name;
		Price = price;
		MaxSpeed = maxSpeed;
		PackageCapacity = packageCapacity;
		FuelConsumption = fuelConsumption;
		TexturePath = texturePath;
	}
	
	public static VehicleData GetVehicleData(VehicleType type)
	{
		switch (type)
		{
			case VehicleType.Car:
	return new VehicleData(
		VehicleType.Car,
		"Osobowy",
		0f,
		75f,  // ZMIENIONE z 300f
		2,
		2.0f,
        "res://Assety do gry/Car1.png"
	);

case VehicleType.Kombi:
	return new VehicleData(
		VehicleType.Kombi,
		"Kombi",
		10f,
		62f,  // proporcjonalnie wolniejsze
		4,
		1.5f,
        "res://Assety do gry/kombi.png"
	);

case VehicleType.Truck:
	return new VehicleData(
		VehicleType.Truck,
		"Ciężarówka",
		15f,
		45f,  // najwolniejsze
		8,
		3.5f,
        "res://Assety do gry/truck.png"
	);
			
			default:
				return GetVehicleData(VehicleType.Car);
		}
	}
}
