using Godot;
using System;
using System.Diagnostics;

public class Main : Node
{
    [Export]
    public PackedScene MobScene;

    public override void _Ready()
    {
        GD.Randomize();
    }

    public void OnMobTimerTimeout()
    {
        // Create a new instance of the Mob scene.
        Mob mob = (Mob)MobScene.Instance();
        GD.Print(mob);

        // Choose a random location on the SpawnPath.
        // We store the reference to the SpawnLocation node.
        var mobSpawnLocation = GetNode<PathFollow>("SpawnPath/SpawnLocation");
        // And give it a random offset.
        mobSpawnLocation.UnitOffset = GD.Randf();

        Vector3 playerPosition = GetNode<Player>("Player").Transform.origin;
        mob.Initialize(mobSpawnLocation.GlobalTranslation, playerPosition);

        GD.Print("Player coordinates: " + playerPosition);

        AddChild(mob);
    }
}
