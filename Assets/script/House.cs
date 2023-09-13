using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace script
{
    public class House: MonoBehaviour, IDestructible
    {
        public GameObject prefabsDebrie;
        public ZombieAgent prefabZombit;
        public GridManager GridManager;
        public int HP;
        public GameObject PrefabsDestruciotnParticules;
        public List<Vector2Int> CellsCoordinates = new List<Vector2Int>();
        [Header("Spawn Parameters")]
        public int zombitToSpawn = 4;
        public Vector3 SpawnOffset = new Vector3(0, 0.5f, 0);
        public float RandomRange = 1;
        [Header(("Sounds"))] public AudioSource AudioSource;
        public AudioClip[] HitSounds;

        [ExecuteInEditMode]
        public void Awake()
        {
            GridManager = GridManager.Instance;
            GridManager.OnClearPathFindingData += ClearCellCoordinateData;
            if(!GridManager) Debug.LogWarning(" GridManager non Assigner sur Maison "+name);
            Debug.Log("Added");
        }
        
        private void Start()
        {
            
        }

        public void ClearCellCoordinateData() => CellsCoordinates.Clear();
        
        public void TakeDamage(int damage)
        {
            if (HP <= 0) return;
            HP -= damage;
            if (HP <= 0)
            {
                Destroy();
            }
        }

        public void Destroy()
        {
            if (prefabsDebrie) Instantiate(prefabsDebrie, transform.position, transform.rotation);
            if( PrefabsDestruciotnParticules)Instantiate(PrefabsDestruciotnParticules, transform.position, transform.rotation);
            ManageZombiSpawn();
            Destroy(gameObject);
            
        }
        [ExecuteInEditMode]
        private void OnDestroy()
        {
            GridManager.OnClearPathFindingData -= ClearCellCoordinateData;
            Debug.Log("remove");
        }

        public bool IsAlive()
        {
            return (HP > 0);
        }

        private void ManageZombiSpawn()
        {
            for (int i = 0; i < zombitToSpawn; i++) {
                Vector3 pos = transform.position + SpawnOffset + new Vector3(Random.Range(-RandomRange, RandomRange), 0,
                    Random.Range(-RandomRange, RandomRange));
                ZombieAgent  zombie =Instantiate(prefabZombit, pos, Quaternion.identity);
                zombie.Generate(GridManager);
            }
        }
    }
}