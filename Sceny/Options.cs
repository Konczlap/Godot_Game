using Godot;
using System;

public partial class Options : Control
{
	private HSlider masterSlider;

	public override void _Ready()
	{
		masterSlider = GetNode<HSlider>("VBoxContainer/VolumeLabel/MasterSlider");

		int masterBus = AudioServer.GetBusIndex("Master");

		float currentDb = AudioServer.GetBusVolumeDb(masterBus);
		masterSlider.Value = Mathf.DbToLinear(currentDb);

		masterSlider.ValueChanged += OnMasterVolumeChanged;
	}

	private void OnMasterVolumeChanged(double value)
	{
		int masterBus = AudioServer.GetBusIndex("Master");
		AudioServer.SetBusVolumeDb(masterBus, Mathf.LinearToDb((float)value));
	}
}
