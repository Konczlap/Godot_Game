using Godot;
using System;

public partial class PlayerMoney : Node2D
{
	[Export] private float _money = 0f;

	public void AddMoney(float amount)
	{
		_money += amount;
		//_money = (float)Math.Round(amount, 2);
		GD.Print($"💰 Dodano {amount}$. Aktualny stan konta: {_money}$");
	}

	public bool SpendMoney(float amount)
	{
		if (_money >= amount)
		{
			_money -= amount;
			//_money = (float)Math.Round(amount, 2);
			GD.Print($"💸 Wydano {amount}$. Pozostało: {_money}$");
			return true;
		}
		else
		{
			GD.Print("❌ Brak środków!");
			return false;
		}
	}
	
	public float GetMoney()
	{
		return _money;
	}
}
