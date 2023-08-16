using System.Collections;
using System.Collections.Generic;
using script;
using UnityEngine;

public class TerrainLocomotionData : ScriptableObject
{
    public string MapName;
    public Vector2Int Size;

    public ChunkSave[] Chunks;
    
    public CellSave[] Cells;
    
}
