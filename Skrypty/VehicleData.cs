using Godot;
using System;

[GlobalClass]
public partial class VehicleData : Resource
{
	[Export] public VehicleType Type;
	[Export] public float MaxForwardSpeed;
	[Export] public float MaxBackwardSpeed;
	[Export] public float Acceleration;
	[Export] public float BrakePower;
	[Export] public float EngineBraking;
	[Export] public float RotationSpeed;
	[Export] public float FuelConsumptionRate;
	[Export] public int MaxPackageAmount;
	[Export] public int Price; //150, 250, 500
	[Export] public Texture2D Sprite;
}
