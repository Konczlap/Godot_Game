using Godot;
using System;

public partial class MoneyHUD : CanvasLayer
{
	private PlayerMoney MoneyScript;
	[Export] private float autoDelay = 0.5f;

	private Label _moneyLabel;
	private Label _floatingChange;
	private float _lastMoney = 0f;

	private float _pendingChange = 0f;
	private double _timeSinceChange = 0f;
	private bool _isAnimating = false;

	public override void _Ready()
	{
		_moneyLabel = GetNode<Label>("MoneyHUDControl/MoneyLabel");
		_floatingChange = GetNode<Label>("MoneyHUDControl/FloatingChange");

			MoneyScript = PlayerMoney.Instance;


		if (MoneyScript == null)
		{
			GD.PushWarning("MoneyHUD: MoneyScript nie podłączony!");
			return;
		}

		_lastMoney = MoneyScript.GetMoney();
		UpdateMoneyLabel();

		_floatingChange.Visible = false;
	}
	
	public void ForceUpdate()
	{
		if (MoneyScript == null) return;
		
		_lastMoney = MoneyScript.GetMoney();
		UpdateMoneyLabel();
		_pendingChange = 0f;
		_timeSinceChange = 0f;
	}
	
	public override void _Process(double delta)
	{
	if (MoneyScript == null)
	{
		GD.Print("❌ MoneyHUD: MoneyScript jest NULL!");
		return;
	}
	
	if (_isAnimating)
		return;

	float current = MoneyScript.GetMoney();
	float diff = current - _lastMoney;
	

	if (Math.Abs(diff) > 0.001f)
	{
		GD.Print($"✅ HUD: Wykryto zmianę! Aktualizuję...");
		_pendingChange += diff;
		_lastMoney = current;
		UpdateMoneyLabel();
		_timeSinceChange = 0f;
	}
	else if (Math.Abs(_pendingChange) > 0.001f)
	{
		_timeSinceChange += delta;
		if (_timeSinceChange >= autoDelay)
		{
			PlayPendingChange();
		}
	}
}

	private void UpdateMoneyLabel()
	{
		_moneyLabel.Text = $"{_lastMoney:0.##} $";
	}

	private async void PlayPendingChange()
	{
		if (_isAnimating || Math.Abs(_pendingChange) < 0.001f)
			return;

		_isAnimating = true;

		if (_pendingChange > 0f)
		{
			_floatingChange.Text = $"+{_pendingChange:0.##} $";
			_floatingChange.Modulate = new Color(0f, 1f, 0f, 1f);
		}
		else
		{
			_floatingChange.Text = $"{_pendingChange:0.##} $";
			_floatingChange.Modulate = new Color(1f, 0f, 0f, 1f);
		}

		_floatingChange.Visible = true;

		Vector2 startPos = _floatingChange.Position;
		Vector2 endPos = startPos + new Vector2(0, -24);
		float duration = 0.6f;
		float t = 0f;
		long startMs = (long)Time.GetTicksMsec();

		while (t < 1f)
		{
			long nowMs = (long)Time.GetTicksMsec();
			t = (nowMs - startMs) / (duration * 1000f);
			if (t > 1f) t = 1f;

			_floatingChange.Position = startPos.Lerp(endPos, t);
			var c = _floatingChange.Modulate;
			_floatingChange.Modulate = new Color(c.R, c.G, c.B, 1f - t);

			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}

		_floatingChange.Visible = false;
		_floatingChange.Position = startPos;
		_floatingChange.Modulate = new Color(_floatingChange.Modulate.R, _floatingChange.Modulate.G, _floatingChange.Modulate.B, 1f);

		_pendingChange = 0f;
		_timeSinceChange = 0f;
		_isAnimating = false;
	}
}
