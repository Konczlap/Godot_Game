using Godot;
using System;
using System.Collections.Generic;

public partial class SpawnManager : Node2D
{
	[Export] private NodePath _packagesParentPath;
	[Export] private NodePath _customersParentPath;

	private Node2D _packagesParent;
	private Node2D _customersParent;

	private List<Vector2> _customerPositions = new List<Vector2>();
	private List<Vector2> _packagePositions = new List<Vector2>();

	public override void _Ready()
	{
		_packagesParent = GetNode<Node2D>(_packagesParentPath);
		_customersParent = GetNode<Node2D>(_customersParentPath);

		foreach (Node child in _customersParent.GetChildren())
		{
			if (child is Node2D node)
				_customerPositions.Add(node.Position);
		}

		foreach (Node child in _packagesParent.GetChildren())
		{
			if (child is Node2D node)
				_packagePositions.Add(node.Position);
		}

		RandomizeSpawn();
	}

	public void RandomizeSpawn()
	{
		var rng = new RandomNumberGenerator();
		rng.Randomize();

		int count = Math.Min(_packagesParent.GetChildCount(), _customersParent.GetChildCount());

		List<Vector2> availableCustomerPos = new List<Vector2>(_customerPositions);
		List<Vector2> availablePackagePos = new List<Vector2>(_packagePositions);

		for (int i = 0; i < count; i++)
		{
			var package = _packagesParent.GetChild<Package>(i);
			var customer = _customersParent.GetChild<Customer>(i);

			int packageIndex = rng.RandiRange(0, availablePackagePos.Count - 1);
			Vector2 packagePos = availablePackagePos[packageIndex];
			availablePackagePos.RemoveAt(packageIndex);

			int customerIndex = rng.RandiRange(0, availableCustomerPos.Count - 1);
			Vector2 customerPos = availableCustomerPos[customerIndex];

			while (customerPos == packagePos && availableCustomerPos.Count > 1)
			{
				customerIndex = rng.RandiRange(0, availableCustomerPos.Count - 1);
				customerPos = availableCustomerPos[customerIndex];
			}
			availableCustomerPos.RemoveAt(customerIndex);

			package.Position = packagePos;
			customer.Position = customerPos;
		}

		GD.Print("✅ Paczki i klienci rozmieszczeni losowo!");
	}
}
