using System;
using UnityEngine;

namespace script {
    [Serializable]
    public struct ChunkSave {
        public int Id;
        public Vector2Int Pos;
        public Vector2Int[] Cells;
        public int[] Neighbors;

        public ChunkSave(int id , Vector2Int[] cells , int[] neighbors, Vector2Int pos) {
            Id = id;
            Cells = cells;
            Neighbors = neighbors;
            Pos = pos;
        }
    }
}