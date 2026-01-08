using Godot;
using System;

public partial class EndDay : Area2D
{
	[Export] private DayNightCycle _dayNightCycle;
	[Export] private PlayerMoney _playerMoney;
	[Export] private Gas _gas;
	[Export] private MovementScript _movementScript;
	[Export] private Delivery _delivery;
	[Export] private VehicleManager _vehicleManager;
	//[Export] private SaveManager _saveManager;
	[Export] public Node2D PackagesContainer;
	[Export] public Node2D CustomersContainer;
	
	private bool _playerInside = false;
	private int _invisiblePackageAmount = 0;
	private int _maxInvisiblePackage = 17;
	private bool _allPackageTaken = false;
	
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;
	}
	
	public override void _Process(double delta)
	{
		_invisiblePackageAmount = 0;  // RESET W KAŻDEJ KLATCE

		bool allInvisible = true;

		foreach (Node child in PackagesContainer.GetChildren())
		{
			if (child is Node2D item)
			{
				if (!item.Visible)
					_invisiblePackageAmount++;
				else
					allInvisible = false;
			}
		}

		// Czy wszystkie paczki dostarczone?
		if (allInvisible && _delivery.CurrentPackageAmount == 0)
		{
			if (!_allPackageTaken)
				GD.Print("Dostarczyłeś wszystkie paczki - wracaj!");

			_allPackageTaken = true;
		}
		else
		{
			_allPackageTaken = false;
		}

		// Koniec dnia
		if (Input.IsActionPressed("action") &&
			_movementScript.GetIsStanding() &&
			_playerInside &&
			(_dayNightCycle.IsNight || _allPackageTaken))
		{
			_dayNightCycle.EndDay();
			var sm = GetNodeOrNull<SaveManager>("/root/SaveManager");
			GD.Print($"SaveManager: {sm}");
			GD.Print($"VehicleManager: {_vehicleManager}");
			if (sm != null)
				sm.SaveGame(_movementScript, _gas, _playerMoney, _dayNightCycle, _vehicleManager);
			else
				GD.PrintErr("❌ Nie znaleziono SaveManager!");
			//SaveManager.SaveGame(_movementScript, _gas, _playerMoney, _dayNightCycle);
			//Zapis
		}
	}
	
	public void OnAreaEntered(Area2D area)
	{
		if (area.GetParent().IsInGroup("Player"))
		{
			_playerInside = true;
			GD.Print("Naduś E");
			
			Node car = area.GetParent();
			Node player = car.GetParent();
			GD.Print($"{player.Name}");
		}
	}

	public void OnAreaExited(Area2D area)
	{
		if (area.GetParent().IsInGroup("Player"))
		{
			_playerInside = false;
			GD.Print("🚗 Opuściłeś dom.");
		}
	}

	//public override void _Process(double delta)
	//{
		//foreach (Node child in PackagesContainer.GetChildren())
		//{
			//if (child is Node2D item)
			//{
				//if (item.Visible == false)
				//{
					//_invisiblePackageAmount++;
					//if (_invisiblePackageAmount == _maxInvisiblePackage && _delivery.CurrentPackageAmount == 0)
					//{
						//GD.Print("Dostarczyłeś wszystkie paczki - wracaj!");
						//_allPackageTaken = true;
					//}
				//}
				//else
				//{
					//return;
				//}
			//}
			//else
			//{
				////GD.Print("To nie działa :(");
			//}
		//}
		//// Koniec dnia po naciśnięciu "E"
		//if (Input.IsActionPressed("action") && _movementScript.GetIsStanding() && _playerInside && (_dayNightCycle.IsNight || _allPackageTaken))
		//{
			//_invisiblePackageAmount = 0;
			//_allPackageTaken = false;
			//_dayNightCycle.EndDay();
		//}
	//}
}
