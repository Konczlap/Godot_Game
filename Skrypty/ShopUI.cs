using Godot;

public partial class ShopUI : Control
{
	[Signal]
	public delegate void ShopClosedEventHandler();
	
	private PlayerMoney _playerMoney;
	private VehicleManager _vehicleManager;
	private MoneyHUD _moneyHUD; // ✅ DODANE
	
	private VBoxContainer _carContainer;
	private VBoxContainer _kombiContainer;
	private VBoxContainer _truckContainer;
	
	private Label _carSpeed, _carCapacity, _carFuel, _carPrice;
	private Label _kombiSpeed, _kombiCapacity, _kombiFuel, _kombiPrice;
	private Label _truckSpeed, _truckCapacity, _truckFuel, _truckPrice;
	
	private Button _carButton, _kombiButton, _truckButton;
	
	public override void _Ready()
	{
		_playerMoney = PlayerMoney.Instance;
		_vehicleManager = VehicleManager.Instance;
		
		// ✅ NAPRAWIONE - Poprawna ścieżka do MoneyHUD
		_moneyHUD = GetNode<MoneyHUD>("/root/Node2D/MoneyHUD");
		if (_moneyHUD == null)
		{
			GD.PrintErr("⚠️ ShopUI: Nie znaleziono MoneyHUD!");
		}
		else
		{
			GD.Print("✅ ShopUI: MoneyHUD połączony!");
		}
		
		var carsRow = GetNode<HBoxContainer>("CenterContainer/CarsRow");
		_carContainer = carsRow.GetNode<VBoxContainer>("car");
		_kombiContainer = carsRow.GetNode<VBoxContainer>("kombi");
		_truckContainer = carsRow.GetNode<VBoxContainer>("truck");
		
		var carStats = _carContainer.GetNode<VBoxContainer>("Panel/Content/Stats");
		_carSpeed = carStats.GetNode<Label>("Label2");
		_carCapacity = carStats.GetNode<Label>("Label3");
		_carFuel = carStats.GetNode<Label>("Label4");
		_carPrice = carStats.GetNode<Label>("Label");
		_carButton = carStats.GetNode<Button>("Button");
		
		var kombiStats = _kombiContainer.GetNode<VBoxContainer>("Panel/Content/Stats");
		_kombiSpeed = kombiStats.GetNode<Label>("Label2");
		_kombiCapacity = kombiStats.GetNode<Label>("Label3");
		_kombiFuel = kombiStats.GetNode<Label>("Label4");
		_kombiPrice = kombiStats.GetNode<Label>("Label");
		_kombiButton = kombiStats.GetNode<Button>("Button");
		
		var truckStats = _truckContainer.GetNode<VBoxContainer>("Panel/Content/Stats");
		_truckSpeed = truckStats.GetNode<Label>("Label2");
		_truckCapacity = truckStats.GetNode<Label>("Label3");
		_truckFuel = truckStats.GetNode<Label>("Label4");
		_truckPrice = truckStats.GetNode<Label>("Label");
		_truckButton = truckStats.GetNode<Button>("Button");
		
		_carButton.Pressed += OnCarButtonPressed;
		_kombiButton.Pressed += OnKombiButtonPressed;
		_truckButton.Pressed += OnTruckButtonPressed;
		
		GetNode<CenterContainer>("CenterContainer").MouseFilter = MouseFilterEnum.Ignore;
		GetNode<HBoxContainer>("CenterContainer/CarsRow").MouseFilter = MouseFilterEnum.Ignore;
		_carContainer.MouseFilter = MouseFilterEnum.Ignore;
		_kombiContainer.MouseFilter = MouseFilterEnum.Ignore;
		_truckContainer.MouseFilter = MouseFilterEnum.Ignore;
		
		UpdateUI();
	}
	
	private void UpdateUI()
	{
		UpdateVehicleUI(VehicleType.Car, _carSpeed, _carCapacity, _carFuel, _carPrice, _carButton);
		UpdateVehicleUI(VehicleType.Kombi, _kombiSpeed, _kombiCapacity, _kombiFuel, _kombiPrice, _kombiButton);
		UpdateVehicleUI(VehicleType.Truck, _truckSpeed, _truckCapacity, _truckFuel, _truckPrice, _truckButton);
	}
	
	private void OnCarButtonPressed()
	{
		OnVehicleButtonPressed(VehicleType.Car);
	}
	
	private void OnKombiButtonPressed()
	{
		OnVehicleButtonPressed(VehicleType.Kombi);
	}
	
	private void OnTruckButtonPressed()
	{
		OnVehicleButtonPressed(VehicleType.Truck);
	}
	
	private void UpdateVehicleUI(VehicleType type, Label speedLabel, Label capacityLabel, Label fuelLabel, Label priceLabel, Button button)
	{
		var data = VehicleData.GetVehicleData(type);
		bool owned = _vehicleManager.OwnsVehicle(type);
		bool isActive = _vehicleManager.GetActiveVehicle() == type;
		
		speedLabel.Text = $"⚡ Prędkość: {data.MaxSpeed}";
		capacityLabel.Text = $"📦 Pojemność: {data.PackageCapacity} paczek";
		fuelLabel.Text = $"⛽ Spalanie: {data.FuelConsumption} L/s";
		
		if (owned)
		{
			priceLabel.Text = "✅ Posiadany";
			priceLabel.Modulate = new Color(0.3f, 1f, 0.3f);
			
			if (isActive)
			{
				button.Text = "⭐ AKTYWNY";
				button.Modulate = new Color(1f, 1f, 0.3f);
				button.Disabled = true;
			}
			else
			{
				button.Text = "🔄 Wybierz";
				button.Modulate = new Color(0.3f, 0.8f, 1f);
				button.Disabled = false;
			}
		}
		else
		{
			priceLabel.Text = $"💰 Cena: {data.Price}$";
			priceLabel.Modulate = new Color(1f, 1f, 1f);
			
			float currentMoney = _playerMoney?.GetMoney() ?? 0;
			if (currentMoney >= data.Price)
			{
				button.Text = "💵 Kup";
				button.Modulate = new Color(0.3f, 1f, 0.3f);
				button.Disabled = false;
			}
			else
			{
				button.Text = "❌ Za drogi";
				button.Modulate = new Color(1f, 0.3f, 0.3f);
				button.Disabled = true;
			}
		}
	}
	
	private void OnVehicleButtonPressed(VehicleType type)
	{
		var data = VehicleData.GetVehicleData(type);
		bool owned = _vehicleManager.OwnsVehicle(type);
		
		if (owned)
		{
			_vehicleManager.SetActiveVehicle(type);
			UpdateUI();
		}
		else
		{
			// ✅ DODANE - Debug przed zakupem
			float moneyBefore = _playerMoney.GetMoney();
			GD.Print($"🛒 SKLEP PRZED: Pieniądze={moneyBefore}$, Cena={data.Price}$");
			
			if (_playerMoney.SpendMoney(data.Price))
			{
				// ✅ DODANE - Debug po zakupie
				float moneyAfter = _playerMoney.GetMoney();
				GD.Print($"🛒 SKLEP PO: Pieniądze={moneyAfter}$, Różnica={moneyBefore - moneyAfter}$");
				
				_vehicleManager.PurchaseVehicle(type);
				_vehicleManager.SetActiveVehicle(type);
				
				// ✅ NAPRAWIONE - Wymuś aktualizację HUD
				if (_moneyHUD != null)
				{
					_moneyHUD.ForceUpdate();
					GD.Print("💰 HUD zaktualizowany po zakupie!");
				}
				
				UpdateUI();
			}
			else
			{
				GD.Print("❌ Zakup nieudany - brak środków!");
			}
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			EmitSignal(SignalName.ShopClosed);
			GetViewport().SetInputAsHandled();
		}
	}
	
	public void OnShopOpened()
	{
		_playerMoney = PlayerMoney.Instance;
		
		// ✅ NAPRAWIONE - Wymuś aktualizację HUD przy otwieraniu
		if (_moneyHUD != null)
		{
			_moneyHUD.ForceUpdate();
		}
		
		UpdateUI();
	}
}
