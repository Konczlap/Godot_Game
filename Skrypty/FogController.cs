using Godot;

public partial class FogController : CanvasLayer
{
	[Export] public NodePath FogRectPath = "FogRect";
	private Control _fogRect;

	public override void _Ready()
	{
		_fogRect = GetNodeOrNull<Control>(FogRectPath);
		var vp = GetViewport();
		if (vp != null)
			vp.SizeChanged += ApplySize;

		ApplySize();
		Visible = false; // start off
	}

	public override void _ExitTree()
	{
		var vp = GetViewport();
		if (vp != null)
			vp.SizeChanged -= ApplySize;
	}

	private void ApplySize()
	{
		if (_fogRect == null) return;
		_fogRect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		_fogRect.OffsetLeft = 0;
		_fogRect.OffsetTop = 0;
		_fogRect.OffsetRight = 0;
		_fogRect.OffsetBottom = 0;
	}
}
