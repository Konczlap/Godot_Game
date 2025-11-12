// res://Skrypty/ShopTrigger.cs
using Godot;
using System;

public partial class ShopTrigger : Area2D
{
	[Export]
	public NodePath ShopWindowPath = new NodePath("HUD/ShopWindow");

	private ShopWindowController _shopWindow;
	private bool _playerInside = false;

	public override void _Ready()
	{
		// Spróbuj znaleźć panel sklepu (przez NodePath ustawiony w Inspectorze)
		_shopWindow = GetNodeOrNull<ShopWindowController>(ShopWindowPath);

		// Podłącz obie pary sygnałów: area_entered/area_exited oraz body_entered/body_exited
		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	// pomocniczna metoda: sprawdza, czy node lub dowolny jego przodek jest w grupie 'player'
	private bool NodeOrAncestorsInPlayerGroup(Node node)
	{
		Node cur = node;
		while (cur != null)
		{
			if (cur.IsInGroup("player"))
				return true;
			cur = cur.GetParent();
		}
		return false;
	}

	private void OnAreaEntered(Area2D area)
	{
		if (NodeOrAncestorsInPlayerGroup(area))
		{
			_playerInside = true;
			ShowHint();
			GD.Print("ShopTrigger: area_entered from player-area -> playerInside = true");
		}
	}

	private void OnAreaExited(Area2D area)
	{
		if (NodeOrAncestorsInPlayerGroup(area))
		{
			_playerInside = false;
			HideHint();
			GD.Print("ShopTrigger: area_exited from player-area -> playerInside = false");
		}
	}

	private void OnBodyEntered(Node body)
	{
		if (NodeOrAncestorsInPlayerGroup(body))
		{
			_playerInside = true;
			ShowHint();
			GD.Print("ShopTrigger: body_entered from player -> playerInside = true");
		}
	}

	private void OnBodyExited(Node body)
	{
		if (NodeOrAncestorsInPlayerGroup(body))
		{
			_playerInside = false;
			HideHint();
			GD.Print("ShopTrigger: body_exited from player -> playerInside = false");
		}
	}

	private void ShowHint()
	{
		var label = GetNodeOrNull<Label>("InteractLabel");
		if (label != null) label.Visible = true;
	}

	private void HideHint()
	{
		var label = GetNodeOrNull<Label>("InteractLabel");
		if (label != null) label.Visible = false;
	}

	public override void _Process(double delta)
	{
		if (_playerInside && Input.IsActionJustPressed("action"))
		{
			if (_shopWindow != null)
			{
				GD.Print("ShopTrigger: player pressed E -> opening shop");
				_shopWindow.OpenShop();
			}
			else
			{
				GD.PrintErr("ShopTrigger: ShopWindow not assigned! Set ShopWindowPath in Inspector to HUD/ShopWindow");
			}
		}
	}
}
