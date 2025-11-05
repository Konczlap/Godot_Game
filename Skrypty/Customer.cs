using Godot;
using System.Collections.Generic;

public partial class Customer : Node2D
{
	[Export] private string CustomerName;
	[Export] public Color CustomerColor; // kolor z inspektora
	[Export] public Vector2 CustomerPosition;

	//Paczki przypisane do tego klienta
	public List<Package> AssignedPackages = new List<Package>();

	public void AssignPackage(Package package)
	{
		if (!AssignedPackages.Contains(package))
			AssignedPackages.Add(package);
	}

	public override void _Ready()
	{
		// (opcjonalnie) ustawienie koloru np. sprite'a klienta
		CustomerName = Name;
		GD.Print($"Nazwa Klienta: {CustomerName}");
		CustomerPosition = Position;
		
		var sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		if (sprite != null)
			CustomerColor = sprite.Modulate;
	}
	
	public string GetCustomerName()
	{
		return CustomerName;
	}
}
