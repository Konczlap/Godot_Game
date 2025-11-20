using Godot;
using System;

public partial class PlayerMoney : Node2D
{
	[Export] private float _money = 0f;
	private float _spendMoney = 0f;
	
	public float IncomePerDay = 0f;
	public float SpendPerDay = 0f;

	public void AddMoney(float amount)
	{
		_money += amount;
		IncomePerDay += amount;
	}

	public bool SpendMoney(float amount)
	{
		if (_money >= amount)
		{
			_money -= amount;
			_spendMoney += amount;
			SpendPerDay += amount;
			return true;
		}
		else
		{
			//GD.Print("❌ Brak środków!");
			return false;
		}
	}
	
	public float GetSpendedMoney()
	{
		return _spendMoney;
	}
	
	public void ZeroingSpendMoney()
	{
		_spendMoney = 0f;
	}
	
	public float GetMoney()
	{
		return _money;
	}
}
