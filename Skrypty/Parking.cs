// Parking.cs
using Godot;
using System;

public partial class Parking : Area2D
{
	[Export] public PackedScene ShopScene { get; set; } = null;
	
	private Label _uiLabel;
	private Panel _uiPanel;
	private bool _playerInArea = false;
	private ShopUI _shopInstance = null;
	private CanvasLayer _shopCanvasLayer = null;

	public override void _Ready()
	{
		CreatePromptUI();
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void CreatePromptUI()
	{
		_uiPanel = new Panel();
		
		var styleBox = new StyleBoxFlat();
		styleBox.BgColor = new Color(0.1f, 0.1f, 0.15f, 0.85f);
		styleBox.BorderColor = new Color(1f, 0.8f, 0.2f, 1f);
		styleBox.SetBorderWidthAll(3);
		styleBox.SetCornerRadiusAll(12);
		
		_uiPanel.AddThemeStyleboxOverride("panel", styleBox);
		_uiPanel.AnchorLeft = 0.5f;
		_uiPanel.AnchorTop = 1.0f;
		_uiPanel.AnchorRight = 0.5f;
		_uiPanel.AnchorBottom = 1.0f;
		_uiPanel.OffsetLeft = -150;
		_uiPanel.OffsetTop = -120;
		_uiPanel.OffsetRight = 150;
		_uiPanel.OffsetBottom = -50;
		_uiPanel.ZIndex = 100;
		_uiPanel.Visible = false;
		
		_uiLabel = new Label();
		_uiLabel.Text = "🚗 [E] SKLEP MOTORYZACYJNY";
		_uiLabel.HorizontalAlignment = HorizontalAlignment.Center;
		_uiLabel.VerticalAlignment = VerticalAlignment.Center;
		_uiLabel.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		_uiLabel.AddThemeFontSizeOverride("font_size", 20);
		
		_uiPanel.AddChild(_uiLabel);
		
		// Użyj CallDeferred żeby uniknąć błędu
		var canvasLayer = new CanvasLayer();
		canvasLayer.Layer = 100;
		GetTree().CurrentScene.CallDeferred("add_child", canvasLayer);
		canvasLayer.CallDeferred("add_child", _uiPanel);
	}

	private void OnBodyEntered(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			_playerInArea = true;
			if (_uiPanel != null)
				_uiPanel.Visible = true;
		}
	}

	private void OnBodyExited(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			_playerInArea = false;
			if (_uiPanel != null)
				_uiPanel.Visible = false;
		}
	}

	public override void _Process(double delta)
	{
		if (_playerInArea && Input.IsActionJustPressed("action") && _shopInstance == null)
		{
			OpenShop();
		}
	}

	private void OpenShop()
	{
		if (ShopScene == null) return;

		if (_uiPanel != null)
			_uiPanel.Visible = false;

		_shopCanvasLayer = new CanvasLayer();
		_shopCanvasLayer.Layer = 100;
		GetTree().Root.AddChild(_shopCanvasLayer);
		
		_shopInstance = ShopScene.Instantiate<ShopUI>();
		_shopCanvasLayer.AddChild(_shopInstance);
		
		_shopInstance.ShopClosed += OnShopClosed;
		_shopInstance.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		_shopInstance.Position = Vector2.Zero;
		
		GetTree().Paused = true;
		_shopCanvasLayer.ProcessMode = ProcessModeEnum.WhenPaused;
		SetProcessModeRecursive(_shopInstance, ProcessModeEnum.WhenPaused);
	}

	private void SetProcessModeRecursive(Node node, ProcessModeEnum mode)
	{
		node.ProcessMode = mode;
		foreach (Node child in node.GetChildren())
		{
			SetProcessModeRecursive(child, mode);
		}
	}

	private void OnShopClosed()
	{
		if (_shopCanvasLayer != null && IsInstanceValid(_shopCanvasLayer))
		{
			_shopCanvasLayer.QueueFree();
		}
		
		_shopCanvasLayer = null;
		_shopInstance = null;
		GetTree().Paused = false;
		
		var player = GetTree().CurrentScene.GetNodeOrNull("Player");
		if (player != null)
		{
			player.GetNodeOrNull<Car>("Car")?.UpdateCarAppearance();
			player.GetNodeOrNull<MovementScript>(".")?.UpdateVehicleStats();
			player.GetNodeOrNull<Delivery>("Car/Delivery")?.UpdateMaxPackageAmount();
			player.GetNodeOrNull<Gas>("Car/Gas")?.UpdateFuelConsumption();
		}
		
		if (_playerInArea && _uiPanel != null)
			_uiPanel.Visible = true;
	}
}
