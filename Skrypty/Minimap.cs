using Godot;
using System;
using System.Collections.Generic;

public partial class Minimap : CanvasLayer
{
	private SubViewportContainer _subViewportContainer;
	private SubViewport _subViewport;
	private Camera2D _minimapCamera;
	private ColorRect _playerMarker;
	
	private Control _markersOverlay; // Kontener na markery w UI
	
	// Listy markerów dla wielu celów
	private List<ColorRect> _packageMarkers = new List<ColorRect>();
	private List<Polygon2D> _edgeMarkers = new List<Polygon2D>();
	private const int MAX_MARKERS = 6; // Maksymalna liczba paczek
	
	public Node2D player_node;
	public List<Node2D> target_packages = new List<Node2D>(); // Lista celów zamiast jednego
	
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
		
		// Utwórz Overlay na markery (nad SubViewportem)
		if (_subViewportContainer != null)
		{
			_markersOverlay = new Control();
			_markersOverlay.Name = "MarkersOverlay";
			_markersOverlay.SetAnchorsPreset(Control.LayoutPreset.FullRect);
			_markersOverlay.MouseFilter = Control.MouseFilterEnum.Ignore;
			_subViewportContainer.AddChild(_markersOverlay);
			GD.Print("Utworzono MarkersOverlay");
		}
		
		// Tworzenie wielu markerów dla paczek/klientów w UI Overlay
		for (int i = 0; i < MAX_MARKERS; i++)
		{
			// Marker paczki/klienta
			var packageMarker = new ColorRect();
			packageMarker.Name = $"PackageMarker_{i}";
			packageMarker.Size = new Vector2(10, 10);  // Mniejszy bo w UI
			packageMarker.Color = new Color(1.0f, 0.8f, 0.0f, 1.0f);
			packageMarker.PivotOffset = new Vector2(5, 5); // Pivot na środku
			packageMarker.Visible = false;
			// packageMarker.ZIndex = 100; // W UI ZIndex działa inaczej (kolejność w drzewie)
			
			if (_markersOverlay != null)
				_markersOverlay.AddChild(packageMarker);
				
			_packageMarkers.Add(packageMarker);
			
			// Marker strzałki na krawędzi
			var edgeMarker = new Polygon2D();
			edgeMarker.Name = $"EdgeMarker_{i}";
			Vector2[] arrowShape = new Vector2[]
			{
				new Vector2(15, 0),    
				new Vector2(-10, -8), 
				new Vector2(-5, 0),    
				new Vector2(-10, 8)   
			};
			edgeMarker.Polygon = arrowShape;
			edgeMarker.Color = new Color(1.0f, 0.3f, 0.0f, 1.0f); 
			edgeMarker.Visible = false;
			
			// Polygon2D w Controlu pozycjonuje się względem (0,0) Controla.
			if (_markersOverlay != null)
				_markersOverlay.AddChild(edgeMarker);
				
			_edgeMarkers.Add(edgeMarker);
		}
		GD.Print($"Utworzono {MAX_MARKERS} markerów paczek i strzałek w Overlayu");

		GD.Print($"SubViewport: {(_subViewport != null ? "OK" : "NULL")}");
		GD.Print($"MinimapCamera: {(_minimapCamera != null ? "OK" : "NULL")}");
		GD.Print($"PlayerMarker: {(_playerMarker != null ? "OK" : "NULL")}");
		GD.Print($"PackageMarkers: {_packageMarkers.Count}");
		GD.Print($"EdgeMarkers: {_edgeMarkers.Count}");

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
						_minimapCamera.Zoom = new Vector2(0.5f, 0.5f); // 0.7f, 0.7f
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
			
			// Ukryj wszystkie markery na start
			for (int i = 0; i < MAX_MARKERS; i++)
			{
				_packageMarkers[i].Visible = false;
				_edgeMarkers[i].Visible = false;
			}
			
			// Pobierz rozmiar wyświetlanej minimapy na ekranie UI
			Vector2 minimapSize = _subViewportContainer != null ? _subViewportContainer.Size : new Vector2(190, 190);
			Vector2 minimapCenter = minimapSize / 2.0f;
			
			// Pozycja kamery w świecie
			Vector2 cameraWorldPos = _minimapCamera.GlobalPosition;
			
			// Promień minimapy (połowa szerokości)
			float radius = minimapSize.X / 2.0f;
			
			// Pokaż markery dla każdego celu
			for (int i = 0; i < target_packages.Count && i < MAX_MARKERS; i++)
			{
				var target = target_packages[i];
				if (target == null) continue;
				
				Vector2 targetWorldPos = target.GlobalPosition;
				Vector2 worldDiff = targetWorldPos - cameraWorldPos;
				
				// Przelicz różnicę świata na piksele UI (używając zoomu)
				Vector2 screenDiff = worldDiff * _minimapCamera.Zoom;
				
				// Odległość od środka minimapy w pikselach
				float distFromCenter = screenDiff.Length();
				
				// Marginesy
				float dotMargin = 10.0f;       // Margines dla kropki
				float arrowTipOffset = 15.0f;  // Odsunięcie czubka strzałki od jej środka (zgodnie z definicją Polygon2D)
				float arrowMargin = 5.0f;      // Dodatkowy margines od krawędzi koła
				
				// Maksymalny promień dla strzałki, aby jej czubek nie wystawał
				// (Promień mapy) - (Długość strzałki) - (Margines estetyczny)
				float maxArrowOrbit = radius - arrowTipOffset - arrowMargin;
				
				// Sprawdzamy czy cel mieści się w kole (z marginesem dla kropki)
				if (distFromCenter < (radius - dotMargin))
				{
					// WIDOCZNY - Pokaż kropkę
					Vector2 targetScreenPos = minimapCenter + screenDiff;
					
					_packageMarkers[i].Position = targetScreenPos - _packageMarkers[i].Size / 2;
					_packageMarkers[i].Visible = true;
					_edgeMarkers[i].Visible = false;
				}
				else
				{
					// NIEWIDOCZNY - Pokaż strzałkę na krawędzi okręgu
					_packageMarkers[i].Visible = false;
					
					Vector2 direction = screenDiff.Normalized();
					
					// Pozycja na obwodzie (pomniejszonym tak, by czubek dotykał krawędzi)
					Vector2 edgePos = minimapCenter + direction * maxArrowOrbit;
					
					_edgeMarkers[i].Position = edgePos;
					_edgeMarkers[i].Rotation = direction.Angle(); // Obróć strzałkę w stronę celu
					_edgeMarkers[i].Visible = true;
				}
			}
		}
	}

	public void ShowPackageMarker()
	{
		_isPackageMode = true;
		foreach (var marker in _packageMarkers)
		{
			marker.Color = new Color(1.0f, 0.8f, 0.0f, 1.0f); 
		}
		foreach (var marker in _edgeMarkers)
		{
			marker.Color = new Color(1.0f, 0.5f, 0.0f, 1.0f); 
		}
	}

	public void ShowCustomerMarker()
	{
		_isPackageMode = false;
		foreach (var marker in _packageMarkers)
		{
			marker.Color = new Color(0.0f, 1.0f, 0.0f, 1.0f); 
		}
		foreach (var marker in _edgeMarkers)
		{
			marker.Color = new Color(0.0f, 0.8f, 0.0f, 1.0f); 
		}
	}
	
	// Nowe metody do zarządzania celami
	public void SetTargets(List<Node2D> targets)
	{
		target_packages.Clear();
		target_packages.AddRange(targets);
	}
	
	public void ClearTargets()
	{
		target_packages.Clear();
	}
	
	public void AddTarget(Node2D target)
	{
		if (!target_packages.Contains(target))
		{
			target_packages.Add(target);
		}
	}
	
	public void RemoveTarget(Node2D target)
	{
		target_packages.Remove(target);
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
