using Godot;
using System;

public class Player : KinematicBody2D
{
    [Export] public float Speed;

    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _Process(float delta)
    {
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

        MoveAndCollide(direction * Speed * delta);
    }
}
