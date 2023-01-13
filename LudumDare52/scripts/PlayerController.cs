using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
    [Export] private float MovementSpeed = 20f;
    [Export] private float MouseSensitivity = 0.002f; // radians/pixel
    [Export] private float MaxCameraRotationX = 1.2f;
    [Export] private float FallAcceleration = 10f;
    [Export] private float InteractionDistance;
    [Export] private Node3D CameraPivot;
    [Export] private Camera3D Camera;
    [Export] private RayCast3D rayCast3D;
    [Export] public MeshInstance3D meat;
    [Export] private float CameraShakeAmount;
    [Export] private float CameraShakeDuration;

    private Vector3 _velocity;

    public override void _Ready()
    {
        meat.Visible = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsOnFloor())
        {
            _velocity.y -= FallAcceleration * (float)delta;
        }

        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_backwards");
        Vector3 direction = (Transform.basis * new Vector3(inputDirection.x, 0, inputDirection.y)).Normalized();
        if (direction != Vector3.Zero)
        {
            _velocity.x = direction.x * MovementSpeed;
            _velocity.z = direction.z * MovementSpeed;
        }
        else
        {
            var tempVelocity = _velocity.MoveToward(direction, MovementSpeed);
            _velocity.x = tempVelocity.x;
            _velocity.z = tempVelocity.z;
        }

        Velocity = _velocity;
        MoveAndSlide();

        HandleInteractions();
    }

    // Check for interaction with objects
    private void HandleInteractions()
    {
        if (rayCast3D.IsColliding())
        {
            var collider = rayCast3D.GetCollider();
            if (collider.HasMethod("Interact") && Input.IsActionJustPressed("interact"))
            {
                collider.Call("Interact");
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            RotateY(-mouseMotion.Relative.x * MouseSensitivity);
            CameraPivot.RotateX(-mouseMotion.Relative.y * MouseSensitivity);
            CameraPivot.Rotation = CameraPivot.Rotation.Clamp(new Vector3(-MaxCameraRotationX, 0, 0),
                new Vector3(MaxCameraRotationX, 0, 0));
        }
    }

    public void ShakeCamera()
    {
        var initOffset = new Vector2(Camera.HOffset, Camera.VOffset);
        var cameraTween = CreateTween();

        RandomNumberGenerator random = new RandomNumberGenerator();

        cameraTween.SetParallel(true);
        cameraTween.TweenProperty(Camera, "h_offset", random.RandfRange(-0.7f, 0.7f) * CameraShakeAmount, CameraShakeDuration);
        cameraTween.TweenProperty(Camera, "v_offset", random.RandfRange(-0.7f, 0.7f) * CameraShakeAmount, CameraShakeDuration);
        cameraTween.Chain().TweenProperty(Camera, "h_offset", 0, CameraShakeDuration);
        cameraTween.TweenProperty(Camera, "v_offset", 0, CameraShakeDuration);

        cameraTween.Play();
    }
}
