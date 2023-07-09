using UnityEngine;

namespace script
{
    public class House: MonoBehaviour, IDestructible
    {
        public GameObject prefabsDebrie;
        public ZombieAgent prefabZombit;
        public GridManager GridManager;
        public int HP;
        public GameObject PrefabsDestruciotnParticules;
        [Header("Spawn Parameters")]
        public int zombitToSpawn = 4;
        public Vector3 SpawnOffset = new Vector3(0, 0.5f, 0);
        public float RandomRange = 1;

        private void Start()
        {
            GridManager = GridManager.Instance;
            if(!GridManager) Debug.LogWarning(" GridManager non Assigner sur Maison "+name);
        }
        
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