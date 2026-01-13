using Godot;
using System;

public partial class Options : Control
{
	private Control graphicsSection;
	private Control audioSection;
	private HSlider masterSlider;

	public override void _Ready()
	{
		graphicsSection = GetNode<Control>("VBoxContainer/GraphicsSection");
		audioSection = GetNode<Control>("VBoxContainer/AudioSection");

		masterSlider = GetNode<HSlider>(
			"VBoxContainer/AudioSection/VolumeLabel/MasterSlider");

		int bus = AudioServer.GetBusIndex("Master");
		masterSlider.Value = Mathf.DbToLinear(
			AudioServer.GetBusVolumeDb(bus));

		masterSlider.ValueChanged += OnMasterVolumeChanged;

		HideAllSections();
	}

	private void OnMasterVolumeChanged(double value)
	{
		int bus = AudioServer.GetBusIndex("Master");
		AudioServer.SetBusVolumeDb(bus, Mathf.LinearToDb((float)value));
	}

	private void HideAllSections()
	{
		graphicsSection.Visible = false;
		audioSection.Visible = false;
	}

	public void OnGraphicsPressed()
	{
		HideAllSections();
		graphicsSection.Visible = true;
	}

	public void OnAudioPressed()
	{
		HideAllSections();
		audioSection.Visible = true;
	}
}
