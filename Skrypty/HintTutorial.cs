using Godot;

public partial class HintTutorial : CanvasLayer
{
	[Export] private Panel samouczekPanel;
	[Export] private Label titleLabel;
	[Export] private Label textLabel;
	[Export] private Button dalejButton;

	[Export] private Delivery delivery;
	[Export] private Gas gas;
	[Export] private DayNightCycle dayNight;
	[Export] private Minimap minimap;

	private int _step = 0;
	private bool _active = true;

	public override void _Ready()
	{
		// Sprawdź czy to pierwszy dzień
		if (dayNight.GetDayNumber() > 1)
		{
			DisableTutorial();
			return;
		}

		TimeScalePause(true);

		dalejButton.Pressed += OnNextClicked;

		ShowStep0();
	}

	// =========================
	// GŁÓWNA LOGIKA
	// =========================

	private void NextStep()
	{
		_step++;

		switch (_step)
		{
			case 1: ShowStep1(); break;
			case 2: ShowStep2(); break;
			case 3: ShowStep3(); break;
			case 4: ShowStep4(); break;
			case 5: ShowStep5(); break;
			case 6: ShowStep6(); break;
			case 7: ShowStep7(); break;
			case 8: ShowStep8(); break;
			case 9: EndTutorial(); break;
		}
	}

	// =========================
	// KROKI
	// =========================

	private void ShowStep0()
	{
		ShowPanel(
			"Witaj!",
			"Twoim celem jest dostarczanie paczek klientom.\n\n" +
			"Sterowanie:\n" +
			"WSAD / strzałki – jazda\n" +
			"Space – hamulec ręczny\n" +
			"E – interakcja\n" +
			"Esc – pauza\n" +
			"Kliknij DALEJ aby rozpocząć."
		);
	}

	private void ShowStep1()
	{
		TimeScalePause(false);
		ShowPanel(
			"Paczka",
			"Najedź na paczkę.\n" +
			"Gdy stoisz nad paczką, naciśnij E aby ją podnieść."
		);
	}

	private void ShowStep2()
	{
		ShowPanel(
			"Masz paczkę!",
			"Paczka zniknęła z mapy i pojawiła się w slocie w prawym dolnym rogu.\n\n" +
			"Strzałka na minimapie pokazuje klienta, do którego masz jechać."
		);
	}

	private void ShowStep3()
	{
		ShowPanel(
			"Klient",
			"Jedź do klienta.\n" +
			"Zatrzymaj się przy nim i naciśnij E."
		);
	}

	private void ShowStep4()
	{
		ShowPanel(
			"Dostawa",
			"Świetnie!\n" +
			"Teraz czas nauczyć się tankowania."
		);
	}

	private void ShowStep5()
	{
		ShowPanel(
			"Stacja paliw",
			"Jedź na stację paliw.\n" +
			"Zatrzymaj się w wyznaczonym miejscu (czerwony prostokąt)."
		);
	}

	private void ShowStep6()
	{
		ShowPanel(
			"Tankowanie",
			"Przytrzymaj E aby zatankować pojazd."
		);
	}

	private void ShowStep7()
	{
		ShowPanel(
			"Sklep",
			"Odwiedź sklep.\n" +
			"Opuść sklep z użyciem Esc.\n" +
			"Możesz tam kupować nowe pojazdy."
		);
	}
	
	private void ShowStep8()
	{
		ShowPanel(
			"Noc",
			"Ciemno robi się od 20:00,\n od 22:00 czas staje, a klienci i paczki znikają.\n Klikając F włączasz światła."
		);
		dayNight.SetNight();
	}

	private void ShowStep9()
	{
		ShowPanel(
			"Koniec dnia",
			"Wróć do domu, aby zakończyć pierwszy dzień."
		);
	}

	// =========================
	// ZAKOŃCZENIE
	// =========================

	private void EndTutorial()
	{
		ShowPanel(
			"Samouczek ukończony!",
			"Od teraz grasz już samodzielnie.\nPowodzenia!"
		);

		GetTree().CreateTimer(2).Timeout += () =>
		{
			DisableTutorial();
		};
	}

	private void DisableTutorial()
	{
		_active = false;
		samouczekPanel.Visible = false;

		// Zapisz że tutorial wykonany
		var sm = GetNodeOrNull<SaveManager>("/root/SaveManager");
		if (sm != null)
			return; //na razie
			//sm.TutorialDone = true;
	}

	// =========================
	// OBSŁUGA ZDARZEŃ
	// =========================

	private void OnNextClicked()
	{
		NextStep();
	}

	// Wywołuj z Delivery
	public void OnPackageTaken()
	{
		if (!_active || _step != 1) return;
		NextStep();
	}

	public void OnPackageDelivered()
	{
		if (!_active || _step != 3) return;
		NextStep();
	}

	public void OnFuelStationReached()
	{
		if (!_active || _step != 5) return;
		NextStep();
	}

	public void OnFuelFilled()
	{
		if (!_active || _step != 6) return;
		NextStep();
	}

	public void OnShopOpened()
	{
		if (!_active || _step != 7) return;
		NextStep();
	}
	
	public void NightReached()
	{
		if (!_active || _step != 8) return;
		NextStep();
	}

	public void OnHomeReached()
	{
		if (!_active || _step != 9) return;
		NextStep();
	}

	// =========================
	// UI
	// =========================

	private void ShowPanel(string title, string text)
	{
		samouczekPanel.Visible = true;
		titleLabel.Text = title;
		textLabel.Text = text;
	}

	private void TimeScalePause(bool pause)
	{
		Engine.TimeScale = pause ? 0f : 1f;
	}
}
