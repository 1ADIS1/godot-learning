using Godot;
using System;

public class Cell : Sprite
{
    public Cell(Texture texture)
    {
        Texture = texture;
        GD.Print("Instantiated cell with texture: {0}", texture);
    }
}

enum RoomTypes
{
    CROSS_ROOM,
    TUNNEL_ROOM,
}
