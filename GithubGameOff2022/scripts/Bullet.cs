using Godot;
using System;

public class Bullet : Area2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	[Export] public float Speed;
	private Vector2 direction = Vector2.Zero;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	internal void InitDirection(Vector2 projectileDir)
	{
		direction = projectileDir;
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)


	public override void _Process(float delta)
	{
		Vector2 velocity = Speed * delta * direction;
		GlobalPosition += velocity;
	}
	public void collisionOn(object body){
		if ((body as Node2D).Name == "Walls")
		{
			QueueFree();
		}else{
			var bodyAsWalker = body as Walker;
			if (bodyAsWalker != null){
				bodyAsWalker.GetDamaged();
			QueueFree();
			}
		}
	}


	private void _on_Node2D_body_entered(object body)
	{
		collisionOn(body);
	}
	private void _on_Node2D_area_entered(object area)
	{
		collisionOn(area);
	}
}





