// using Godot;

// // Class for generating UI map of the level
// class MapUI
// {
//     // Offset of the map cell in the wolrd coordinates.
//     [Export]
//     private int rowDrawStep = 20;
//     [Export]
//     private int columnDrawStep = 20;

//     private Grid _map;

//     public MapUI(Grid map)
//     {
//         this._map = map;
//     }

//     // Draws UI representation of the level.
//     private void DrawStageMap()
//     {
//         if (_map == null || _map.IsEmpty())
//         {
//             GD.PushError("Tried to draw in the empty grid!");
//             return;
//         }

//         for (int i = 0; i < _map.rooms.Count; i++)
//         {
//             CreateMapCell(_map.rooms[i]);
//         }

//         // Mark starting room.
//         _map.rooms[_startingRoomIndex].AddChild(_cellTemplates.CurrentCell.Instance<Sprite>());

//     }

//     private void InstantiateEmptyRooms()
//     {
//         _map = new Grid(GridSize);

//         DrawLevel();

//         SetStartingRoomCell(StartingRoomScene.Instance<Cell>());

//         // Iterate through each cell that has neighbours and instantiate them.
//         GenerateNeighbours(_map.rooms[_startingRoomIndex]);
//     }
// }