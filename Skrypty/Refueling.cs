using Godot;

public partial class Refueling : Area2D
{
	[Export] private float _fuelPrice = 0.1f;
	[Export] private float _refuelRate = 5f;
	[Export] private PlayerMoney _playerMoney;
	
	private Gas _gas;
	private MovementScript _movementScript;
	private bool _canRefuel = false;
	private MoneyHUD _moneyHUD; // ✅ DODANE

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;
		
		// ✅ NAPRAWIONE - Poprawna ścieżka do MoneyHUD
		_moneyHUD = GetNode<MoneyHUD>("/root/Node2D/MoneyHUD");
		if (_moneyHUD == null)
		{
			GD.PrintErr("⚠️ Refueling: Nie znaleziono MoneyHUD!");
		}
	}

	public override void _Process(double delta)
	{
		if (!_canRefuel || _gas == null || _movementScript == null)
		{
			_movementScript?.StopRefuel();
			return;
		}

		bool wantsToRefuel = Input.IsActionPressed("action") && _movementScript.GetIsStanding();

		if (wantsToRefuel)
			_movementScript.StartRefuel();
		else
			_movementScript.StopRefuel();

		if (wantsToRefuel)
		{
			float fuelToAdd = _refuelRate * (float)delta;
			float cost = fuelToAdd * _fuelPrice;

			if (_playerMoney.GetMoney() >= cost)
			{
				_playerMoney.SpendMoney(cost);
				_gas.AddFuel(fuelToAdd);

				// ✅ NAPRAWIONE - Aktualizuj HUD po tankowaniu
				if (_moneyHUD != null)
				{
					_moneyHUD.ForceUpdate();
				}

				if (_gas.GetFuel() >= _gas.GetMaxFuel())
				{
					_canRefuel = false;
				}
			}
		}
	}

	public void OnAreaEntered(Area2D area)
	{
		var parent = area.GetParentOrNull<Node>();
		if (parent != null && parent.IsInGroup("Player"))
		{
			Node car = parent;
			Node player = car.GetParentOrNull<Node>();
			
			if (player != null)
			{
				_movementScript = player.GetNodeOrNull<MovementScript>(".");
				_gas = car.GetNodeOrNull<Gas>("Gas");
				_playerMoney = PlayerMoney.Instance;

				if (_movementScript != null && _gas != null && _playerMoney != null)
				{
					_canRefuel = true;
				}
			}
		}
	}

	public void OnAreaExited(Area2D area)
	{
		var parent = area.GetParentOrNull<Node>();
		if (parent != null && parent.IsInGroup("Player"))
		{
			_movementScript?.StopRefuel();
			_canRefuel = false;
			_gas = null;
			_movementScript = null;
			_playerMoney = null;
		}
	}
}
