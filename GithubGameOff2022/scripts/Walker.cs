using Godot;
using System;

public class Walker : KinematicBody2D
{
	internal Room room;
	[Export] public int Health;
	[Export] public int Damage;
	[Export] public float Speed;
	[Export] public PackedScene soulPrefab;
	private bool isPlayerInTheRoom = false;

	public void InitializeEnemy(bool isPlayerInTheRoom)
	{
		this.isPlayerInTheRoom = isPlayerInTheRoom;
	}

	public override void _Process(float delta)
	{
		if (!room.isClosedOff)
		{
			return;
		}
		Player player = GetNode<Player>("/root/Main/Player");
		Vector2 velocity = Speed * delta * GlobalPosition.DirectionTo(player.GlobalPosition);
		MoveAndCollide(velocity);
	}

	internal void GetDamaged()
	{
		Health--;
		if (Health <= 0){
			room.EnemyDefeated(this);
			var soulInstance = soulPrefab.Instance() as Node2D;
			soulInstance.GlobalPosition = GlobalPosition;
			GetNode("/root/Main").AddChild(soulInstance);
			QueueFree();
		}
	}
}
