using Godot;
using System;

public class BackgroundPlayer : AudioStreamPlayer
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	[Export] public AudioStream secretMusic;
	[Export] public AudioStream normalMusic;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}
	public void PlayMusicForType(RoomType roomType){
		var newStream = roomType == RoomType.SECRET?secretMusic:normalMusic;
		if (Stream == newStream)
			return;
		Stream = newStream;
		Playing = true;
	}
	
}
