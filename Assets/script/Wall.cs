using System.Security.Cryptography;
using UnityEngine;

namespace script
{
    public class Wall : MonoBehaviour, IDestructible
    {
        public GameObject prefabsDebrie;
        public GameObject prefabsFX;
        public int HP;

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
            if (prefabsFX) Instantiate(prefabsFX, transform.position, transform.rotation);
            if (prefabsDebrie) Instantiate(prefabsDebrie, transform.position, transform.rotation);
            Destroy(gameObject);
        }

        public bool IsAlive()
        {
            
                return (HP > 0);
            

        }

        public Vector3 GetWorldPos() {
            return transform.position;
        }
    }
}