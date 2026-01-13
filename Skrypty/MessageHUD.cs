using Godot;
using System;

public partial class MessageHUD : CanvasLayer
{
	private Label _label;
	private ColorRect _background;

	public override void _Ready()
	{
		_label = GetNode<Label>("MessageHUDControl/MessageLabel");
		_background = GetNode<ColorRect>("MessageHUDControl/Background");
		HideMessage();
	}

	public void ShowMessage(string text, Color color)
	{
		_background.Visible = true;
		_label.Text = text;
		_label.Modulate = color;
		_label.Visible = true;
	}

	public void HideMessage()
	{
		_background.Visible = false;
		_label.Visible = false;
	}
}
