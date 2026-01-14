using Godot;
using System;

public partial class Parking : Area2D
{
	[Export] private ShopUI shopUI;
	private bool _playerInside = false;
	private MessageHUD _messageHUD;

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;
		
		_messageHUD = GetTree().Root.GetNode<MessageHUD>("Node2D/MessageHUD");

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
			
			_messageHUD?.HideMessage();
			//GetNodeOrNull<HintTutorial>("/root/Node2D/HintHUD")?.OnShopOpened();
		}
		
		if(!shopUI.IsOpen)
		{
			_messageHUD?.ShowMessage("Naciśnij E, aby wejść do sklepu", new Color("#FFFFFF"));
		}
	}
	
	private void OnAreaEntered(Area2D area)
	{
		if (!area.GetParent().IsInGroup("Player"))
			return;

		_playerInside = true;

		_messageHUD?.ShowMessage("Naciśnij E, aby wejść do sklepu", new Color("#FFFFFF"));
	}

	private void OnAreaExited(Area2D area)
	{
		if (!area.GetParent().IsInGroup("Player"))
			return;

		_playerInside = false;

		_messageHUD?.HideMessage();

		if (shopUI != null && shopUI.IsOpen)
			shopUI.CloseShop();
	}
}
