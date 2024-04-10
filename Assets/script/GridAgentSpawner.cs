using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridAgentSpawner : MonoBehaviour
{

    [SerializeField] private float _radius;
    [SerializeField] private GridAgent[] _agentToSpawn;
    
    [SerializeField] private UnitStartOrder _unitStartOrder;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    [ContextMenu("Spanw And Order")]
    private void SpawnAndOrder() {
        List<GridAgent> agens = SpawnAgent();
        if (_unitStartOrder == null) return;
        _unitStartOrder.GiveAgentMoveOrder(agens);
        
    }

    [ContextMenu("Spawn Agents")]
    private void DebugSpawnAgents() => SpawnAgent();
    private List<GridAgent> SpawnAgent() {
        List<GridAgent> agents = new List<GridAgent>();
        foreach (GridAgent agent in _agentToSpawn) {
            if (agent == null) return null;
            Vector3 pos = transform.position +
                          new Vector3(Random.Range(-_radius, _radius), 0, Random.Range(-_radius, _radius));
            agents.Add(Instantiate(agent, pos, transform.rotation));
        }
        return agents;
    }
    
    

    private void OnDrawGizmos() {
        if (EditorControlStatics.DisplayGizmos) {
            Color col = Color.blue;
            col.a = 0.4f;
            Gizmos.color = col; 
            Gizmos.DrawSphere(transform.position, _radius);
        }
    }
}
