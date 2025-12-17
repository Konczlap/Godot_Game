using Godot;

public partial class ShopUI : Control
{
	[Signal]
	public delegate void ShopClosedEventHandler();
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			GD.Print("[ShopUI] ESC naciśnięte - wysyłam sygnał ShopClosed");
			EmitSignal(SignalName.ShopClosed);
			GetViewport().SetInputAsHandled();
		}
	}
}
