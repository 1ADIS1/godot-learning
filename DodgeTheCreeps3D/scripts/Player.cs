using Godot;
using System;

//TODO: slowly move between directions.
//TODO: upon landing on an enemy spawn the particles and play the sound.
//TODO: make "fish" movement for the player. 
public class Player : KinematicBody
{
    [Export]
    public float Speed = 14.0f;

    [Export]
    public float FallAcceleration = 75.0f;

    private Vector3 _velocity = Vector3.Zero;

    public override void _PhysicsProcess(float delta)
    {
        Vector3 direction = Vector3.Zero;

        if (Input.IsActionPressed("move_right"))
        {
            direction.x += 1f;
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction.x -= 1f;
        }
        if (Input.IsActionPressed("move_forward"))
        {
            direction.z -= 1f;
        }
        if (Input.IsActionPressed("move_back"))
        {
            direction.z += 1f;
        }

        if (direction != Vector3.Zero)
        {
            direction = direction.Normalized();
            GetNode<Spatial>("Pivot")?.LookAt(Translation + direction, Vector3.Up);
        }

        // Ground velocity
        _velocity.x = direction.x * Speed;
        _velocity.z = direction.z * Speed;
        // Vertical velocity
        _velocity.y -= FallAcceleration * delta;
        // Moving the character
        MoveAndSlide(_velocity, Vector3.Up);
    }
}
