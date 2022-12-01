using Godot;
using System;

public interface Enemy
{

}

public class EnemyBase : Enemy
{
    [Export] public int Health;
    [Export] public int Damage;
    [Export] public float Speed;
}