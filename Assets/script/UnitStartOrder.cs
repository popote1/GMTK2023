using System.Collections.Generic;
using script;
using UnityEngine;

public class UnitStartOrder : MonoBehaviour
{
    
        public GridManager GridManager;
        public GridAgent[] Agents;
        public bool IsLoop;

        public Transform[] Targets;
        protected Vector2Int originChunkTarget = new Vector2Int(0, 0);
    
    
        void Start() {
            GridManager = GridManager.Instance;
            if (GridManager == null) {
                Debug.LogWarning("No Grid Manager Fount", this);
                return;
            }
        }

        public void GiveAgentMoveOrder(List<GridAgent> agents) {
            Agents = agents.ToArray();
            GiveUniteMoveOrder();
        }

        [ContextMenu("Give Simple Order")]
        public void GiveUniteMoveOrder() {
            if (Targets.Length > 0) {
                if( Targets.Length ==1) GiveSimpleMoveOrder(GridManager.GetCellFromWorldPos(Targets[0].position));
                else GiveChainMoveOrder(GridManager.GetCellsFromTransforms(Targets));
            }
        }
        
  
        public void GiveSimpleMoveOrder(Cell targetCell) {
            List<Chunk> totalChunks = GetChunksPathFromZombie(Agents,targetCell);
            if (totalChunks == null) return;
            Subgrid subgrid = new Subgrid();
            subgrid.GenerateSubGrid(totalChunks.ToArray(), GridManager.Size, GridManager.Offset);
            subgrid.StartCalcFlowfield(new []{targetCell});

            foreach (var zombieAgent in Agents) {
                zombieAgent.SetNewSubGrid(subgrid);
                Debug.Log("OrderGiven");
            }
        }

        public void GiveChainMoveOrder(Cell[]targetCells) {
            
            Subgrid firstOrder= new Subgrid();
            Subgrid previousOrder = new Subgrid();
            Subgrid[] subgrids = new Subgrid[targetCells.Length];
            for (int i = 0; i < targetCells.Length; i++) {
                List<Chunk> totalChunks = new List<Chunk>();
                if (i == 0) totalChunks = GetChunksPathFromZombie(Agents, targetCells[i]);
                else totalChunks = GetChunksPath(new[] {targetCells[i - 1]}, targetCells[i]);
                
                if (totalChunks == null) return;
                Subgrid currentSubgrid = new Subgrid();
                
                currentSubgrid.GenerateSubGrid(totalChunks.ToArray(), GridManager.Size, GridManager.Offset);
                currentSubgrid.StartCalcFlowfield(new []{targetCells[i]});


                //if (i == 0) firstOrder = currentSubgrid;
                //else previousOrder.NextSubGrid = currentSubgrid;
                subgrids[i] = currentSubgrid;
                if (i > 0) subgrids[i - 1].NextSubGrid = currentSubgrid;

                if (i == targetCells.Length - 1 && IsLoop) currentSubgrid.NextSubGrid = subgrids[0];
                //previousOrder = currentSubgrid;
                Debug.Log("Target index ="+i+ "  is at corrodonate"+ targetCells[i].WordPos);
            }
            
            
            foreach (var zombieAgent in Agents)
            {
                zombieAgent.SetNewSubGrid(subgrids[0]);
                Debug.Log("Chain Order Given");
            }
        }

        private List<Chunk> GetChunksPathFromZombie(GridAgent[] origins, Cell targetCell) {
            List<Cell> cells = new List<Cell>();
            for (int i = 0; i < origins.Length; i++) {
                if (origins[i] == null) continue;
                Cell cell = GridManager.GetCellFromWorldPos(origins[i].transform.position);
                if(!cells.Contains(cell))cells.Add(cell);
            }
            
            return GetChunksPath(cells.ToArray(), targetCell);
        }

        private List<Chunk> GetChunksPath(Cell[] Origin, Cell targetCell) {
            List<Chunk> startchunks = new List<Chunk>();
            List<Chunk> pathChunks = new List<Chunk>();
            foreach (var cell in Origin) {
                if (cell == null) continue;
                if (!startchunks.Contains(cell.Chunk)) startchunks.Add(cell.Chunk);
            }

            if (startchunks.Count == 0) {
                Debug.LogWarning("StartChunks not founds ");
                return null;
            }

            foreach (var chunk in startchunks) {
                foreach (var chunkpath in GridManager.GetAStartPath(chunk, targetCell.Chunk)) {
                    if (!pathChunks.Contains(chunkpath)) pathChunks.Add(chunkpath);
                }
            }
            pathChunks.AddRange(GridManager.GetNeighborsOfPath(pathChunks));
            return pathChunks;
        }
        private void OnDrawGizmos() {
            if (Targets.Length > 0&& EditorControlStatics.DisplayGizmos) {
                for (int i = 0; i < Targets.Length; i++) {
                    if (Targets[i] == null) continue;
                    if (i == 0) {
                        Gizmos.color = Color.green;
                        Gizmos.DrawCube(Targets[i].position, Vector3.one * 0.25f);
                        continue;
                    }

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(Targets[i].position, 0.25f);
                    if (Targets[i - 1] == null) continue;
                    Gizmos.DrawLine(Targets[i].position, Targets[i - 1].position);
                }

                if (IsLoop && Targets[0] != null && Targets[Targets.Length - 1] != null) {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(Targets[0].position, Targets[Targets.Length - 1].position);
                }
            }

        }
}