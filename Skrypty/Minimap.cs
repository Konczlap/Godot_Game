// Minimap.cs
using Godot;
using System;

public partial class Minimap : CanvasLayer
{
	private SubViewportContainer _subViewportContainer;
	private SubViewport _subViewport;
	private Camera2D _minimapCamera;
	private ColorRect _playerMarker;
	private ColorRect _packageMarker;
	private Polygon2D _edgeMarker; 
	public Node2D player_node;
	public Node2D target_package;
	
	private bool _isPackageMode = true;

	public override void _Ready()
	{
		GD.Print("=== Minimap _Ready START ===");
		
		_subViewportContainer = GetNodeOrNull<SubViewportContainer>("UI/MarginContainer/SubViewportContainer");
		_subViewport = GetNodeOrNull<SubViewport>("UI/MarginContainer/SubViewportContainer/SubViewport");
		_minimapCamera = GetNodeOrNull<Camera2D>("UI/MarginContainer/SubViewportContainer/SubViewport/MinimapCamera");
		
		if (_minimapCamera == null)
		{
			_minimapCamera = GetNodeOrNull<Camera2D>("UI/MarginContainer/SubViewportContainer/Subviewport/MinimapCamera");
		}
		
		_playerMarker = _subViewport?.GetNodeOrNull<ColorRect>("PlayerMarker");
		if (_playerMarker == null && _subViewport != null)
		{
			_playerMarker = new ColorRect();
			_playerMarker.Name = "PlayerMarker";
			_playerMarker.Size = new Vector2(16, 16); 
			_playerMarker.Color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
			_playerMarker.Position = new Vector2(-6, -6);
			_playerMarker.ZIndex = 101;
			_subViewport.AddChild(_playerMarker);
			GD.Print("Utworzono marker gracza");
		}
		
		_packageMarker = _subViewport?.GetNodeOrNull<ColorRect>("PackageMarker");
		if (_packageMarker == null && _subViewport != null)
		{
			_packageMarker = new ColorRect();
			_packageMarker.Name = "PackageMarker";
			_packageMarker.Size = new Vector2(15, 15); 
			_packageMarker.Color = new Color(1.0f, 0.8f, 0.0f, 1.0f);
			_packageMarker.Position = new Vector2(-12.5f, -12.5f);
			_packageMarker.Visible = false;
			_packageMarker.ZIndex = 100;
			_subViewport.AddChild(_packageMarker);
			GD.Print("Utworzono marker paczki");
		}
		
		_edgeMarker = _subViewport?.GetNodeOrNull<Polygon2D>("EdgeMarker");
		if (_edgeMarker == null && _subViewport != null)
		{
			_edgeMarker = new Polygon2D();
			_edgeMarker.Name = "EdgeMarker";
			Vector2[] arrowShape = new Vector2[]
			{
				new Vector2(20, 0),    
				new Vector2(-15, -12), 
				new Vector2(-8, 0),    
				new Vector2(-15, 12)   
			};
			_edgeMarker.Polygon = arrowShape;
			_edgeMarker.Color = new Color(1.0f, 0.3f, 0.0f, 1.0f); 
			_edgeMarker.Visible = false;
			_edgeMarker.ZIndex = 102;
			_subViewport.AddChild(_edgeMarker);
			GD.Print("Utworzono marker krawędzi (strzałka)");
		}

		GD.Print($"SubViewport: {(_subViewport != null ? "OK" : "NULL")}");
		GD.Print($"MinimapCamera: {(_minimapCamera != null ? "OK" : "NULL")}");
		GD.Print($"PlayerMarker: {(_playerMarker != null ? "OK" : "NULL")}");
		GD.Print($"PackageMarker: {(_packageMarker != null ? "OK" : "NULL")}");
		GD.Print($"EdgeMarker: {(_edgeMarker != null ? "OK" : "NULL")}");

		if (_subViewport == null)
		{
			GD.PrintErr("Minimap: SubViewport nie został znaleziony!");
			return;
		}

		if (_minimapCamera == null)
		{
			GD.PrintErr("Minimap: MinimapCamera nie została znaleziona!");
			return;
		}

		var currentScene = GetTree().CurrentScene;
		if (currentScene == null)
		{
			GD.PrintErr("Minimap: CurrentScene is null.");
			return;
		}

		var mapa = currentScene.GetNodeOrNull<Node>("Mapa");
		if (mapa == null)
		{
			GD.PrintErr("Minimap: Nie znaleziono węzła 'Mapa'.");
			return;
		}

		GD.Print($"Mapa znaleziona! Dzieci Mapy:");
		bool foundDroga = false;
		
		var tlo = mapa.GetNodeOrNull<Sprite2D>("Tlo");
		if (tlo != null)
		{
			var minimapTlo = tlo.Duplicate() as Sprite2D;
			if (minimapTlo != null)
			{
				minimapTlo.Modulate = new Color(0.6f, 0.6f, 0.6f);
				_subViewport.AddChild(minimapTlo);
				GD.Print("Dodano tło do minimapy");
			}
		}
		
		var domki = mapa.GetNodeOrNull<Node2D>("Domki");
		if (domki != null)
		{
			var minimapDomki = domki.Duplicate() as Node2D;
			if (minimapDomki != null)
			{
				minimapDomki.Modulate = new Color(0.8f, 0.8f, 0.8f, 0.7f);
				_subViewport.AddChild(minimapDomki);
				GD.Print("Dodano domy do minimapy");
			}
		}
		
		var drzewka = mapa.GetNodeOrNull<Node2D>("Drzewka");
		if (drzewka != null)
		{
			var minimapDrzewka = drzewka.Duplicate() as Node2D;
			if (minimapDrzewka != null)
			{
				minimapDrzewka.Modulate = new Color(0.5f, 0.8f, 0.5f, 0.6f);
				_subViewport.AddChild(minimapDrzewka);
				GD.Print("Dodano drzewka do minimapy");
			}
		}
		
		var kamienie = mapa.GetNodeOrNull<Node2D>("Kamienie");
		if (kamienie != null)
		{
			var minimapKamienie = kamienie.Duplicate() as Node2D;
			if (minimapKamienie != null)
			{
				minimapKamienie.Modulate = new Color(0.7f, 0.7f, 0.7f, 0.6f);
				_subViewport.AddChild(minimapKamienie);
				GD.Print("Dodano kamienie do minimapy");
			}
		}
		
		var stacja = mapa.GetNodeOrNull<Node2D>("Stacja Paliw");
		if (stacja != null)
		{
			var minimapStacja = stacja.Duplicate() as Node2D;
			if (minimapStacja != null)
			{
				minimapStacja.Modulate = new Color(1.0f, 1.0f, 0.5f, 0.8f);
				_subViewport.AddChild(minimapStacja);
				GD.Print("Dodano stację paliw do minimapy");
			}
		}
		
		foreach (Node child in mapa.GetChildren())
		{
			if (child.Name == "Droga")
			{
				foundDroga = true;
				GD.Print($"Znaleziono węzeł 'Droga'!");
				
				var minimapDroga = child.Duplicate() as Node2D;
				if (minimapDroga != null)
				{
					_subViewport.AddChild(minimapDroga);
					
					if (_minimapCamera != null)
					{
						_minimapCamera.Zoom = new Vector2(0.7f, 0.7f);
					}
					
					SetMinimapLimitsFromNode(child);
				}
				break;
			}
		}
		
		if (!foundDroga)
		{
			GD.PrintErr("Minimap: Nie znaleziono węzła 'Droga'!");
		}
		
		GD.Print("=== Minimap _Ready END ===");
	}

	public override void _Process(double delta)
	{
		if (player_node != null && _minimapCamera != null)
		{
			Vector2 camPos = _minimapCamera.GlobalPosition;
			Vector2 targetPos = player_node.GlobalPosition;
			_minimapCamera.GlobalPosition = camPos.Lerp(targetPos, 0.1f);
			
			if (_playerMarker != null)
			{
				_playerMarker.GlobalPosition = player_node.GlobalPosition;
				_playerMarker.Visible = true;
			}
			
			if (target_package != null)
			{
				if (_packageMarker == null || _edgeMarker == null)
				{
					return; 
				}
				
				Vector2 targetWorldPos = target_package.GlobalPosition;
				Vector2 cameraWorldPos = _minimapCamera.GlobalPosition;
				Vector2 relativePos = targetWorldPos - cameraWorldPos;
				
				Vector2 viewportSize = _subViewport != null ? _subViewport.Size : new Vector2(200, 200);
				Vector2 visibleWorldSize = viewportSize / _minimapCamera.Zoom;
				float halfWidth = visibleWorldSize.X / 2;
				float halfHeight = visibleWorldSize.Y / 2;
				
				float margin = 30;
				bool isVisible = Mathf.Abs(relativePos.X) < (halfWidth - margin) && 
								 Mathf.Abs(relativePos.Y) < (halfHeight - margin);
				
				if (isVisible)
				{
					_packageMarker.GlobalPosition = targetWorldPos;
					_packageMarker.Visible = true;
					_packageMarker.Scale = Vector2.One; 
					_edgeMarker.Visible = false;
				}
				else
				{
					_packageMarker.Visible = false;
					
					Vector2 direction = relativePos.Normalized();
					
					float edgeMargin = 35;
					float maxX = halfWidth - edgeMargin;
					float maxY = halfHeight - edgeMargin;
					
					float scaleX = maxX / Mathf.Abs(direction.X);
					float scaleY = maxY / Mathf.Abs(direction.Y);
					float scale = Mathf.Min(scaleX, scaleY);
					
					Vector2 edgeOffset = direction * scale;
					Vector2 edgeWorldPos = cameraWorldPos + edgeOffset;
					
					_edgeMarker.GlobalPosition = edgeWorldPos;
					_edgeMarker.Visible = true;
					_edgeMarker.Rotation = relativePos.Angle();
					_edgeMarker.Scale = Vector2.One;
				}
			}
			else
			{
				if (_packageMarker != null) _packageMarker.Visible = false;
				if (_edgeMarker != null) _edgeMarker.Visible = false;
			}
		}
	}

	public void ShowPackageMarker()
	{
		_isPackageMode = true;
		if (_packageMarker != null)
		{
			_packageMarker.Color = new Color(1.0f, 0.8f, 0.0f, 1.0f); 
		}
		if (_edgeMarker != null)
		{
			_edgeMarker.Color = new Color(1.0f, 0.5f, 0.0f, 1.0f); 
		}
	}

	public void ShowCustomerMarker()
	{
		_isPackageMode = false;
		if (_packageMarker != null)
		{
			_packageMarker.Color = new Color(0.0f, 1.0f, 0.0f, 1.0f); 
		}
		if (_edgeMarker != null)
		{
			_edgeMarker.Color = new Color(0.0f, 0.8f, 0.0f, 1.0f); 
		}
	}
	
	public void ClearTarget()
	{
		target_package = null;
		if (_packageMarker != null) _packageMarker.Visible = false;
		if (_edgeMarker != null) _edgeMarker.Visible = false;
		GD.Print("🗺️ Minimap: Cel wyczyszczony");
	}

	private void SetMinimapLimitsFromNode(Node roadNode)
	{
		if (_minimapCamera == null) return;

		float minX = float.MaxValue, minY = float.MaxValue;
		float maxX = float.MinValue, maxY = float.MinValue;
		bool foundAny = false;

		foreach (Node child in roadNode.GetChildren())
		{
			if (child is Sprite2D sprite)
			{
				foundAny = true;
				Vector2 pos = sprite.GlobalPosition;
				Texture2D texture = sprite.Texture;
				
				if (texture != null)
				{
					Vector2 size = texture.GetSize() * sprite.Scale;
					minX = Mathf.Min(minX, pos.X - size.X / 2);
					minY = Mathf.Min(minY, pos.Y - size.Y / 2);
					maxX = Mathf.Max(maxX, pos.X + size.X / 2);
					maxY = Mathf.Max(maxY, pos.Y + size.Y / 2);
				}
			}
		}

		if (foundAny)
		{
			float margin = 1000;
			_minimapCamera.LimitLeft = (int)(minX - margin);
			_minimapCamera.LimitTop = (int)(minY - margin);
			_minimapCamera.LimitRight = (int)(maxX + margin);
			_minimapCamera.LimitBottom = (int)(maxY + margin);
			_minimapCamera.PositionSmoothingEnabled = true;
			_minimapCamera.PositionSmoothingSpeed = 5.0f;
			
			GD.Print($"✅ Limity kamery minimapy ustawione");
			GD.Print($"✅ Zoom kamery: {_minimapCamera.Zoom}");
		}
	}
}
