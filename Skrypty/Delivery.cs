using Godot;
using System.Collections.Generic;

public partial class Delivery : Area2D
{
	[Export] public MovementScript _movementScript;
	[Export] public PlayerMoney _playerMoney;
	[Export] public int MaxPackageAmount = 2;   // maksymalna ilość paczek
	public int CurrentPackageAmount = 0;        // aktualna ilość paczek
	public int DeliveredPackagesPerDay = 0; 

	private Package _overlappingPackageNode = null;
	private Customer _overlappingCustomerNode = null;

	private bool _canTakePackage = false;
	private bool _canDeliverPackage = false;

	// Lista podniesionych paczek (możesz wykorzystać później np. do ponownego włączenia)
	private List<Package> _collectedPackages = new List<Package>();

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;
	}

	private void OnAreaEntered(Area2D area)
	{
		// Gdy gracz wjeżdża w paczkę
		if (area.GetParent().IsInGroup("Package"))
		{
			_overlappingPackageNode = area.GetParent() as Package;
			_canTakePackage = true;
			//GD.Print("Wykryto paczkę: " + area.GetParent().Name);
		}

		// Gdy gracz wjeżdża w klienta
		if (area.GetParent().IsInGroup("Customer"))
		{
			_overlappingCustomerNode = area.GetParent() as Customer;
			_canDeliverPackage = true;
			GD.Print("Wykryto klienta: " + area.GetParent().Name);
		}
	}

	private void OnAreaExited(Area2D area)
	{
		// Gdy gracz wyjeżdża z pola paczki
		if (area.GetParent() == _overlappingPackageNode)
		{
			_overlappingPackageNode = null;
			_canTakePackage = false;
		}

		// Gdy gracz wyjeżdża z pola klienta
		if (area.GetParent() == _overlappingCustomerNode)
		{
			_overlappingCustomerNode = null;
			_canDeliverPackage = false;
		}
	}

	public override void _Process(double delta)
	{
		// Sprawdzanie naciśnięcia przycisku akcji
		if (Input.IsActionJustPressed("action"))
		{
			// --- PODNOSZENIE PACZKI ---
			if (_canTakePackage && _overlappingPackageNode != null && _overlappingPackageNode.Visible)
			{
				//GD.Print($"Mogę podnieść paczkę?");
				TryTakePackage();
			}

			// --- DOSTARCZANIE PACZKI ---
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
			// Paczka "znika", ale nie jest usuwana
			_overlappingPackageNode.Visible = false;
			
			// Znajdujemy Area2D w paczce i wyłączamy jej monitoring
			var area = _overlappingPackageNode.GetNode<Area2D>("Area2D");
			if (area != null)
			{
				area.Monitoring = false;
				area.Monitorable = false; // opcjonalnie — przestaje być wykrywalna przez inne Area2D
			}
			
			_collectedPackages.Add(_overlappingPackageNode);
			CurrentPackageAmount++;

			//GD.Print($"Podniesiono paczkę. Aktualnie wieziesz: {CurrentPackageAmount}/{MaxPackageAmount}");
			PrintCollectedPackages();
		}
		else
		{
			//GD.Print("Nie możesz podnieść więcej paczek, albo nie stoisz!");
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
					//GD.Print($"Dostarczono paczkę. Pozostało: {CurrentPackageAmount}/{MaxPackageAmount}");
					return; // właściwa znaleziona
				}
				else
				{
					//GD.Print("Zły klient!");
				}
			}
			
		}
		else
		{
			//GD.Print("Nie masz żadnych paczek do dostarczenia, albo nie stoisz!");
		}
	}

	// Pomocnicza metoda (jeśli kiedyś chcesz przywracać paczki)
	public void ResetPackages()
	{
		foreach (var p in _collectedPackages)
			p.Visible = true;
			
		_collectedPackages.Clear();
		CurrentPackageAmount = 0;
	}
}
