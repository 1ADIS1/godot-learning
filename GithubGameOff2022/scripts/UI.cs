using Godot;
using System;

public class UI : CanvasLayer
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}


 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
	 var player = GetNode("/root/Main/Player") as Player;
	 (GetNode("Health") as TextureRect).Visible = player.Health > 0;
	 (GetNode("Health2") as TextureRect).Visible = player.Health > 1;
	 (GetNode("Health3") as TextureRect).Visible = player.Health > 2;
	 (GetNode("SoulIcon/Label") as Label).Text = player.score.ToString();
 }
}
