using Godot;

public partial class Gas : Node2D
{
	[Export] private float _maxFuel = 100f;        // Maksymalny poziom paliwa
	[Export] private float _currentFuel = 100f;    // Aktualny poziom paliwa
	[Export] private float _fuelConsumptionRate = 2f; // Ile paliwa ubywa na sekundę podczas jazdy
	[Export] private Sprite2D _fuelStationSpawn; // np. pozycja stacji paliw

	[Export] public MovementScript _movementScript;
	[Export] public DayNightCycle _dayNightCycle;
	[Export] public FuelWarningHUD FuelHUD; // podłącz w Inspectorze naszą scenę FuelWarningHUD
	[Export] public PlayerMoney PlayerMoney; // podłącz PlayerMoney node

	public bool _isTeleporting = false;

	public override async void _Process(double delta)
	{
		if (_movementScript == null)
			return;
		
		if (_dayNightCycle.IsSummaryOpen)
		{
			_movementScript.CanMove = false;
			return;
		}

		// Jeśli auto się rusza, spalaj paliwo
		if (!_movementScript.GetIsStanding())
		{
			_currentFuel -= _fuelConsumptionRate * (float)delta;
			_currentFuel = Mathf.Max(_currentFuel, 0f);

			//GD.Print($"⛽ Paliwo: {_currentFuel:0.0}/{_maxFuel}");
		}

		// Gdy paliwo się skończy — pokaż odpowiednie okienko (przejęcie kontroli przez HUD)
		if (_currentFuel <= 0f)
		{
			_currentFuel = 0f;
			_movementScript.CanMove = false;

			// Jeśli nie mamy podłączonego HUDu / systemu pieniędzy — fallback: teleportuj
			if (FuelHUD == null || PlayerMoney == null)
			{
				_isTeleporting = true;
				await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);
				TeleportToFuelStation();
				return;
			}

			// brak środków -> pokaż porażkę
			if (PlayerMoney.GetMoney() < 75f)
			{
				FuelHUD.ShowFailureMessage();
				return;
			}

			// mamy środki -> pokaż komunikat o holowaniu (HUD przejmie wykonanie)
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
		//GD.Print($"⛽ Zatankowano {amount}L. Stan baku: {_currentFuel}/{_maxFuel}");
	}
	
	public void TeleportToFuelStation()
{
	//if (_fuelStationSpawn == null)
	//{
		//GD.PrintErr("⚠️ Nie ustawiono punktu stacji paliw (FuelStationSpawn)!");
		//return;
	//}
	if(_isTeleporting)
	{
		_isTeleporting = false;
		Node2D player = GetParent<Node2D>().GetParent<Node2D>();
		player.GlobalPosition = _fuelStationSpawn.GlobalPosition;
		player.GlobalRotation = -90f;

		//GD.Print("⛽ Teleportowano na stację paliw.");
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
	
	public void SetFuelConsumptionRate(float value)
	{
		_fuelConsumptionRate = value;
	}
}
