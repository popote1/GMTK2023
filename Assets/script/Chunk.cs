using System;
using System.Collections.Generic;
using UnityEngine;

namespace script
{
    [Serializable]
    public class Chunk {
        public Vector2Int Coordonate; 
        [NonSerialized] public List<Chunk> neighbors = new List<Chunk>();
        [NonSerialized]public List<Cell> cells = new List<Cell>();

        public Vector3 Center;
        public int Gcost;
        public int Hcost;
        public int Fcost {
            get => Gcost + Hcost;
        }
        [NonSerialized]public Chunk FromChunk;

        public Chunk(int x, int y) {
            Coordonate = new Vector2Int(x, y);
        }
        public Chunk(Vector2Int coor) {
            Coordonate = coor;
        }
        
        public void CalculatCenter() {
            Vector3 pos = Vector3.zero;
            foreach (var cell in cells) {
                pos += cell.WordPos;
            }
            Center = pos / cells.Count;
        }

        public Vector2Int[] GetCellPos() {
            Vector2Int[] pos = new Vector2Int[cells.Count];
            for (int i = 0; i < cells.Count; i++) {
                pos[i] = cells[i].Pos;
            }
            return pos;
        }
    }
}