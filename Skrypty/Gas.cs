// Gas.cs
using Godot;

public partial class Gas : Node2D
{
	[Export] private float _maxFuel = 100f;
	private float _currentFuel = 100f;
	public float _fuelConsumptionRate = 2f;
	[Export] private Sprite2D _fuelStationSpawn;
	[Export] public MovementScript _movementScript;
	[Export] public DayNightCycle _dayNightCycle;
	[Export] public FuelWarningHUD FuelHUD;
	[Export] public PlayerMoney PlayerMoney;
	
	private VehicleManager _vehicleManager;
	public bool _isTeleporting = false;
	
	public override void _Ready()
	{
		_vehicleManager = VehicleManager.Instance;
		UpdateFuelConsumption();
	}
	
	public void UpdateFuelConsumption()
	{
		if (_vehicleManager != null)
		{
			var vehicleData = _vehicleManager.GetActiveVehicleData();
			_fuelConsumptionRate = vehicleData.FuelConsumption;
			GD.Print($"⛽ Gas: Spalanie ustawione na {_fuelConsumptionRate} L/s");
		}
	}
	
	public override async void _Process(double delta)
	{
		if (_movementScript == null)
			return;
		
		if (_dayNightCycle.IsSummaryOpen)
		{
			_movementScript.CanMove = false;
			return;
		}
		
		if (!_movementScript.GetIsStanding())
		{
			_currentFuel -= _fuelConsumptionRate * (float)delta;
			_currentFuel = Mathf.Max(_currentFuel, 0f);
		}
		
		if (_currentFuel <= 0f)
		{
			_currentFuel = 0f;
			_movementScript.CanMove = false;
			
			if (FuelHUD == null || PlayerMoney == null)
			{
				_isTeleporting = true;
				await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);
				TeleportToFuelStation();
				return;
			}
			
			if (PlayerMoney.GetMoney() < 75f)
			{
				FuelHUD.ShowFailureMessage();
				return;
			}
			
			FuelHUD.ShowTowMessage();
			_isTeleporting = true;
			await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);
			TeleportToFuelStation();
			return;
		}
	}
	
	public void AddFuel(float amount)
	{
		_currentFuel = Mathf.Clamp(_currentFuel + amount, 0f, _maxFuel);
		GD.Print($"⛽ Zatankowano {amount}L. Stan baku: {_currentFuel:F1}/{_maxFuel}");
	}
	
	public void TeleportToFuelStation()
	{
		if(_isTeleporting)
		{
			_isTeleporting = false;
			Node2D player = GetParent<Node2D>().GetParent<Node2D>();
			player.GlobalPosition = _fuelStationSpawn.GlobalPosition;
			player.GlobalRotation = -90f;
		}
	}
	
	public float GetFuel()
	{
		return _currentFuel;
	}
	
	public void SetFuel(float value)
	{
		_currentFuel = value;
	}
	
	public float GetMaxFuel()
	{
		return _maxFuel;
	}
}
