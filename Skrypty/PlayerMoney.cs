// PlayerMoney.cs
using Godot;
using System;

public partial class PlayerMoney : Node2D
{
	private static PlayerMoney _instance;
	
	private float _money = 50f;
	private float _spendMoney = 0f;
	
	private float incomePerDay = 0f;
	private float spendPerDay = 0f;
	private const int decimals = 2;
	
	public override void _EnterTree()
	{
		if (_instance != null && _instance != this)
		{
			if (_instance.IsInsideTree())
			{
				QueueFree();
				return;
			}
		}
		_instance = this;
	}
	
	public static PlayerMoney Instance => _instance;
	
	public void AddMoney(float amount)
	{
		_money += amount;
		incomePerDay += amount;
		GD.Print($"💰 +{amount}$ | Stan: {_money}$");
	}
	
	public bool SpendMoney(float amount)
	{
		GD.Print($"🔍 PRZED: _money={_money}, amount={amount}");
		
		if (_money >= amount)
		{
			_money -= amount;
			_spendMoney += amount;
			spendPerDay += amount;
			GD.Print($"🔍 PO: _money={_money}");
			GD.Print($"💸 -{amount}$ | Stan: {_money}$ | Wydano dziś: {_spendMoney}$");
			return true;
		}
		else
		{
			GD.Print($"❌ Brak kasy! Potrzebujesz {amount}$, masz {_money}$");
			return false;
		}
	}
	
	public float GetSpendedMoney()
	{
		return (float)Math.Round(_spendMoney, decimals);
	}
	
	public void ZeroingSpendMoney()
	{
		GD.Print($"🔄 Reset wydanych pieniędzy: {_spendMoney}$ → 0$");
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
		return _money;
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
