using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace script
{
  
    public class Cell {
        public Vector2Int Pos;
        public Vector3 Offset;
        public bool Stat;
        public bool IsBlock;
        public TestCell DebugCell;
        public static Color ColorFree = new Color(0,1,0,0.5f);
        public static Color ColorBlock = new Color(1,0,0,0.5f);
        public static Color ColorDesctuctible = new Color(0,0,1,0.5f);

        public int MoveCost;
        public int TotalMoveCost;
        [NonSerialized]public Cell FromCell;
        public Vector3 DirectionTarget;
        [NonSerialized]public Chunk Chunk;

        public int Gcost;
        public int Hcost;
        public int Fcost {
            get => Gcost + Hcost;
        }

        public Cell AStartFrom;
        public Vector3 WordPos {
            get => new Vector3(Pos.x+Offset.x, Offset.y, Pos.y+Offset.z);
        }
        public Cell(int x, int y, Vector3 offset) {
            Pos = new Vector2Int(x, y);
            Offset = offset;
        }
        public Cell(Vector2Int pos, Vector3 offset) {
            Pos = pos;
            Offset = offset;
        }

        public void ColorDebugCell(Color col) {
            if (DebugCell == null) return;
            DebugCell.Render.color = col;
        }

        public void CheckCellColliders() {
            IsBlock = false;
           // Collider[]cols =Physics.OverlapBox(WordPos,Vector3.one/2f);
            Collider[]cols =Physics.OverlapBox(WordPos,Metrics.cellColliderBockSize);
            if (cols.Length > 0) {
                foreach (var col in cols) {
                    if (col.gameObject.CompareTag("WalkBlocker")) {
                        Stat = true;
                        IsBlock = true;
                        MoveCost = GridManager.BlockMoveCostMoveCost;
                        ColorDebugCell(ColorBlock );
                        return;
                    }
                    if (col.gameObject.CompareTag("Destructible")) {
                        Stat = true;
                        MoveCost = GridManager.DestructibleMoveCost;
                        ColorDebugCell(ColorDesctuctible );
                        return;
                    }
                }
            }
            else
            {
                Stat = false;
                ColorDebugCell(ColorFree );
                MoveCost = GridManager.FreeWalkMoveCos;
            }
        }

        public bool CheckCellColliderContain(GameObject target) {
            Collider[]cols =Physics.OverlapBox(WordPos,Metrics.cellColliderBockSize);
            if (cols.Length > 0) {
                foreach (var col in cols) {
                    if (col.gameObject==target) return true;
                }
            }
            return false;
        }
        public void ClearPathFindingData() {
            TotalMoveCost = Int32.MaxValue;
            FromCell = null;
        }

        public CellSave GetCellSave() =>new CellSave(this);
        
    }
}