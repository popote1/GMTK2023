using UnityEngine;

namespace script
{
    public class House: MonoBehaviour, IDestructible
    {
        public GameObject prefabsDebrie;
        public ZombieAgent prefabZombit;
        public GridManager GridManager;
        public int HP;
        public int zombitToSpawn = 4;
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
            for (int i = 0; i < zombitToSpawn; i++)
            {
                ZombieAgent  zombie =Instantiate(prefabZombit, transform.position, Quaternion.identity);
                zombie.Generate(GridManager);
            }
            Destroy(gameObject);
        }

        public bool IsAlive()
        {
            return (HP > 0);
        }
    }
}