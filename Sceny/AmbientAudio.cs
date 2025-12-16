using Godot;
using System;

public partial class AmbientAudio : AudioStreamPlayer
{
	[Export] private DayNightCycle dayNight;

	[Export] private float dayVolume = -6f;
	[Export] private float nightVolume = -30f;
	[Export] private float fadeSpeed = 10f;

	public override void _Ready()
	{
		VolumeDb = nightVolume;
		Play();
	}

	public override void _Process(double delta)
	{
		if (dayNight == null)
			return;

		int hour = dayNight.GetTimeHour();
		int minute = dayNight.GetTimeMinute();

		float targetVolume;

		// DZIEŃ
		if (hour >= 6 && hour < 18)
		{
			targetVolume = dayVolume;
		}
		// WIECZÓR (18:00–22:00) → płynne wyciszanie
		else if (hour >= 18 && hour < 22)
		{
			float minutesSince18 = (hour - 18) * 60 + minute;
			float t = minutesSince18 / (4f * 60f); // 0–1

			targetVolume = Mathf.Lerp(dayVolume, nightVolume, t);
		}
		// NOC
		else
		{
			targetVolume = nightVolume;
		}

		VolumeDb = Mathf.MoveToward(
			VolumeDb,
			targetVolume,
			fadeSpeed * (float)delta
		);
	}
}
