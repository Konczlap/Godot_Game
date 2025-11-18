using Godot;
using System;
using System.Collections.Generic;

public partial class PackageHUD : CanvasLayer
{
	[Export] public Delivery DeliveryScript;   // podłącz node Delivery w Inspectorze
	[Export] public Texture2D PackageIcon;     // przypnij res://assets/Package.png

	private List<TextureRect> _icons = new List<TextureRect>();
	private int _prevCount = 0;

	public override void _Ready()
	{
		// Pobierz referencje do ikon w slotach (Slot0..Slot9/Icon)
		for (int i = 0; i < 10; i++)
		{
			string path = $"PackageHUDControl/BarBackground/Slots/Slot{i}/Icon";
			if (!HasNode(path))
			{
				GD.PrintErr($"PackageHUD: nie znaleziono node'a {path}");
				_icons.Add(null);
				continue;
			}

			var icon = GetNode<TextureRect>(path);
			icon.Texture = null;
			icon.Visible = false;
			_icons.Add(icon);
		}

		if (DeliveryScript == null)
			GD.PushWarning("PackageHUD: DeliveryScript nie podłączony w Inspectorze.");

		_prevCount = DeliveryScript != null ? DeliveryScript.CurrentPackageAmount : 0;

		// ustaw istniejące ikony od razu (np. przy wczytaniu save)
		UpdateIcons();
	}

	public override void _Process(double delta)
	{
		if (DeliveryScript == null) return;

		int current = DeliveryScript.CurrentPackageAmount;
		if (current != _prevCount)
		{
			UpdateIcons();
			_prevCount = current;
		}
	}

	private void UpdateIcons()
	{
		int current = DeliveryScript.CurrentPackageAmount;

		// wypełnij wszystkie zajęte sloty
		for (int i = 0; i < _icons.Count; i++)
		{
			var icon = _icons[i];
			if (icon == null) continue;

			if (i < current)
			{
				icon.Texture = PackageIcon;
				icon.Visible = true;
			}
			else
			{
				icon.Texture = null;
				icon.Visible = false;
			}
		}
	}
}
