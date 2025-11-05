using Godot;

public partial class Gas : Node2D
{
	[Export] private float _maxFuel = 100f;        // Maksymalny poziom paliwa
	[Export] private float _currentFuel = 100f;    // Aktualny poziom paliwa
	[Export] private float _fuelConsumptionRate = 2f; // Ile paliwa ubywa na sekundę podczas jazdy
	[Export] private Sprite2D _fuelStationSpawn; // np. pozycja stacji paliw

	[Export] public MovementScript _movementScript;

	public override async void _Process(double delta)
	{
		if (_movementScript == null)
			return;

		// Jeśli auto się rusza, spalaj paliwo
		if (!_movementScript.GetIsStanding())
		{
			_currentFuel -= _fuelConsumptionRate * (float)delta;
			_currentFuel = Mathf.Max(_currentFuel, 0f);

			GD.Print($"⛽ Paliwo: {_currentFuel:0.0}/{_maxFuel}");
		}

		// Gdy paliwo się skończy — można dodać reakcję (np. zatrzymanie auta)
		if (_currentFuel <= 0f)
		{
			_currentFuel = 0f;
			_movementScript.CanMove = false;
			GD.Print("🚫 Brak paliwa!");
			
			await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);
			TeleportToFuelStation();
		}
	}

	public void AddFuel(float amount)
	{
		_currentFuel = Mathf.Clamp(_currentFuel + amount, 0f, _maxFuel);
		GD.Print($"⛽ Zatankowano {amount}L. Stan baku: {_currentFuel}/{_maxFuel}");
	}
	
	private void TeleportToFuelStation()
{
	//if (_fuelStationSpawn == null)
	//{
		//GD.PrintErr("⚠️ Nie ustawiono punktu stacji paliw (FuelStationSpawn)!");
		//return;
	//}

	Node2D player = GetParent<Node2D>().GetParent<Node2D>();
	player.GlobalPosition = _fuelStationSpawn.GlobalPosition;
	player.Rotation = -90f;

	GD.Print("⛽ Teleportowano na stację paliw.");

	// Przy okazji można pozwolić znowu na ruch (np. po zatankowaniu)
	if (_currentFuel > 0f)
		_movementScript.CanMove = true;
}

	public float GetFuel()
	{
		return _currentFuel;
	}

	public float GetMaxFuel()
	{
		return _maxFuel;
	}
}
