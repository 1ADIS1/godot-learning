using Godot;
using System;

public class Walker : KinematicBody2D
{
    [Export] EnemyBase enemyBase;

    private bool isPlayerInTheRoom = false;

    public void InitializeEnemy(bool isPlayerInTheRoom)
    {
        this.isPlayerInTheRoom = isPlayerInTheRoom;
    }

    public override void _Process(float delta)
    {
        if (!isPlayerInTheRoom)
        {
            return;
        }
        Player player = GetNode<Player>("Player");
        Vector2 velocity = enemyBase.Speed * delta * (Position - player.Position);
        MoveAndCollide(velocity);
    }
}
