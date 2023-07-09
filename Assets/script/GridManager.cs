using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace script {
    public class GridManager :MonoBehaviour
    {
        public Vector2Int Size;
        public Vector3 Offset;
        public TestCell PrefabsDebugCell;
        public static int  FreeWalkMoveCos=1;
        public static int BlockMoveCostMoveCost=1000;
        public static int DestructibleMoveCost=20;
        [Header("FlowFiled")]  
        public bool DisplayDebugDirection;
        public int MoveCost=10;
        public int DiagonalMoveCost = 14;

        public Cell Origin;
        public bool IsCalculating;
        public int CellParFrame = 500; 
        private Cell[,] _cells;

        public static GridManager Instance;

        private void Awake()
        {
            if (Instance != null) {
                Debug.LogWarning(" GridManager Déjà référencer");
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
        }

        public void Start()
        {
            GenerateCells();
            CheckColliders();
        }
        
        public void Update() {
            if( DisplayDebugDirection)DisplayDebugDirectionfunction();
        }

        public Cell GetCellFromPos(int x, int y) {
            if (x < 0 || x >= Size.x || y < 0 || y >= Size.y) return null;
            return _cells[x, y];
        }

        public void ColorCell(Vector3 pos)
        {
            Cell cell = GetCellFromWorldPos(pos);
            if( cell !=null) cell.ColorDebugCell(Color.blue);
        }

        public Cell GetCellFromWorldPos(Vector3 pos) {
            pos =pos - Offset;
            return GetCellFromPos(Mathf.RoundToInt(pos.x ), Mathf.RoundToInt(pos.z ));
        }


        public void StartCalcFlowfield(Cell origin)
        {
            if (!IsCalculating)
            {
                StartCoroutine("CalculatFlowFieldC", origin);
                IsCalculating = true;
            }
        }

        IEnumerator CalculatFlowFieldC(Cell origin)
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
                if (counter >= CellParFrame) {
                    counter = 0;
                    yield return new WaitForSeconds(0.01f);
                }

                counter++;
                Cell cell = openList[0];
                Cell[] neighbors = GetNeighbors(cell);

                for (int i = 0; i < neighbors.Length; i++) {
                    if (neighbors[i] == null) continue;

                    int movecost = MoveCost;
                    if (i == 0 || i == 2 || i == 4 || i == 6) movecost = DiagonalMoveCost;
                    if (neighbors[i].TotalMoveCost > cell.TotalMoveCost + movecost+neighbors[i].MoveCost) {
                        neighbors[i].TotalMoveCost = cell.TotalMoveCost + movecost+neighbors[i].MoveCost;
                        neighbors[i].FromCell = cell;
                        neighbors[i].DirectionTarget = ((Vector2) (cell.pos - neighbors[i].pos)).normalized;
                        if (!openList.Contains(neighbors[i]))openList.Add(neighbors[i]); 
                    }
                }
                openList.Remove(cell);
            }

            IsCalculating = false;
        }
        public void CalculatFlowField(Cell origin)
        {
            foreach (var cell in _cells) {
                if(cell== null) continue;
                cell.ClearPathFindingData();
            }

            List<Cell> openList = new List<Cell>();
            origin.MoveCost = 0;
            origin.TotalMoveCost = 0;
            openList.Add(origin);
            
            
            while (openList.Count>0) {
                Cell cell = openList[0];
                Cell[] neighbors = GetNeighbors(cell);

                for (int i = 0; i < neighbors.Length; i++) {
                    if (neighbors[i] == null) continue;

                    int movecost = MoveCost;
                    if (i == 0 || i == 2 || i == 4 || i == 6) movecost = DiagonalMoveCost;
                    if (neighbors[i].TotalMoveCost > cell.TotalMoveCost + movecost+neighbors[i].MoveCost) {
                        neighbors[i].TotalMoveCost = cell.TotalMoveCost + movecost+neighbors[i].MoveCost;
                        neighbors[i].FromCell = cell;
                        neighbors[i].DirectionTarget = ((Vector2) (cell.pos - neighbors[i].pos)).normalized;
                        if (!openList.Contains(neighbors[i]))openList.Add(neighbors[i]); 
                    }
                }
                openList.Remove(cell);
            }
        }

        private bool Containe(List<Cell> list , Cell cell) {
            for (int i = 0; i < list.Count; i++) {
                if (list[i] == cell) return true;
            }

            return false;
        }

        private Cell[] GetNeighbors(Cell cell) {
            Cell[] neighbors = new Cell[8];
            neighbors[0] = GetCellFromPos(cell.pos.x - 1, cell.pos.y + 1);
            neighbors[1] = GetCellFromPos(cell.pos.x , cell.pos.y + 1);
            neighbors[2] = GetCellFromPos(cell.pos.x + 1, cell.pos.y + 1);
            neighbors[3] = GetCellFromPos(cell.pos.x + 1, cell.pos.y);
            neighbors[4] = GetCellFromPos(cell.pos.x + 1, cell.pos.y - 1);
            neighbors[5] = GetCellFromPos(cell.pos.x , cell.pos.y - 1);
            neighbors[6] = GetCellFromPos(cell.pos.x - 1, cell.pos.y - 1);
            neighbors[7] = GetCellFromPos(cell.pos.x - 1, cell.pos.y );
            return neighbors;
        }

        [ContextMenu("GenerateGrid")]
        public void GenerateCells()
        {
            ClearGrid();
            _cells = new Cell[Size.x,Size.y];
            for (int x = 0; x < Size.x; x++) {
                for (int y = 0; y < Size.y; y++) {
                    _cells[x,y] = new Cell(x,y,Offset);
                }
            }
        }

        [ContextMenu("Display DebugCells")]
        public void GenerateDebugCells() {
            foreach (var cell in _cells) {
                cell.DebugCell = Instantiate(PrefabsDebugCell, cell.WordPos, Quaternion.identity);
                cell.DebugCell.transform.SetParent(transform);
            }
        }
        
        [ContextMenu("CheckColliders")]
        public void CheckColliders() {
            foreach (var cell in _cells) {
                cell.CheckCellColliders();
            }
        }
        public void DisplayDebugDirectionfunction() {
            foreach (var cell in _cells) {
                Debug.DrawLine(cell.WordPos, (new Vector3(cell.DirectionTarget.x,0,cell.DirectionTarget.y)*0.8f)+cell.WordPos);
            }
        }
        
        

        [ContextMenu("Clear DebugCells")]
        public void ClearGrid() {
            if (_cells != null) {
                foreach (var cell in _cells) {
                    if (cell == null) continue;
                    if (cell.DebugCell == null) continue;
                    DestroyImmediate(cell.DebugCell.gameObject);
                }
            }

            for (int i = transform.childCount-1; i >0; i--) {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }

    public class Cell {
        public Vector2Int pos;
        public Vector3 Offset;
        public bool Stat;
        public TestCell DebugCell;
        public static Color ColorFree = new Color(0,1,0,0.5f);
        public static Color ColorBlock = new Color(1,0,0,0.5f);
        public static Color ColorDesctuctible = new Color(0,0,1,0.5f);

        public int MoveCost;
        public int TotalMoveCost;
        public Cell FromCell;
        public Vector3 DirectionTarget;
        public Vector3 WordPos {
            get => new Vector3(pos.x+Offset.x, Offset.y, pos.y+Offset.z);
        }
        public Cell(int x, int y, Vector3 offset) {
            pos = new Vector2Int(x, y);
            Offset = offset;
        }

        public void ColorDebugCell(Color col) {
            if (DebugCell == null) return;
            DebugCell.Render.color = col;
        }

        public void CheckCellColliders()
        {
            Collider[]cols =Physics.OverlapBox(WordPos,Vector3.one/2f);
            if (cols.Length > 0) {
                foreach (var col in cols) {
                    if (col.gameObject.CompareTag("WalkBlocker")) {
                        Stat = true;
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
        public void ClearPathFindingData() {
            TotalMoveCost = Int32.MaxValue;
            FromCell = null;
        }

        
    }
}