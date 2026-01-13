using Godot;
using System.Collections.Generic;

public partial class Delivery : Area2D
{
	[Export] public MovementScript _movementScript;
	[Export] public PlayerMoney _playerMoney;
	[Export] public int MaxPackageAmount = 2;
	public int CurrentPackageAmount = 0;
	public int DeliveredPackagesPerDay = 0;

	private Package _overlappingPackageNode = null;
	private Customer _overlappingCustomerNode = null;
	private bool _canTakePackage = false;
	private bool _canDeliverPackage = false;
	private List<Package> _collectedPackages = new List<Package>();
	
	private Minimap _minimap;

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;
		
		// Znajdź minimapę
		_minimap = GetTree().Root.GetNode<Minimap>("Node2D/Minimap");
		if (_minimap == null)
		{
			GD.PrintErr("Delivery: Nie znaleziono minimapy!");
		}
		else
		{
			// Ustaw pierwszy cel przy starcie gry
			CallDeferred(nameof(InitializeMinimapTarget));
		}
	}

	private void InitializeMinimapTarget()
	{
		// Poczekaj chwilę aż wszystko się załaduje
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
					_playerMoney.AddMoney(_collectedPackage.GetPackagePrice());
					CurrentPackageAmount--;
					_collectedPackages.Remove(_collectedPackage);
					DeliveredPackagesPerDay++;
					
					// Aktualizuj cel na minimapie
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

		// Jeśli mamy paczki, pokaż WSZYSTKICH klientów docelowych
		if (_collectedPackages.Count > 0)
		{
			var targets = new System.Collections.Generic.List<Node2D>();
			
			foreach (var package in _collectedPackages)
			{
				var targetCustomer = package.GetTargetCustomerNode();
				if (targetCustomer != null)
				{
					targets.Add(targetCustomer);
				}
			}
			
			if (targets.Count > 0)
			{
				_minimap.SetTargets(targets);
				_minimap.ShowCustomerMarker();
				GD.Print($"📍 Minimap: Ustawiono {targets.Count} celów (klientów)");
			}
			else
			{
				_minimap.ClearTargets();
				GD.PrintErr("UpdateMinimapTarget: Żaden klient docelowy nie jest dostępny!");
			}
		}
		else
		{
			// Jeśli nie mamy paczek, znajdź najbliższą widoczną paczkę
			var nearestPackage = FindNearestVisiblePackage();
			if (nearestPackage != null)
			{
				var targets = new System.Collections.Generic.List<Node2D> { nearestPackage };
				_minimap.SetTargets(targets);
				_minimap.ShowPackageMarker();
				GD.Print($"📦 Minimap: Cel ustawiony na paczkę {nearestPackage.Name}");
			}
			else
			{
				_minimap.ClearTargets();
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
	
	// Wywoływane na początku nowego dnia - usuwa paczki z ekwipunku gracza
	public void ClearInventory()
	{
		_collectedPackages.Clear();
		CurrentPackageAmount = 0;
		UpdateMinimapTarget();
	}

	// Wywoływane gdy kończy się dzień (godzina 22:00)
	public void StopDeliveries()
	{
		if (_minimap != null)
		{
			_minimap.ClearTargets();
		}
	}
	
	public void SetMaxPackages(int value)
	{
		MaxPackageAmount = value;
	}
}
