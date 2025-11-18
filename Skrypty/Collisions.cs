using Godot;
using System;

public partial class Collisions : Node2D
{
	// Ścieżka do samochodu (Car → MovementScript)
	//[Export] private MovementScript _movementScript;

	public void CheckCollisionStop(MovementScript _movementScript)
	{
		// Sprawdzenie kolizji po ruchu
		if (_movementScript.GetSlideCollisionCount() > 0)
		{
			//GD.Print("Kolizja!");
			StopCar(_movementScript);
			// Nie blokujemy CanMove, aby możliwy był ruch w tył
		}
	}

	// Natychmiast zatrzymuje samochód
	public void StopCar(MovementScript _movementScript)
	{
		_movementScript.SetCurrentSpeed(0f);
		_movementScript.Velocity = Vector2.Zero;
	}

	//// Wywoływana, gdy samochód wchodzi w Area2D (Delivery)
	//private void _on_Delivery_body_entered(Node2D body)
	//{
		//// Konwersja StringName do string, aby używać Contains i ToLower
		//string name = body.Name.ToString().ToLower();
//
		//// Kolizje tylko z budynkami, sklepami i stacją paliw
		//if (name.Contains("dom") || name.Contains("sklep") || name.Contains("stacja"))
		//{
			//GD.Print("Kolizja z: " + body.Name);
//
			//// Natychmiast zatrzymujemy samochód
			//_car.StopCar();
//
			//// Tutaj można zablokować jazdę do przodu, ruch w tył pozostaje możliwy
		//}
	//}
}
