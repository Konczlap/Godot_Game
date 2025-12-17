using Godot;
using System.Collections.Generic;

public partial class Delivery : Area2D
{
	[Export] public MovementScript _movementScript;
	// ❌ USUŃ: [Export] public PlayerMoney _playerMoney;
	
	private PlayerMoney _playerMoney; // ✅ Będzie pobrane z singletona
	
	public int MaxPackageAmount = 2;
	public int CurrentPackageAmount = 0;
	public int DeliveredPackagesPerDay = 0;

	private Package _overlappingPackageNode = null;
	private Customer _overlappingCustomerNode = null;
	private bool _canTakePackage = false;
	private bool _canDeliverPackage = false;
	private List<Package> _collectedPackages = new List<Package>();
	
	private Minimap _minimap;
	private VehicleManager _vehicleManager;
	private MoneyHUD _moneyHUD;

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;
		
		// ✅ NAPRAWIONE - Pobierz singleton
		_playerMoney = PlayerMoney.Instance;
		if (_playerMoney == null)
		{
			GD.PrintErr("❌ Delivery: PlayerMoney.Instance jest NULL!");
		}
		else
		{
			GD.Print($"✅ Delivery: Połączono z PlayerMoney.Instance (ID: {_playerMoney.GetInstanceId()})");
		}
		
		_vehicleManager = VehicleManager.Instance;
		UpdateMaxPackageAmount();
		
		// Pobierz MoneyHUD
		_moneyHUD = GetTree().Root.GetNodeOrNull<MoneyHUD>("Node2D/MoneyHUD");
		if (_moneyHUD == null)
		{
			var allNodes = GetTree().GetNodesInGroup("HUD");
			foreach (Node node in allNodes)
			{
				if (node is MoneyHUD hud)
				{
					_moneyHUD = hud;
					break;
				}
			}
			
			if (_moneyHUD == null)
			{
				GD.PrintErr("⚠️ Delivery: Nie znaleziono MoneyHUD!");
			}
		}
		
		_minimap = GetTree().Root.GetNode<Minimap>("Node2D/Minimap");
		if (_minimap == null)
		{
			GD.PrintErr("Delivery: Nie znaleziono minimapy!");
		}
		else
		{
			CallDeferred(nameof(InitializeMinimapTarget));
		}
	}
	
	public void UpdateMaxPackageAmount()
	{
		if (_vehicleManager != null)
		{
			var vehicleData = _vehicleManager.GetActiveVehicleData();
			MaxPackageAmount = vehicleData.PackageCapacity;
			GD.Print($"📦 Delivery: Pojemność ustawiona na {MaxPackageAmount} paczek");
		}
	}

	private void InitializeMinimapTarget()
	{
		GetTree().CreateTimer(0.5).Timeout += () =>
		{
			UpdateMinimapTarget();
		};
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area.GetParent().IsInGroup("Package"))
		{
			_overlappingPackageNode = area.GetParent() as Package;
			_canTakePackage = true;
		}

		if (area.GetParent().IsInGroup("Customer"))
		{
			_overlappingCustomerNode = area.GetParent() as Customer;
			_canDeliverPackage = true;
			GD.Print("Wykryto klienta: " + area.GetParent().Name);
		}
	}

	private void OnAreaExited(Area2D area)
	{
		if (area.GetParent() == _overlappingPackageNode)
		{
			_overlappingPackageNode = null;
			_canTakePackage = false;
		}

		if (area.GetParent() == _overlappingCustomerNode)
		{
			_overlappingCustomerNode = null;
			_canDeliverPackage = false;
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("action"))
		{
			if (_canTakePackage && _overlappingPackageNode != null && _overlappingPackageNode.Visible)
			{
				TryTakePackage();
			}

			if (_canDeliverPackage && _overlappingCustomerNode != null)
			{
				TryDeliverPackage();
			}
		}
	}

	private void TryTakePackage()
	{
		if (CurrentPackageAmount < MaxPackageAmount && _movementScript.GetIsStanding())
		{
			_overlappingPackageNode.Visible = false;
			
			var area = _overlappingPackageNode.GetNode<Area2D>("Area2D");
			if (area != null)
			{
				area.Monitoring = false;
				area.Monitorable = false;
			}
			
			_collectedPackages.Add(_overlappingPackageNode);
			CurrentPackageAmount++;

			PrintCollectedPackages();
			UpdateMinimapTarget();
		}
	}
	
	private void PrintCollectedPackages()
	{
		foreach (var p in _collectedPackages)
		{
			GD.Print($"Masz paczkę dla: {p.GetTargetCustomer()}");
		}
	}

	private void TryDeliverPackage()
	{	
		if (CurrentPackageAmount > 0 && _movementScript.GetIsStanding())
		{
			foreach (var _collectedPackage in _collectedPackages.ToArray())
			{
				if (_collectedPackage.GetTargetCustomer() == _overlappingCustomerNode.Name)
				{
					// ✅ TERAZ UŻYWA TEGO SAMEGO PLAYERMONEY CO SKLEP
					_playerMoney.AddMoney(_collectedPackage.GetPackagePrice());
					
					if (_moneyHUD != null)
					{
						_moneyHUD.ForceUpdate();
						GD.Print("💰 HUD zaktualizowany po dostawie!");
					}
					
					CurrentPackageAmount--;
					_collectedPackages.Remove(_collectedPackage);
					DeliveredPackagesPerDay++;
					
					UpdateMinimapTarget();
					return;
				}
			}
		}
	}

	private void UpdateMinimapTarget()
	{
		if (_minimap == null)
		{
			GD.PrintErr("UpdateMinimapTarget: Minimap is null!");
			return;
		}

		if (_collectedPackages.Count > 0)
		{
			var firstPackage = _collectedPackages[0];
			var targetCustomer = firstPackage.GetTargetCustomerNode();
			
			if (targetCustomer != null)
			{
				_minimap.target_package = targetCustomer;
				_minimap.ShowCustomerMarker();
				GD.Print($"📍 Minimap: Cel ustawiony na klienta {targetCustomer.Name}");
			}
			else
			{
				GD.PrintErr("UpdateMinimapTarget: Target customer is null!");
			}
		}
		else
		{
			var nearestPackage = FindNearestVisiblePackage();
			if (nearestPackage != null)
			{
				_minimap.target_package = nearestPackage;
				_minimap.ShowPackageMarker();
				GD.Print($"📦 Minimap: Cel ustawiony na paczkę {nearestPackage.Name}");
			}
			else
			{
				_minimap.target_package = null;
				GD.Print("⚠️ Minimap: Brak widocznych paczek!");
			}
		}
	}

	private Node2D FindNearestVisiblePackage()
	{
		var packages = GetTree().GetNodesInGroup("Package");
		Node2D nearest = null;
		float minDistance = float.MaxValue;

		foreach (Node node in packages)
		{
			if (node is Package pkg && pkg.Visible)
			{
				float distance = GlobalPosition.DistanceTo(pkg.GlobalPosition);
				if (distance < minDistance)
				{
					minDistance = distance;
					nearest = pkg;
				}
			}
		}

		return nearest;
	}

	public void ResetPackages()
	{
		foreach (var p in _collectedPackages)
			p.Visible = true;
			
		_collectedPackages.Clear();
		CurrentPackageAmount = 0;
		
		UpdateMinimapTarget();
	}
	
	public void UpdateMinimapAfterReset()
	{
		_collectedPackages.Clear();
		CurrentPackageAmount = 0;
		UpdateMinimapTarget();
		GD.Print("🗺️ Delivery: Minimap zaktualizowana po resecie");
	}
}
