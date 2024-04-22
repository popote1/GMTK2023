using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnnemyAutoSpawner : UnitStartOrder {
    [SerializeField] private GridAgent[] _agentsToSpawn;
    [Range(1,5)][SerializeField] private float _spawnZoneSize=1;
    [SerializeField] private LayerMask _groundLayerMask ;
    
    [ContextMenu("Spawn Agents")]
    public void SpawnUnits() {
        List<GridAgent> spawnedAgents = new List<GridAgent>();
        foreach (var agent in _agentsToSpawn) {
            if (agent == null) continue;
            spawnedAgents.Add(Instantiate(agent, GetSpawnPos(), quaternion.identity));
        }
        GiveAgentMoveOrder(spawnedAgents);
    }

    private Vector3 GetSpawnPos() {
        if (Targets[0] == null) return Vector3.zero;
        Vector3 offset = new Vector3(Random.Range(-_spawnZoneSize, _spawnZoneSize), 0,
            Random.Range(-_spawnZoneSize, _spawnZoneSize));
        
        Vector3 pos =Targets[0].position + offset;
        RaycastHit hit;
        if (Physics.Raycast(new Ray(pos, Vector3.down), out hit, 10, _groundLayerMask))
        {
            return hit.point + Vector3.up * 0.5f;
        }
        return pos;
    } 
    private void OnDrawGizmos()
    {
        if (Targets == null) return;
        
        if (Targets.Length > 0&& EditorControlStatics.DisplayGizmos) {
            
            for (int i = 0; i < Targets.Length; i++) {
                if (Targets[i] == null) continue;
                if (i == 0) {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(Targets[i].position, Vector3.one*_spawnZoneSize);
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