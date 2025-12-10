using Godot;
using System;

public partial class PlayerMoney : Node2D
{
	[Export] private float _money = 0f;
	private float _spendMoney = 0f;
	
	private float incomePerDay = 0f;
	private float spendPerDay = 0f;
	private const int decimals = 2;

	public void AddMoney(float amount)
	{
		_money += amount;
		incomePerDay += amount;
	}

	public bool SpendMoney(float amount)
	{
		if (_money >= amount)
		{
			_money -= amount;
			_spendMoney += amount;
			spendPerDay += amount;
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
		return (float)Math.Round(_spendMoney, decimals);
	}
	
	public void ZeroingSpendMoney()
	{
		_spendMoney = 0f;
	}
	
	public void ZeroingIncomePerDay()
	{
		incomePerDay = 0f;
	}
	
	public void ZeroingSpendPerDay()
	{
		spendPerDay = 0f;
	}
	
	public float GetMoney()
	{
		return (float)Math.Round(_money, decimals);
	}
	
	public void SetMoney(float value)
	{
		_money = value;
	}
	
	public float GetIncomePerDay()
	{
		return (float)Math.Round(incomePerDay, decimals);
	}
	
	public float GetSpendPerDay()
	{
		return (float)Math.Round(spendPerDay, decimals);
	}
}
