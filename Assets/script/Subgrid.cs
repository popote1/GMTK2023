using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace script
{
    public class Subgrid :ICloneable
    {
        public Vector2Int Size;
        public Vector3 Offset;

        [NonSerialized] public  Subgrid NextSubGrid;
        
        private Cell[,] _cells;
        private List<Chunk> _chunks = new List<Chunk>();
        private Cell[] _targetCellses;

        public Cell[] TargetCells {
            get => _targetCellses;
        }

        public Vector2Int TargetPos {
            get {
                Vector2Int some =Vector2Int.zero;
                foreach (var cell in _targetCellses) some += cell.Pos;
                return some / _targetCellses.Length;
            }
        }

        
        public void SetNextSubGrid(Subgrid subgrid)=> NextSubGrid = subgrid;
           
        

        public Subgrid GetLastSubgrid(int security=0)
        {
            if (security >= 10 || NextSubGrid == null) {
                return this;
            }
            else {
                security++;
                return NextSubGrid.GetLastSubgrid(security);
            }
        }

        public void GenerateSubGrid(Chunk[] chunks, Vector2Int size, Vector3 offset) {
            Size = size;
            Offset = offset;
            _chunks = chunks.ToList();
            _cells = new Cell[size.x*Metrics.chunkSize, size.y*Metrics.chunkSize];
            foreach (var chunk in chunks) {
                foreach (var saveCell in chunk.cells) {
                    Cell cell = _cells[saveCell.Pos.x, saveCell.Pos.y] = new Cell(saveCell.Pos, saveCell.Offset);
                    cell.IsBlock = saveCell.IsBlock;
                    cell.MoveCost = saveCell.MoveCost;
                }
            }
        }

        public void AddChunksToSubGrid(Chunk[] newChunks) {
            foreach (var chunk in newChunks)
            {
                if (_chunks.Contains(chunk)) continue;
                foreach (var saveCell in chunk.cells) {
                    Cell cell = _cells[saveCell.Pos.x, saveCell.Pos.y] = new Cell(saveCell.Pos, saveCell.Offset);
                    cell.IsBlock = saveCell.IsBlock;
                    cell.MoveCost = saveCell.MoveCost; 
                }
            }
            StartCalcFlowfield(_targetCellses);
        }
        
        public Cell GetCellFromPos(int x, int y) {
            if (x < 0 || x >= Size.x*Metrics.chunkSize || y < 0 || y >= Size.y*Metrics.chunkSize) return null;
            return _cells[x, y];
        }
        public Cell GetCellFromPos(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= Size.x * Metrics.chunkSize || pos.y < 0 ||
                pos.y >= Size.y * Metrics.chunkSize) return null;
            return _cells[pos.x, pos.y];
        }
        
        public Cell GetCellFromWorldPos(Vector3 pos) {
            pos =pos - Offset;
            return GetCellFromPos(Mathf.RoundToInt(pos.x ), Mathf.RoundToInt(pos.z ));
        }
        private Cell[] GetNeighbors(Cell cell) {
            Cell[] neighbors = new Cell[8];
            neighbors[0] = GetCellFromPos(cell.Pos.x - 1, cell.Pos.y + 1);
            neighbors[1] = GetCellFromPos(cell.Pos.x , cell.Pos.y + 1);
            neighbors[2] = GetCellFromPos(cell.Pos.x + 1, cell.Pos.y + 1);
            neighbors[3] = GetCellFromPos(cell.Pos.x + 1, cell.Pos.y);
            neighbors[4] = GetCellFromPos(cell.Pos.x + 1, cell.Pos.y - 1);
            neighbors[5] = GetCellFromPos(cell.Pos.x , cell.Pos.y - 1);
            neighbors[6] = GetCellFromPos(cell.Pos.x - 1, cell.Pos.y - 1);
            neighbors[7] = GetCellFromPos(cell.Pos.x - 1, cell.Pos.y );
            return neighbors;
        }
        
        public void StartCalcFlowfield(Cell[] origin) {
            CalculatFlowFieldC(origin);
            _targetCellses = origin;
        }

        public void StartAttackBuilding(Cell[] targets) {
             Vector2Int some = Vector2Int.zero;
             foreach (var cell in targets) some += cell.Pos;
             Vector2 center = some / targets.Length;
             foreach (var cell in targets) {
                 if (GetCellFromPos(cell.Pos) != null) {
                     GetCellFromPos(cell.Pos).DirectionTarget = center - cell.Pos;
                 }
             }
             CalculatFlowFieldC(targets);
        }
        
        public void CalculatFlowFieldC(Cell origin)
                {
                    int counter=0;
                    foreach (var cell in _cells) {
                        if(cell== null) continue;
                        cell.ClearPathFindingData();
                    }
        
                    List<Cell> openList = new List<Cell>();
                    origin.MoveCost = 0;
                    origin.TotalMoveCost = 0;
                    openList.Add(origin);
                    while (openList.Count>0) {
                        //if (counter >= Metrics.FlowFieldCellPerFrame) {
                        //    counter = 0;
                        //    yield return new WaitForSeconds(0.01f);
                        //}
                        counter++;
                        Cell cell = openList[0];
                        Cell[] neighbors = GetNeighbors(cell);
        
                        for (int i = 0; i < neighbors.Length; i++) {
                            if (neighbors[i] == null) continue;
        
                            int movecost = Metrics.MoveCost;
                            if (i == 0 || i == 2 || i == 4 || i == 6) movecost = Metrics.DiagonalMoveCost;
                            if (neighbors[i].TotalMoveCost > cell.TotalMoveCost + movecost+neighbors[i].MoveCost) {
                                neighbors[i].TotalMoveCost = cell.TotalMoveCost + movecost+neighbors[i].MoveCost;
                                neighbors[i].FromCell = cell;
                                neighbors[i].DirectionTarget = ((Vector2) (cell.Pos - neighbors[i].Pos)).normalized;
                                if (!openList.Contains(neighbors[i]))openList.Add(neighbors[i]); 
                            }
                        }
                        openList.Remove(cell);
                    }
                    Debug.Log("FlowField Calculated");
                }
        public void CalculatFlowFieldC(Cell[] origin) {
            int counter=0;
            foreach (var cell in _cells) {
                if(cell== null) continue;
                cell.ClearPathFindingData();
            }
        
            List<Cell> openList = new List<Cell>();
            foreach (Cell cell in origin) {
                cell.MoveCost = 0;
                cell.TotalMoveCost = 0; 
            }
            
            openList.AddRange(origin);
            while (openList.Count>0) {
                //if (counter >= Metrics.FlowFieldCellPerFrame) {
                //    counter = 0;
                //    yield return new WaitForSeconds(0.01f);
                //}
                counter++;
                Cell cell = openList[0];
                Cell[] neighbors = GetNeighbors(cell);
        
                for (int i = 0; i < neighbors.Length; i++) {
                    if (neighbors[i] == null) continue;
        
                    int movecost = Metrics.MoveCost;
                    if (i == 0 || i == 2 || i == 4 || i == 6) movecost = Metrics.DiagonalMoveCost;
                    if (neighbors[i].TotalMoveCost > cell.TotalMoveCost + movecost+neighbors[i].MoveCost) {
                        neighbors[i].TotalMoveCost = cell.TotalMoveCost + movecost+neighbors[i].MoveCost;
                        neighbors[i].FromCell = cell;
                        neighbors[i].DirectionTarget = ((Vector2) (cell.Pos - neighbors[i].Pos)).normalized;
                        if (!openList.Contains(neighbors[i]))openList.Add(neighbors[i]); 
                    }
                }
                openList.Remove(cell);
            }
            
        }

        public object Clone() {
            Subgrid clone = new Subgrid();
            clone._cells = _cells;
            clone._chunks = _chunks;
            clone._targetCellses = TargetCells;
            clone.Offset = Offset;
            clone.NextSubGrid = NextSubGrid;
            clone.Size = Size;
            return clone;
        }
    }
}