using Godot;
using System;

public partial class FuelWarningHUD : CanvasLayer
{
	// PODŁĄCZ W INSPECTORZE
	[Export] public MovementScript Movement;   // movement script (blokowanie ruchu)
	[Export] public Gas GasScript;             // skrypt paliwa
	[Export] public PlayerMoney PlayerMoney;   // system pieniędzy
	[Export] public Delivery Delivery;         // opcjonalnie: reset paczek przy reload
	[Export] public DayNightCycle DayNight;    // do restartu dnia

	private Label _title;
	private Label _note;
	private Button _reloadButton;
	private Button _confirmButton;

	public override void _Ready()
	{
		_title = GetNode<Label>("CenterContainer/MessageBox/Vertical/TitleLabel");
		_note = GetNode<Label>("CenterContainer/MessageBox/Vertical/NoteLabel");
		_reloadButton = GetNode<Button>("CenterContainer/MessageBox/Vertical/ButtonsBox/ReloadButton");
		_confirmButton = GetNode<Button>("CenterContainer/MessageBox/Vertical/ButtonsBox/ConfirmButton");

		// Podłączamy eventy
		_reloadButton.Pressed += OnReloadPressed;
		_confirmButton.Pressed += OnConfirmPressed;

		Visible = false;
	}

	// Pokazujemy czerwone okno "PORAŻKA" (brak pieniędzy)
	public void ShowFailureMessage()
	{
		Visible = true;
		_title.Text = "PORAŻKA";
		_title.Modulate = new Color(1f, 0f, 0f); // czerwony
		_note.Text = "NOTE:\nZabrakło ci paliwa i nie masz pieniędzy by zatankować swój pojazd.";
		_reloadButton.Visible = true;
		_confirmButton.Visible = false;

		if (Movement != null) Movement.CanMove = false;
	}

	// Pokazujemy pomarańczowe okno "KOMUNIKAT" (ma pieniądze -> holowanie)
	public void ShowTowMessage()
	{
		Visible = true;
		_title.Text = "KOMUNIKAT";
		_title.Modulate = new Color(1f, 0.5f, 0f); // pomarańczowy
		_note.Text = "NOTE:\nZabrakło ci paliwa, twoje auto zostało przeholowane na stację paliw,\nCena holowania 75 zł.";
		_reloadButton.Visible = false;
		_confirmButton.Visible = true;

		if (Movement != null) Movement.CanMove = false;
	}

	// Wczytaj save (działa jak PauseMenu.OnRestartButtonPressed)
	private void OnReloadPressed()
	{
		var sm = GetNodeOrNull<SaveManager>("/root/SaveManager");
		if (sm == null)
		{
			GD.PrintErr("SaveManager singleton nie znaleziony!");
			return;
		}

		// Wczytaj
		if (!sm.LoadSave())
		{
			GD.PrintErr("Błąd wczytywania save.");
			return;
		}

		if (Movement != null)
		{
			Movement.SetCurrentSpeed(0f);
			Movement.GlobalPosition = sm.PlayerPosition;
		}

		if (GasScript != null)
			GasScript.SetFuel(sm.Fuel);

		if (PlayerMoney != null)
			PlayerMoney.SetMoney(sm.Money);

		if (DayNight != null)
		{
			DayNight.SetDayNumber(sm.Day);
			DayNight.RestartDay();
		}

		if (Delivery != null)
			Delivery.ResetPackages();

		Visible = false;
		if (Movement != null) Movement.CanMove = true;
	}

	// Potwierdzenie holowania: pobieramy kasę, ustawiamy paliwo, teleportujemy
	private void OnConfirmPressed()
	{
		if (PlayerMoney != null)
		{
			bool ok = PlayerMoney.SpendMoney(75f);
			if (!ok)
			{
				GD.PrintErr("FuelWarningHUD: nie udało się pobrać 75 zł (brak środków) - nie powinno się zdarzyć.");
			}
		}

		if (GasScript != null)
			GasScript.SetFuel(0.00001f); // ustaw paliwo po holowaniu

		// Wywołujemy teleport (na Gas zmienimy metodę na publiczną)
		if (GasScript != null)
			GasScript.TeleportToFuelStation();

		Visible = false;
		if (Movement != null) Movement.CanMove = true;
	}
	
	public void HideCommunicate()
	{
		Visible = false;
	}
	
	public void ShowCommunicate()
	{
		Visible = true;
	}
}
