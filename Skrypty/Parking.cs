using Godot;
using System;

public partial class Parking : Area2D
{
	[Export] private ShopUI shopUI;
	[Export] private Label hintLabel;   // "[E] SKLEP MOTORYZACYJNY"

	private bool _playerInside = false;

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;

		if (hintLabel != null)
			hintLabel.Visible = false;

		if (shopUI != null)
			shopUI.HideShop();
	}

	public override void _Process(double delta)
	{
		if (!_playerInside)
			return;

		if (Input.IsActionJustPressed("action")) // E
		{
			if (shopUI != null)
				shopUI.OpenShop();
			
			GD.Print("Wjechałeś do sklepu");
		}
	}

	private void OnAreaEntered(Area2D area)
	{
		if (!area.GetParent().IsInGroup("Player"))
			return;

		_playerInside = true;

		if (hintLabel != null)
			hintLabel.Visible = true;

		GD.Print("🅿️ Możesz wejść do sklepu (E)");
	}

	private void OnAreaExited(Area2D area)
	{
		if (!area.GetParent().IsInGroup("Player"))
			return;

		_playerInside = false;

		if (hintLabel != null)
			hintLabel.Visible = false;

		if (shopUI != null && shopUI.IsOpen)
			shopUI.CloseShop();

		GD.Print("🚗 Opuściłeś parking sklepu");
	}
}
