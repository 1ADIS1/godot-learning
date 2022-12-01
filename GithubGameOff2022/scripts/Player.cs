using Godot;
using System;
using System.Collections.Generic;

public class Player : KinematicBody2D
{
	[Export] public float Speed;
	[Export] public float secondsOfImmortalityAfterHit;
	[Export] public float secondsOfReloadAfterShot;
	[Export] public int Health;
	[Export] public PackedScene bullet;
	private List<Walker> collidingEnemies;
	float secondsOfImmortalityLeft = 0;
	float reloadingTimeLeft = 0;

	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		collidingEnemies = new List<Walker>();
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Process(float delta)
	{
		secondsOfImmortalityLeft -= delta;
		reloadingTimeLeft -= delta;
		if (reloadingTimeLeft <0)
			reloadingTimeLeft = 0;
		if (secondsOfImmortalityLeft < 0)
		{
			secondsOfImmortalityLeft = 0;
			collidingEnemies = collidingEnemies.FindAll(enemy => enemy != null);
			if (collidingEnemies.Count > 0)
			{
				Health--;
				if (Health <= 0 ){

					GetTree().ReloadCurrentScene();
					return;
				}
				secondsOfImmortalityLeft = secondsOfImmortalityAfterHit;
			}
		}
		if (Input.IsActionPressed("ui_down"))
		{
			_animationPlayer.Play("walk_down");
		}
		else if (Input.IsActionPressed("ui_up"))
		{
			_animationPlayer.Play("walk_up");
		}
		else if (Input.IsActionPressed("ui_left"))
		{
			_animationPlayer.Play("walk_left");
		}
		else if (Input.IsActionPressed("ui_right"))
		{
			_animationPlayer.Play("walk_right");
		}
		else
		{
			_animationPlayer.Stop();
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 direction = Vector2.Zero;

		if (Input.IsActionPressed("ui_down"))
		{
			direction = Vector2.Down;
		}
		else if (Input.IsActionPressed("ui_up"))
		{
			direction = Vector2.Up;
		}
		else if (Input.IsActionPressed("ui_left"))
		{
			direction = Vector2.Left;
		}
		else if (Input.IsActionPressed("ui_right"))
		{
			direction = Vector2.Right;
		}

		if (reloadingTimeLeft <= 0 && Input.IsMouseButtonPressed(1)){
			reloadingTimeLeft = secondsOfReloadAfterShot;
			var projectileDir = GlobalPosition.DirectionTo(GetGlobalMousePosition());
			var bulletInstance = bullet.Instance() as Bullet;
			bulletInstance.InitDirection(projectileDir);
			bulletInstance.GlobalPosition = GlobalPosition;
			GetNode("/root/Main").AddChild(bulletInstance);
		}

		MoveAndCollide(direction * Speed * delta);
	}

	internal void SetCentreOfGlobalMassPosition(Vector2 globalPosition)
	{
		GlobalPosition = globalPosition;
		GlobalPosition += -GetNode<Node2D>("PhysicsCollision").Position;
	}

	private void _on_Area2D_body_entered(object body)
	{
		var enemy = body as Walker;
		if (enemy == null)
			return;
		if (!collidingEnemies.Contains(enemy))
			collidingEnemies.Add(enemy);
	}
	private void _on_Area2D_body_exited(object body)
	{
		var enemy = body as Walker;
		if (enemy == null)
			return;
		if (collidingEnemies.Contains(enemy))
			collidingEnemies.Remove(enemy);
	}

}


