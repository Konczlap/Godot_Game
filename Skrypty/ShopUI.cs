using Godot;
using System;

public partial class ShopUI : CanvasLayer
{
	[Export] private VehicleManager vehicleManager;
	[Export] private PlayerMoney playerMoney;

	// UI
	[Export] private Button buyPersonalBtn;
	[Export] private Button buyKombiBtn;
	[Export] private Button buyTruckBtn;

	//[Export] private Label moneyLabel;

	public bool IsOpen { get; private set; } = false;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;   // 🔥 KLUCZ
		Visible = false;

		buyPersonalBtn.Pressed += () => OnBuyOrEquip(VehicleType.Personal);
		buyKombiBtn.Pressed += () => OnBuyOrEquip(VehicleType.Kombi);
		buyTruckBtn.Pressed += () => OnBuyOrEquip(VehicleType.Truck);
	}

	public override void _UnhandledInput(InputEvent e)
	{
		if (!Visible)
			return;

		if (e.IsActionPressed("ui_cancel")) // ESC
		{
			GD.Print("Opuszczasz sklep");
			CloseShop();
		}
	}

	// =====================
	// OTWIERANIE / ZAMYKANIE
	// =====================

	public void OpenShop()
	{
		IsOpen = true;
		Visible = true;
		GetTree().Paused = true;
		GD.Print($"VM: {vehicleManager}");
		GD.Print($"Money: {playerMoney}");

		RefreshUI();
		GD.Print("🛒 Sklep otwarty");
	}

	public void CloseShop()
	{
		IsOpen = false;
		Visible = false;
		GetTree().Paused = false;

		GD.Print("❌ Sklep zamknięty");
	}

	public void HideShop()
	{
		IsOpen = false;
		Visible = false;
	}

	// =====================
	// LOGIKA SKLEPU
	// =====================

	private void OnBuyOrEquip(VehicleType type)
	{
		VehicleData data = GetData(type);

		if (vehicleManager.Owns(type))
		{
			vehicleManager.EquipVehicle(data);
			RefreshUI();
			return;
		}

		bool success = vehicleManager.BuyVehicle(data, playerMoney);

		if (!success)
		{
			GD.Print("❌ Za mało pieniędzy!");
			return;
		}

		RefreshUI();
	}

	private void RefreshUI()
	{
		//moneyLabel.Text = $"Kasa: {playerMoney.GetMoney():0.##} $";

		UpdateButton(buyPersonalBtn, VehicleType.Personal);
		UpdateButton(buyKombiBtn, VehicleType.Kombi);
		UpdateButton(buyTruckBtn, VehicleType.Truck);
	}

	private void UpdateButton(Button btn, VehicleType type)
	{
		if (vehicleManager.Owns(type))
		{
			if (vehicleManager.CurrentVehicle.Type == type)
			{
				btn.Text = "W UŻYCIU";
				btn.Disabled = true;
			}
			else
			{
				btn.Text = "WYPOSAŻ";
				btn.Disabled = false;
			}
		}
		else
		{
			VehicleData data = GetData(type);
			btn.Text = $"KUP ({data.Price}$)";
			btn.Disabled = false;
		}
	}

	private VehicleData GetData(VehicleType type)
	{
		return type switch
		{
			VehicleType.Personal => vehicleManager.Personal,
			VehicleType.Kombi => vehicleManager.Kombi,
			VehicleType.Truck => vehicleManager.Truck,
			_ => vehicleManager.Personal
		};
	}
}
