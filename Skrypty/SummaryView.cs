using Godot;
using System;

public partial class SummaryView : CanvasLayer
{
	[Export] private DayNightCycle _dayNightCycle; 
	[Export] private Delivery _delivery;
	[Export] private PlayerMoney _playerMoney;

	private Label _titleLabel;
	private Label _summaryLabel;

	public override void _Ready()
	{
		_titleLabel = GetNode<Label>("Control/PodsumowaniePanel/TytułLabel");
		_summaryLabel = GetNode<Label>("Control/PodsumowaniePanel/PodsumowanieLabel");
	}

	/// <summary>
	/// Funkcja do wywołania Gdy został zakończony dzień z DayNightCycle.EndDay()
	/// </summary>
	public void UpdateSummary()
	{
		int day = _dayNightCycle.GetDayNumber();
		int delivered = _delivery.DeliveredPackagesPerDay;
		int maxPackages = 17;  // jeśli stała, można wynieść do export
		float earned = _playerMoney.IncomePerDay;
		float spent = _playerMoney.SpendPerDay;
		float balance = earned - spent;

		// Ustawianie tekstów w panelu
		_titleLabel.Text = $"Podsumowanie dnia {day}";
		_summaryLabel.Text = $"Dostarczono {delivered} paczek na {maxPackages}\nZarobiono: {earned:0.##}$\nWydano: {spent:0.##}$\nBilans: {earned:0.##} - {spent:0.##} = {balance:0.##}$";
	}
}
