using Godot;

public partial class Package : Node2D
{
	[Export] private float _price = 25f; // wartość paczki
	[Export] public NodePath CustomerPath; // referencja do klienta (ustawiana w edytorze)
	private Customer _targetCustomer;

	public override void _Ready()
	{
		if (CustomerPath != null)
			_targetCustomer = GetNodeOrNull<Customer>(CustomerPath);

		if (_targetCustomer != null)
		{
			// zarejestruj się u klienta
			_targetCustomer.AssignPackage(this);
			GD.Print($"{Name} przypisana do klienta {_targetCustomer.GetCustomerName()}");
		}
	}
	
	public string GetTargetCustomer()
	{
		return _targetCustomer.GetCustomerName();
	}
	
	public float GetPackagePrice()
	{
		return _price;
	}
}
