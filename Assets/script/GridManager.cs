using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using Random = UnityEngine.Random;

namespace script
{
    public class GridManager : MonoBehaviour
    {
        public Vector2Int Size;
        public Vector3 Offset;
        public TestCell PrefabsDebugCell;
        public static int FreeWalkMoveCos = 1;
        public static int BlockMoveCostMoveCost = 1000;
        public static int DestructibleMoveCost = 20;
        [Header("FlowFiled")] public bool DisplayDebugDirection;
        public int MoveCost = 10;
        public int DiagonalMoveCost = 14;
        public LayerMask LayerMaskGrund;
        public Cell Origin;
        public bool IsCalculating;
        public int CellParFrame = 500;
        public bool DisplayChunkLinks;
        [Header(" SaveData")] public TerrainLocomotionData TerrainLocomotionData;

        private List<Chunk> _Chunks;
        private Cell[,] _cells;

        public static GridManager Instance;

        public static Action OnClearPathFindingData;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning(" GridManager Déjà référencer");
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            GenerateCells();
            CheckColliders();
        }

        public void Start()
        {
            
        }

        public void Update()
        {
            if (DisplayDebugDirection) DisplayDebugDirectionfunction();
            if (DisplayChunkLinks) DisplayDebugChunkLinks();
        }

        #region Accessor

        public Cell GetCellFromPos(int x, int y)
        {
            if (x < 0 || x >= Size.x * Metrics.chunkSize || y < 0 || y >= Size.y * Metrics.chunkSize) return null;
            return _cells[x, y];
        }

        public Cell GetCellFromPos(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= Size.x * Metrics.chunkSize || pos.y < 0 ||
                pos.y >= Size.y * Metrics.chunkSize) return null;
            return _cells[pos.x, pos.y];
        }

        public Cell GetCellFromWorldPos(Vector3 pos)
        {
            pos = pos - Offset;
            return GetCellFromPos(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
        }

        public Cell[] GetCellsFromTransforms(Transform[] transforms) {
            List<Cell> cells = new List<Cell>();
            for (int i = 0; i < transforms.Length; i++) {
                if (transforms[i] == null) continue;
                Cell cell = GetCellFromWorldPos(transforms[i].position);
                if(!cells.Contains(cell))cells.Add(cell);
            }
            return cells.ToArray();
        }

        public Chunk GetChunk(int x, int y)
        {
            if (x < 0 || x >= Size.x || y < 0 || y >= Size.y) return null;
            int i = x * Size.y + y;
            return _Chunks[i];
        }

        public List<Chunk> GetNeighborsOfPath(List<Chunk> path)
        {
            List<Chunk> neighbors = new List<Chunk>();
            foreach (var chunk in path)
            {
                foreach (var neighbor in chunk.neighbors)
                {
                    if (neighbor == null) continue;
                    if (path.Contains(neighbor)) continue;
                    if (neighbors.Contains(neighbor)) continue;
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        public List<Cell> GetCellNeighborRange(Cell origin, int range) {
            List<Cell> returnCells = new List<Cell>();
            Cell current = null;
            int xMin = Mathf.Clamp(origin.Pos.x - range, 0, Size.x*Metrics.chunkSize);
            int xMax = Mathf.Clamp(origin.Pos.x + range+1, 0, Size.x*Metrics.chunkSize);
            int yMin = Mathf.Clamp(origin.Pos.y - range, 0, Size.y*Metrics.chunkSize);
            int yMax = Mathf.Clamp(origin.Pos.y + range+1, 0, Size.y*Metrics.chunkSize);
            
            for (int x = xMin; x < xMax; x++)
            {
                for (int y = yMin; y < yMax; y++)
                {
                    current = GetCellFromPos(x, y);
                    
                    if (current != null && !returnCells.Contains(current) && current != origin) ;
                    returnCells.Add(current);
                }
            }

            return returnCells;
        }

        private Chunk GetLowestFcost(List<Chunk> chunks)
        {
            int fcost = Int32.MaxValue;
            Chunk bestChoise = null;
            foreach (var chunk in chunks)
            {
                if (chunk.Fcost <= fcost)
                {
                    fcost = chunk.Fcost;
                    bestChoise = chunk;
                }
            }

            return bestChoise;
        }

        private Cell[] GetNeighbors(Cell cell)
        {
            Cell[] neighbors = new Cell[8];
            neighbors[0] = GetCellFromPos(cell.Pos.x - 1, cell.Pos.y + 1);
            neighbors[1] = GetCellFromPos(cell.Pos.x, cell.Pos.y + 1);
            neighbors[2] = GetCellFromPos(cell.Pos.x + 1, cell.Pos.y + 1);
            neighbors[3] = GetCellFromPos(cell.Pos.x + 1, cell.Pos.y);
            neighbors[4] = GetCellFromPos(cell.Pos.x + 1, cell.Pos.y - 1);
            neighbors[5] = GetCellFromPos(cell.Pos.x, cell.Pos.y - 1);
            neighbors[6] = GetCellFromPos(cell.Pos.x - 1, cell.Pos.y - 1);
            neighbors[7] = GetCellFromPos(cell.Pos.x - 1, cell.Pos.y);
            return neighbors;
        }

        private Cell[] Get4Neighbors(Cell cell)
        {
            Cell[] neighbors = new Cell[4];
            neighbors[0] = GetCellFromPos(cell.Pos.x, cell.Pos.y + 1);
            neighbors[1] = GetCellFromPos(cell.Pos.x + 1, cell.Pos.y);
            neighbors[2] = GetCellFromPos(cell.Pos.x, cell.Pos.y - 1);
            neighbors[3] = GetCellFromPos(cell.Pos.x - 1, cell.Pos.y);
            return neighbors;
        }

        private int[] GetNeighborsID(Chunk chunk)
        {
            int[] ids = new int[chunk.neighbors.Count];
            for (int i = 0; i < chunk.neighbors.Count; i++)
            {
                ids[i] = _Chunks.IndexOf(chunk.neighbors[i]);
            }

            return ids;
        }

        private List<List<Cell>> GetBlockedCells(Chunk chunk, List<List<Cell>> rooms)
        {
            Dictionary<Cell, int> classedCell = new Dictionary<Cell, int>();
            List<List<Cell>> newClassed = new List<List<Cell>>();
            for (int i = 0; i < rooms.Count; i++)
            {
                newClassed.Add(new List<Cell>());
            }

            for (int i = 0; i < rooms.Count; i++)
            {
                foreach (Cell cell in rooms[i])
                {
                    classedCell.Add(cell, i);
                }
            }

            foreach (Cell cell in chunk.cells)
            {
                if (classedCell.Keys.Contains(cell)) continue;
                newClassed[GetClosestRoom(cell, classedCell)].Add(cell);
            }

            for (int i = 0; i < newClassed.Count; i++)
            {
                if (newClassed.Count <= 0) continue;
                rooms[i].AddRange(newClassed[i]);
            }

            return rooms.ToList();
        }

        private int GetClosestRoom(Cell cell, Dictionary<Cell, int> classify)
        {
            float bestDistance = float.PositiveInfinity;
            int bestRoom = 1;
            foreach (var cellsC in classify)
            {
                if (Vector2.Distance(cell.Pos, cellsC.Key.Pos) < bestDistance)
                {
                    bestDistance = Vector2.Distance(cell.Pos, cellsC.Key.Pos);
                    bestRoom = cellsC.Value;
                }
            }

            return bestRoom;
        }

        public Cell[] GetBreathFirstCells(Cell originCell, int range)
        {
            List<Cell> openList = new List<Cell>();
            List<Cell> closeList = new List<Cell>();
            List<Cell> toAddList = new List<Cell>();
            openList.Add(originCell);

            for (int i = 0; i < range; i++)
            {
                foreach (Cell cell in openList)
                {
                    foreach (Cell neighbor in Get4Neighbors(cell))
                    {
                        if (cell.IsBlock) continue;
                        if (closeList.Contains(neighbor)) continue;
                        if (openList.Contains(neighbor)) continue;
                        if (toAddList.Contains(neighbor)) continue;
                        toAddList.Add(neighbor);
                    }
                }

                closeList.AddRange(openList);
                openList.Clear();
                openList.AddRange(toAddList);
                toAddList.Clear();
            }

            closeList.AddRange(toAddList);
            return closeList.ToArray();
        }

        private List<Cell> GetCellRoom(Cell originCell, Chunk chunk)
        {
            List<Cell> OpenList = new List<Cell>();
            List<Cell> CLoseList = new List<Cell>();
            OpenList.Add(originCell);
            while (OpenList.Count > 0)
            {
                Cell current = OpenList[0];
                OpenList.Remove(current);
                CLoseList.Add(current);
                foreach (var neighbor in GetNeighbors(current))
                {
                    if (!chunk.cells.Contains(neighbor)) continue;
                    if (neighbor.IsBlock) continue;
                    if (CLoseList.Contains(neighbor)) continue;
                    if (OpenList.Contains(neighbor)) continue;
                    OpenList.Add(neighbor);
                }
            }

            return CLoseList;
        }

        private List<Chunk> GetChunksNeighbors(Chunk chunk)
        {
            List<Chunk> chunks = new List<Chunk>();
            foreach (Cell cell in chunk.cells)
            {
                foreach (var neighbor in Get4Neighbors(cell))
                {
                    if (neighbor == null) continue;
                    if (neighbor.IsBlock) continue;
                    if (chunk.cells.Contains(neighbor)) continue;
                    if (neighbor.Chunk == chunk) continue;
                    if (chunks.Contains(neighbor.Chunk)) continue;

                    chunks.Add(neighbor.Chunk);
                }
            }

            return chunks;
        }
        
        #endregion

        #region opperator

        public List<Chunk> GetAStartPath(Chunk origin, Chunk target)
        {
            List<Chunk> openList = new List<Chunk>();
            List<Chunk> closeList = new List<Chunk>();

            origin.FromChunk = null;
            origin.Gcost = 0;
            openList.Add(origin);

            while (openList.Count > 0)
            {
                Chunk current = GetLowestFcost(openList);
                openList.Remove(current);
                foreach (var neighbor in current.neighbors)
                {
                    if (neighbor == null) continue;
                    if (neighbor == target)
                    {
                        neighbor.FromChunk = current;
                        return ReturnAStartResult(origin, target);
                    }

                    if (neighbor.Gcost > current.Gcost + 1) neighbor.Gcost = current.Gcost + 1;

                    if (openList.Contains(neighbor)) continue;
                    if (closeList.Contains(neighbor)) continue;

                    neighbor.Hcost = Mathf.FloorToInt(Vector2.Distance(neighbor.Coordonate, target.Coordonate) * 10);
                    //neighbor.Hcost = Mathf.Abs(neighbor.Coordonate.x - target.Coordonate.x) +
                    //                 Mathf.Abs(neighbor.Coordonate.y - target.Coordonate.y);
                    neighbor.FromChunk = current;
                    openList.Add(neighbor);
                }

                closeList.Add(current);
            }

            return null;
        }

        private List<Chunk> ReturnAStartResult(Chunk start, Chunk target)
        {
            Chunk current = target;
            List<Chunk> returnlist = new List<Chunk>();
            while (current != null && target != start)
            {
                returnlist.Add(current);
                current = current.FromChunk;
            }

            returnlist.Add(start);
            return returnlist;
        }

        [ContextMenu("GenerateGrid")]
        public void GenerateCells()
        {
            ClearGrid();
            _Chunks = new List<Chunk>();
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    Chunk chunk = new Chunk(x, y);
                    _Chunks.Add(chunk);
                    if (x > 0)
                    {
                        Chunk neighbor = GetChunk(x - 1, y);
                        neighbor.neighbors.Add(chunk);
                        chunk.neighbors.Add(neighbor);
                    }

                    if (y > 0)
                    {
                        Chunk neighbor = GetChunk(x, y - 1);
                        neighbor.neighbors.Add(chunk);
                        chunk.neighbors.Add(neighbor);
                    }
                }
            }

            _cells = new Cell[Size.x * Metrics.chunkSize, Size.y * Metrics.chunkSize];
            for (int x = 0; x < Size.x * Metrics.chunkSize; x++)
            {
                for (int y = 0; y < Size.y * Metrics.chunkSize; y++)
                {
                    //Debug.Log(x+";"+ y);
                    _cells[x, y] = new Cell(x, y, Offset);
                    _cells[x, y].Chunk = GetChunk(x / Metrics.chunkSize, y / Metrics.chunkSize);
                    GetChunk(x / Metrics.chunkSize, y / Metrics.chunkSize).cells.Add(_cells[x, y]);
                }
            }

            foreach (var chunk in _Chunks)
            {
                chunk.CalculatCenter();
            }
        }

        [ContextMenu("Display DebugCells")]
        public void GenerateDebugCells()
        {
            foreach (var cell in _cells) {
                cell.DebugCell = Instantiate(PrefabsDebugCell, cell.WordPos, Quaternion.identity);
                cell.DebugCell.transform.SetParent(transform);

                if (Metrics.UsDebugCellGroundOffsetting)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(cell.DebugCell.transform.position + new Vector3(0, 50, 0), Vector3.down,
                        out hit, LayerMaskGrund))
                    {
                        cell.DebugCell.transform.position = hit.point+new Vector3(0,0.5f,0);
                    }
                    
                }

            }
        }

        [ContextMenu("CheckColliders")]
        public void CheckColliders() {
            OnClearPathFindingData?.Invoke();
            ColorAllDebugGridToColor(Color.white);
            foreach (var cell in _cells) {
                cell.CheckCellColliders();
            }
        }

        public void DisplayDebugDirectionfunction()
        {
            foreach (var cell in _cells)
            {
                Debug.DrawLine(cell.WordPos,
                    (new Vector3(cell.DirectionTarget.x, 0, cell.DirectionTarget.y) * 0.8f) + cell.WordPos);
            }
        }

        public void DisplayDebugChunkLinks()
        {
            foreach (var chunk in _Chunks)
            {
                if (chunk == null)
                {
                    Debug.Log("Chunk is null");
                    continue;
                }

                foreach (var neighbor in chunk.neighbors)
                {
                    if (neighbor == null)
                    {
                        Debug.DrawLine(chunk.Center, chunk.Center + new Vector3(3f, 3f), Color.red);
                        continue;
                    }

                    Debug.DrawLine(chunk.Center, Vector3.Lerp(chunk.Center, neighbor.Center, 0.4f), Color.blue);
                }
            }
        }

        [ContextMenu("colorChunk")]
        public void ColorChunks()
        {
            foreach (var chunk in _Chunks)
            {
                Color col = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1);
                foreach (var cell in chunk.cells)
                {
                    cell.ColorDebugCell(col);
                }
            }
        }
        
        [ContextMenu("Clear DebugCells")]
        public void ClearGrid()
        {
            if (_cells != null)
            {
                foreach (var cell in _cells)
                {
                    if (cell == null) continue;
                    if (cell.DebugCell == null) continue;
                    DestroyImmediate(cell.DebugCell.gameObject);
                }
            }

            for (int i = transform.childCount - 1; i > 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        public void ColorAllDebugGridToColor(Color color)
        {
            foreach (var cell in _cells)
            {
                if (cell != null) cell.ColorDebugCell(color);
            }
        }

        [ContextMenu("recalculateChunks")]
        public void RecalculateChunks()
        {
            List<Cell> CloseList = new List<Cell>();
            List<List<Cell>> Rooms = new List<List<Cell>>();

            foreach (var chunk in _Chunks.ToList())
            {
                Rooms.Clear();
                CloseList.Clear();
                foreach (var cell in chunk.cells)
                {
                    if (cell == null) continue;
                    if (cell.IsBlock) continue;
                    if (CloseList.Contains(cell)) continue;
                    List<Cell> room = GetCellRoom(cell, chunk);
                    CloseList.AddRange(room);
                    Rooms.Add(room);
                }

                Debug.Log("Rooms founds = " + Rooms.Count);
                foreach (var neighbor in chunk.neighbors)
                {
                    neighbor.neighbors.Remove(chunk);
                }

                if (Rooms.Count > 1)
                {
                    Rooms = GetBlockedCells(chunk, Rooms);
                    Vector2Int originalPos = chunk.Coordonate;
                    foreach (var cell in chunk.cells)
                    {
                        cell.Chunk = null;
                    }

                    _Chunks.Remove(chunk);

                    foreach (var room in Rooms)
                    {
                        Chunk newChunk = new Chunk(chunk.Coordonate);
                        newChunk.cells = room;
                        newChunk.CalculatCenter();
                        newChunk.neighbors = GetChunksNeighbors(newChunk);
                        newChunk.Coordonate = originalPos;
                        _Chunks.Add(newChunk);

                        foreach (var cell in newChunk.cells)
                        {
                            cell.Chunk = newChunk;
                        }

                        foreach (var neighbor in newChunk.neighbors)
                        {
                            if (neighbor != null) neighbor.neighbors.Add(newChunk);
                        }
                    }
                }
                else if (Rooms.Count > 0)
                {
                    //chunk.cells.Clear();
                    //chunk.cells = Rooms[0];
                    chunk.CalculatCenter();
                    chunk.neighbors = GetChunksNeighbors(chunk);
                    chunk.Coordonate = chunk.Coordonate;
                    foreach (var neighbor in chunk.neighbors)
                    {
                        if (neighbor != null) neighbor.neighbors.Add(chunk);
                    }
                }
            }


        }
        
        #endregion

        #region SaveLoadSystem
        
        public void LoadLocomotionData()
        {
            if (TerrainLocomotionData == null)
            {
                Debug.LogWarning("There No TerrainLocomotionData");
                return;
            }

            ClearGrid();
            _cells = new Cell[Size.x * Metrics.chunkSize, Size.y * Metrics.chunkSize];

            foreach (var cell in TerrainLocomotionData.Cells)
            {
                _cells[cell.Pos.x, cell.Pos.y] = new Cell(cell.Pos, Offset);
                _cells[cell.Pos.x, cell.Pos.y].IsBlock = cell.IsBlock;
                if (cell.IsBlock)
                {
                    _cells[cell.Pos.x, cell.Pos.y].MoveCost = BlockMoveCostMoveCost;
                }
                
            }
            
            _Chunks = new List<Chunk>();

            foreach (var chunk in TerrainLocomotionData.Chunks)
            {
                _Chunks.Add(new Chunk(chunk.Pos));
            }

            foreach (var chunk in TerrainLocomotionData.Chunks)
            {
                _Chunks[chunk.Id].neighbors = new List<Chunk>();
                foreach (var neightbor in chunk.Neighbors)
                {
                    if (neightbor == -1) continue;
                    if (_Chunks[chunk.Id] == null)
                    {
                        continue;
                    }

                    if (_Chunks[neightbor] == null)
                    {
                        continue;
                    }

                    _Chunks[chunk.Id].neighbors.Add(_Chunks[neightbor]);
                }

                _Chunks[chunk.Id].cells = new List<Cell>();
                foreach (var cell in chunk.Cells)
                {
                    if (GetCellFromPos(cell.x, cell.y) == null)
                    {
                        Debug.LogWarning("Cell Don't Found at pos => " + cell.x + " ; " + cell.y);

                    }

                    _Chunks[chunk.Id].cells.Add(GetCellFromPos(cell.x, cell.y));
                    GetCellFromPos(cell.x, cell.y).Chunk = _Chunks[chunk.Id];
                }

            }

            foreach (var chunk in _Chunks)
            {
                chunk.CalculatCenter();
            }

            Debug.Log("Locomotion Data Loaded");
        }
        public void SaveLocomotionData()
        {
            if (TerrainLocomotionData == null)
            {
                TerrainLocomotionData = ScriptableObject.CreateInstance<TerrainLocomotionData>();
                TerrainLocomotionData.MapName = SceneManager.GetActiveScene().name;
                if (!Directory.Exists("Assets/LocomotionData"))
                {
                    Directory.CreateDirectory("Assets/LocomotionData");
                }

                UnityEditor.AssetDatabase.CreateAsset(TerrainLocomotionData,
                    "Assets/LocomotionData/TLD" + TerrainLocomotionData.MapName + ".asset");
            }

            TerrainLocomotionData.Size = Size;
            TerrainLocomotionData.Chunks = new ChunkSave[_Chunks.Count];
            for (int i = 0; i < TerrainLocomotionData.Chunks.Length; i++)
            {
                TerrainLocomotionData.Chunks[i] = new ChunkSave(i, _Chunks[i].GetCellPos(), GetNeighborsID(_Chunks[i]),
                    _Chunks[i].Coordonate);
            }

            TerrainLocomotionData.Cells = new CellSave[_cells.Length];


            for (int x = 0; x < Size.x * Metrics.chunkSize; x++)
            {
                for (int y = 0; y < Size.y * Metrics.chunkSize; y++)
                {
                    TerrainLocomotionData.Cells[x * Metrics.chunkSize * Size.y + y] = _cells[x, y].GetCellSave();

                }
            }

            Debug.Log("Data Sava");
        }
        
        #endregion
    }
}

