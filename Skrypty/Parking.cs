using Godot;
using System;

public partial class Parking : Area2D
{
	[Export]
	public NodePath UiLabelPath { get; set; } = null;
	
	[Export]
	public string PromptText { get; set; } = "Kliknij E";
	
	[Export]
	public PackedScene ShopScene { get; set; } = null;
	
	private Label _uiLabel;
	private bool _playerInArea = false;
	private ShopUI _shopInstance = null;
	private CanvasLayer _shopCanvasLayer = null;

	public override void _Ready()
	{
		GD.Print($"[Parking] Inicjalizacja węzła: {Name}");
		
		if (UiLabelPath != null)
		{
			_uiLabel = GetNode<Label>(UiLabelPath);
			if (_uiLabel != null)
			{
				_uiLabel.Visible = false;
				_uiLabel.Text = PromptText;
				GD.Print($"[Parking] Label znaleziony: {_uiLabel.Name}");
			}
			else
			{
				GD.PrintErr("[Parking] Nie można znaleźć Label na ścieżce: " + UiLabelPath);
			}
		}
		else
		{
			GD.PrintErr("[Parking] UiLabelPath nie został ustawiony w inspektorze!");
		}

		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
		
		GD.Print("[Parking] Sygnały podłączone.");
	}

	private void OnBodyEntered(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			_playerInArea = true;
			if (_uiLabel != null)
			{
				_uiLabel.Visible = true;
				GD.Print("[Parking] ✓ Gracz wszedł na parking - Label widoczny!");
			}
		}
	}

	private void OnBodyExited(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			_playerInArea = false;
			if (_uiLabel != null)
			{
				_uiLabel.Visible = false;
				GD.Print("[Parking] ✓ Gracz opuścił parking - Label ukryty.");
			}
		}
	}

	public override void _Process(double delta)
	{
		// Debug info
		if (_playerInArea && _shopInstance == null)
		{
			GD.Print($"[Parking] Czekam na E. Sklep: {(_shopInstance != null ? "otwarty" : "zamknięty")}");
		}
		
		// Otwórz sklep na E
		if (_playerInArea && Input.IsActionJustPressed("action") && _shopInstance == null)
		{
			GD.Print("[Parking] E naciśnięte - otwieram sklep!");
			OpenShop();
		}
	}

	private void OpenShop()
	{
		if (ShopScene == null)
		{
			GD.PrintErr("[Parking] ShopScene nie został przypisany w inspektorze!");
			return;
		}

		if (_uiLabel != null)
			_uiLabel.Visible = false;

		// Utwórz CanvasLayer
		_shopCanvasLayer = new CanvasLayer();
		_shopCanvasLayer.Layer = 100;
		GetTree().Root.AddChild(_shopCanvasLayer);
		
		// Utwórz sklep
		_shopInstance = ShopScene.Instantiate<ShopUI>();
		_shopCanvasLayer.AddChild(_shopInstance);
		
		// WAŻNE: Podłącz sygnał zamknięcia
		_shopInstance.ShopClosed += OnShopClosed;
		
		// Konfiguracja
		_shopInstance.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		_shopInstance.Position = Vector2.Zero;
		
		GetTree().Paused = true;
		_shopCanvasLayer.ProcessMode = ProcessModeEnum.WhenPaused;
		_shopInstance.ProcessMode = ProcessModeEnum.WhenPaused;
		
		GD.Print("[Parking] ✓ Sklep otwarty.");
	}

	private void OnShopClosed()
	{
		GD.Print("[Parking] Sygnał ShopClosed otrzymany!");
		
		if (_shopCanvasLayer != null && IsInstanceValid(_shopCanvasLayer))
		{
			_shopCanvasLayer.QueueFree();
		}
		
		// KRYTYCZNE: Wyzeruj referencje
		_shopCanvasLayer = null;
		_shopInstance = null;
		
		// Wznów grę
		GetTree().Paused = false;
		
		// Pokaż prompt jeśli gracz wciąż w obszarze
		if (_playerInArea && _uiLabel != null)
			_uiLabel.Visible = true;
		
		GD.Print("[Parking] ✓ Sklep zamknięty, flagi zresetowane.");
	}
}
