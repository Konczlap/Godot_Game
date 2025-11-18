using Godot;
using System;

public partial class FuelHUD : CanvasLayer
{
	[Export] public Gas GasScript; // podłącz w Inspectorze swój node z Gas.cs

	[Export] private Color FullColor = new Color(1f, 0.9f, 0f); // żółty
	[Export] private Color MidColor  = new Color(1f, 0.5f, 0f); // pomarańczowy
	[Export] private Color LowColor  = new Color(1f, 0f, 0f);   // czerwony
	[Export] public float barMultiplier = 3.5f;

	private ColorRect _fuelFill;
	private Label _status;
	private Label _segments;
	private Label _vertical;

	public override void _Ready()
	{
		_fuelFill = GetNode<ColorRect>("FuelHUDControl/FuelBarContainer/FuelFill");
		_status = GetNode<Label>("FuelHUDControl/FuelStatusText");
		_segments = GetNode<Label>("FuelHUDControl/FuelSegmentsLabel");
		_vertical = GetNode<Label>("FuelHUDControl/FuelBarContainer/FuelVerticalLabel");
	}

	public override void _Process(double delta)
	{
		if (GasScript == null)
			return;

		UpdateFuelBar();
	}

	private void UpdateFuelBar()
	{
		float fuel = GasScript.GetFuel();
		float maxFuel = GasScript.GetMaxFuel();
		float percent = fuel / maxFuel;

		// ----- wysokość paska -----
		float fullHeight = _fuelFill.Size.Y; //.Control.AnchorPoints.Top; //.Size.Y; //.GetChild<Control>().Size.Y;
		float currentHeight = maxFuel * percent * barMultiplier;
		//float newHeight = fullHeight * percent;

		_fuelFill.Size = new Vector2(_fuelFill.Size.X, currentHeight);
		_fuelFill.Position = new Vector2(0, fullHeight - currentHeight);

		// ----- zmiana koloru -----
		Color col;
		if (percent > 0.5f)
			col = FullColor.Lerp(MidColor, 1 - (percent - 0.5f) * 2f);
		else
			col = MidColor.Lerp(LowColor, 1 - percent * 2f);
		_fuelFill.Color = col;

		// ----- komunikaty -----
		if (percent <= 0)
		{
			_status.Text = "Brak paliwa";
			_status.Modulate = Colors.Red;
		}
		else if (percent < 0.25f)
		{
			_status.Text = "Mało paliwa\nzatankuj\nsamochód";
			_status.Modulate = Colors.Orange;
		}
		else
		{
			_status.Text = "";
		}
	}
}
