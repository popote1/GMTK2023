using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace script
{
    public class Subgrid 
    {
        public Vector2Int Size;
        public Vector3 Offset;
        private Cell[,] _cells;
        private List<Chunk> _chunks = new List<Chunk>();
        private Cell _targetCell;

        public Cell TargetCell {
            get => _targetCell;
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
            StartCalcFlowfield(_targetCell);
        }
        
        public Cell GetCellFromPos(int x, int y) {
            if (x < 0 || x >= Size.x*Metrics.chunkSize || y < 0 || y >= Size.y*Metrics.chunkSize) return null;
            return _cells[x, y];
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
        
        public void StartCalcFlowfield(Cell origin) {
            CalculatFlowFieldC(origin);
            _targetCell = origin;
            Debug.Log("Start CalculateFlowField");
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
    }
}