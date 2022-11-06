using Godot;
using System;

public class Mob : KinematicBody
{
    [Export]
    public float MaxSpeed = 18f;

    [Export]
    public float MinSpeed = 10f;

    private Vector3 _velocity = Vector3.Zero;

    public override void _PhysicsProcess(float delta)
    {
        MoveAndSlide(_velocity);
    }

    // We will call this function from the Main scene
    public void Initialize(Vector3 startPosition, Vector3 playerPosition)
    {
        // We position the mob and turn it so that it looks at the player.
        LookAtFromPosition(startPosition, playerPosition, Vector3.Up);
        // And rotate it randomly so it doesn't move exactly toward the player.
        RotateY((float)GD.RandRange(-Mathf.Pi / 4.0, Mathf.Pi / 4.0));

        // We calculate a random speed.
        float randomSpeed = (float)GD.RandRange(MinSpeed, MaxSpeed);
        // We calculate a forward velocity that represents the speed.
        _velocity = Vector3.Forward * randomSpeed;
        // We then rotate the vector based on the mob's Y rotation to move in the direction it's looking
        _velocity = _velocity.Rotated(Vector3.Up, Rotation.y);
    }

    // We also specified this function name in PascalCase in the editor's connection window
    public void OnVisibilityNotifierScreenExited()
    {
        QueueFree();
    }
}
