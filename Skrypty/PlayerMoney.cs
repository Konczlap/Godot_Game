// PlayerMoney.cs - WERSJA Z PEŁNYM DEBUGIEM
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
		float before = _money;
		_money += amount;
		incomePerDay += amount;
		
		// ✅ ROZSZERZONY DEBUG
		GD.Print($"💰 AddMoney: {before}$ + {amount}$ = {_money}$");
		
		// ✅ STACK TRACE - pokaż kto wywołał tę funkcję
		GD.Print($"   └─ Wywołane z: {new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name}");
	}
	
	public bool SpendMoney(float amount)
	{
		float before = _money;
		
		// ✅ ROZSZERZONY DEBUG
		GD.Print($"🔍 SpendMoney wywołane: _money={before}$, amount={amount}$");
		GD.Print($"   └─ Wywołane z: {new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name}");
		
		if (_money >= amount)
		{
			_money -= amount;
			_spendMoney += amount;
			spendPerDay += amount;
			
			GD.Print($"✅ Wydano: {before}$ - {amount}$ = {_money}$");
			return true;
		}
		else
		{
			GD.Print($"❌ Brak kasy! Potrzebujesz {amount}$, masz {before}$");
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
		float before = _money;
		_money = value;
		
		// ✅ ROZSZERZONY DEBUG
		GD.Print($"💵 SetMoney: {before}$ → {value}$");
		GD.Print($"   └─ Wywołane z: {new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name}");
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
