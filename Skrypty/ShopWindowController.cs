using Godot;
using System;

public partial class ShopWindowController : Panel
{
	[Export]
	public NodePath CloseButtonPath = new NodePath("VBoxMain/TopBar/CloseButton");

	[Export]
	public NodePath MinimapPath = new NodePath("HUD/MiniMap");

	private Button _closeButton;
	private Control _minimap;

	public override void _Ready()
	{
		Visible = false; // ukryj sklep na starcie

		// znajdź przycisk X
		_closeButton = GetNodeOrNull<Button>(CloseButtonPath);
		if (_closeButton != null)
		{
			_closeButton.Pressed += OnClosePressed;
		}
		else
		{
			GD.PushError("ShopWindowController: CloseButton not found at " + CloseButtonPath);
		}

		// znajdź mini-mapę
		_minimap = GetNodeOrNull<Control>(MinimapPath);
		if (_minimap == null)
		{
			GD.PushWarning("ShopWindowController: Minimap not found at " + MinimapPath);
		}
	}

	private void OnClosePressed()
	{
		CloseShop();
	}

	public override void _Process(double delta)
	{
		// Zamknięcie przez X lub Escape
		if (Visible && (Input.IsKeyPressed(Key.X) || Input.IsKeyPressed(Key.Escape)))
		{
			CloseShop();
		}
	}

	public void OpenShop()
	{
		Visible = true;

		if (_minimap != null)
			_minimap.Visible = false;

		// Zatrzymaj grę
		GetTree().Paused = true;
		GD.Print("Shop opened (tree paused, minimap hidden)");
	}

	public void CloseShop()
	{
		Visible = false;

		if (_minimap != null)
			_minimap.Visible = true;

		GetTree().Paused = false;
		GD.Print("Shop closed (tree resumed, minimap visible)");
	}
}
