using Godot;
using System;

public partial class DayNightCycle : Node2D
{
	[Export] public float RealSecondsPerGameMinute = 1f; // tempo czasu
	//[Export] public Light2D GlobalLight;                 // globalne światło
	[Export] public Node2D CustomersContainer;
	[Export] public Node2D PackagesContainer;
	[Export] public /*CanvasLayer*/ SummaryView SummaryMenu;
	[Export] public CanvasModulate GlobalModulate;
	[Export] private MovementScript _movementScript;
	[Export] private Delivery _delivery;
	[Export] private PlayerMoney _playerMoney;
	[Export] private PackageHUD _packageHUD;
	[Export] private SpawnManager _spawnManager;
	public bool IsSummaryOpen = false;
	
	private float _currentMinutes = 6 * 60; // start: 6:00
	public bool IsNight => GetTimeHour() >= 22 || GetTimeHour() < 6;
	private int _dayNumber = 1;

	public override void _Ready()
	{
		SummaryMenu.Visible = false;
	}

	public override void _Process(double delta)
	{
		UpdateTime(delta);
		UpdateLight();
		HandleNightTransition();
	}

	private void UpdateTime(double delta)
	{
		_currentMinutes += (float)delta / RealSecondsPerGameMinute;

		if (_currentMinutes >= 22 * 60)
		{
			//_currentMinutes = 6 * 60; // następny dzień od 6:00
			_currentMinutes = 22 * 60;
		}
	}

	public int GetTimeHour() => (int)(_currentMinutes / 60f);
	public int GetTimeMinute() => (int)(_currentMinutes % 60f);

	private void UpdateLight()
	{
		int hour = GetTimeHour();
		int minutes = GetTimeMinute();
		//GD.Print($"Czas: {hour}:{minutes}");

		Color dayColor = new Color(1, 1, 1);     // pełna jasność
		Color nightColor = new Color(0.25f, 0.25f, 0.35f); // ciemno z lekkim niebieskim

		Color target;

		if (hour >= 6 && hour < 20)
			target = dayColor;
		else
			target = nightColor;

		GlobalModulate.Color = GlobalModulate.Color.Lerp(target, 0.02f);
	}

	private void HandleNightTransition()
	{
		if (GetTimeHour() == 22 && CustomersContainer.Visible)
		{
			CustomersContainer.Visible = false;
			PackagesContainer.Visible = false;
		}
	}

	public void EndDay()
	{
		IsSummaryOpen = true;
		_movementScript.CanMove = false;
		SummaryMenu.Visible = true;
		SummaryMenu.UpdateSummary();
	}

	public void StartNextDay()
	{
		SummaryMenu.Visible = false;
		IsSummaryOpen = false;
		CustomersContainer.Visible = true; // TRZEBA PRZEJŚĆ PRZEZ WSZYSTKIE DZIECI I JE WŁĄCZYĆ BO
		PackagesContainer.Visible = true; // TAKIE WŁĄCZENIE NIE WŁĄCZY TYCH KTÓRE ZOSTAŁY WEWNĄTRZ NICH WYŁĄCZONE
		_spawnManager.RandomizeSpawn();
		foreach (Node child in PackagesContainer.GetChildren())
		{
			if (child is Node2D item)
			{
				//GD.Print("Paczki robią się widoczne");
				item.Visible = true;
				var area = item.GetNodeOrNull<Area2D>("Area2D");
				if (area != null)
				{
					area.Monitoring = true;
					area.Monitorable = true;
				}
			}
			else
			{
				//GD.Print("To nie działa :(");
			}
		}
		_delivery.CurrentPackageAmount = 0;
		_delivery.DeliveredPackagesPerDay = 0;
		_playerMoney.IncomePerDay = 0;
		_playerMoney.SpendPerDay = 0;
		//_delivery.ResetPackages();
		_packageHUD.UpdateIcons();
		
		_dayNumber++;
		_movementScript.CanMove = true;
		_currentMinutes = 6 * 60; // 6:00 rano
	}
	
	public int GetDayNumber()
	{
		return _dayNumber;
	}
}
