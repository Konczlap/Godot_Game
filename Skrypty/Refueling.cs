using Godot;

public partial class Refueling : Area2D
{
	[Export] private float _fuelPrice = 0.1f; // koszt 0.1 golda za jednostkę paliwa
	[Export] private float _refuelRate = 5f;  // ile paliwa wlewa się na sekundę

	[Export] private PlayerMoney _playerMoney;
	private Gas _gas;
	private MovementScript _movementScript;
	private MessageHUD _messageHUD;
	private bool _canRefuel = false;

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;
		_messageHUD = GetTree().Root.GetNode<MessageHUD>("Node2D/MessageHUD");
		//// Szukamy referencji (zakładamy, że są w tym samym drzewie lub wyżej)
		//_playerMoney = GetTree().Root.GetNode<PlayerMoney>("/root/PlayerMoney");
	}

	public override void _Process(double delta)
	{
		if (!_canRefuel || _gas == null || _movementScript == null)
		{
			_movementScript?.StopRefuel();
			return;
		}

		bool wantsToRefuel =
			Input.IsActionPressed("action") &&
			_movementScript.GetIsStanding();

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

				if (_gas.GetFuel() >= _gas.GetMaxFuel())
				{
					//GetNodeOrNull<HintTutorial>("/root/Node2D/HintHUD")?.OnFuelFilled();
					_canRefuel = false;
				}
			}
			else
			{
				GD.Print("❌ Brak pieniędzy na paliwo!");
			}
		}
	}

	public void OnAreaEntered(Area2D area)
	{
		if (area.GetParent().IsInGroup("Player"))
		{
			//GD.Print("Wykryto pojazd na stacji paliw");
			//GetNodeOrNull<HintTutorial>("/root/Node2D/HintHUD")?.OnFuelStationReached();
			Node car = area.GetParent();
			Node player = car.GetParent();
			//GD.Print($"{player.Name}");
			
			_movementScript = player.GetNodeOrNull<MovementScript>(".");
			_gas = car.GetNodeOrNull<Gas>("Gas");
			_playerMoney = car.GetNodeOrNull<PlayerMoney>("PlayerMoney");
			_playerMoney.ZeroingSpendMoney();

			// Sprawdzamy, czy gracz/pojazd ma potrzebne komponenty
			if (_movementScript != null && _gas != null && _playerMoney != null)
			{
				_canRefuel = true;
				_messageHUD?.ShowMessage("Przytrzymaj E, aby zatankować samochód", new Color("#FFFFFF"));
			}
			else
			{
				//GD.PrintErr("⚠️ Nie znaleziono któregoś z komponentów (MovementScript / Gas / PlayerMoney)");
			}
		}
	}

	public void OnAreaExited(Area2D area)
	{
		if (area.GetParent().IsInGroup("Player"))
		{
			_movementScript?.StopRefuel();
			
			_canRefuel = false;
			_gas = null;
			_movementScript = null;
			_playerMoney = null;
			_messageHUD?.HideMessage();
		}
	}
}
