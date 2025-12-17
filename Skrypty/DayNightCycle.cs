// DayNightCycle.cs
using Godot;
using System;

public partial class DayNightCycle : Node2D
{
	[Export] public float RealSecondsPerGameMinute = 1f;
	[Export] public Node2D CustomersContainer;
	[Export] public Node2D PackagesContainer;
	[Export] public SummaryView SummaryMenu;
	[Export] public CanvasModulate GlobalModulate;
	[Export] private AudioStreamPlayer _cityAmbient;
	[Export] private MovementScript _movementScript;
	[Export] private Delivery _delivery;
	[Export] private PlayerMoney _playerMoney;
	[Export] private PackageHUD _packageHUD;
	[Export] private SpawnManager _spawnManager;
	public bool IsSummaryOpen = false;
	
	private float _currentMinutes = 6 * 60;
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
		UpdateCityAmbient(delta);
	}

	private void UpdateTime(double delta)
	{
		_currentMinutes += (float)delta / RealSecondsPerGameMinute;

		if (_currentMinutes >= 22 * 60)
		{
			_currentMinutes = 22 * 60;
		}
	}

	public int GetTimeHour() => (int)(_currentMinutes / 60f);
	public int GetTimeMinute() => (int)(_currentMinutes % 60f);

	private void UpdateLight()
	{
		int hour = GetTimeHour();

		Color dayColor = new Color(1, 1, 1);
		Color nightColor = new Color(0.25f, 0.25f, 0.35f);

		Color target;

		if (hour >= 6 && hour < 20)
			target = dayColor;
		else
			target = nightColor;

		GlobalModulate.Color = GlobalModulate.Color.Lerp(target, 0.02f);
	}
	
	private void UpdateCityAmbient(double delta)
	{
		if (_cityAmbient == null)
			return;

		float dayVolume = -20f;
		float nightVolume = -35f;
		float fadeSpeed = 10f;

		int hour = GetTimeHour();
		int minute = GetTimeMinute();

		float targetVolume;

		if (hour >= 6 && hour < 18)
		{
			targetVolume = dayVolume;
		}
		else if (hour >= 18 && hour < 22)
		{
			float t = (hour * 60 + minute - 18 * 60) / (float)(4 * 60);
			targetVolume = Mathf.Lerp(dayVolume, nightVolume, t);
		}
		else
		{
			targetVolume = nightVolume;
		}

		_cityAmbient.VolumeDb = Mathf.MoveToward(
			_cityAmbient.VolumeDb,
			targetVolume,
			fadeSpeed * (float)delta
		);
	}

	private void HandleNightTransition()
	{
		if (GetTimeHour() == 22 && CustomersContainer.Visible)
		{
			CustomersContainer.Visible = false;
			PackagesContainer.Visible = false;
			
			var minimap = GetTree().Root.GetNodeOrNull<Minimap>("Node2D/Minimap");
			if (minimap != null)
			{
				minimap.ClearTarget();
			}
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
		CustomersContainer.Visible = true;
		PackagesContainer.Visible = true;
		_spawnManager.RandomizeSpawn();
		
		foreach (Node child in PackagesContainer.GetChildren())
		{
			if (child is Node2D item)
			{
				item.Visible = true;
				var area = item.GetNodeOrNull<Area2D>("Area2D");
				if (area != null)
				{
					area.Monitoring = true;
					area.Monitorable = true;
				}
			}
		}
		
		_delivery.CurrentPackageAmount = 0;
		_delivery.DeliveredPackagesPerDay = 0;
		_playerMoney.ZeroingIncomePerDay();
		_playerMoney.ZeroingSpendPerDay();
		_packageHUD.UpdateIcons();
		
		_delivery.UpdateMinimapAfterReset();
		
		_dayNumber++;
		_movementScript.CanMove = true;
		_currentMinutes = 6 * 60;
	}
	
	public void RestartDay()
	{
		CustomersContainer.Visible = true;
		PackagesContainer.Visible = true;
		
		foreach (Node child in PackagesContainer.GetChildren())
		{
			if (child is Node2D item)
			{
				item.Visible = true;
				var area = item.GetNodeOrNull<Area2D>("Area2D");
				if (area != null)
				{
					area.Monitoring = true;
					area.Monitorable = true;
				}
			}
		}
		
		_delivery.CurrentPackageAmount = 0;
		_delivery.DeliveredPackagesPerDay = 0;
		_playerMoney.ZeroingIncomePerDay();
		_playerMoney.ZeroingSpendPerDay();
		_packageHUD.UpdateIcons();
		
		_delivery.UpdateMinimapAfterReset();
		
		_movementScript.CanMove = true;
		_currentMinutes = 6 * 60;
	}
	
	public int GetDayNumber()
	{
		return _dayNumber;
	}
	
	public void SetDayNumber(int value)
	{
		_dayNumber = value;
	}
	
	public void TimeRestart()
	{
		_currentMinutes = 6 * 60;
	}
}
