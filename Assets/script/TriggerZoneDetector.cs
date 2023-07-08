using System;
using System.Collections.Generic;
using UnityEngine;

namespace script
{
    public class TriggerZoneDetector: MonoBehaviour
    {
        public List<ZombieAgent> Zombis;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Zombi"))
            {
                ZombieAgent z = other.GetComponent<ZombieAgent>(); 
                if (z !=null && !Zombis.Contains(z)) {
                   Zombis.Add(z); 
                }
            }
        }
        private void OnTriggerExit(Collider other) {
            CheckOfNull();
            if (other.gameObject.CompareTag("Zombi")) {
                ZombieAgent z = other.GetComponent<ZombieAgent>(); 
                if (z !=null && !Zombis.Contains(z)) {
                    Zombis.Add(z); 
                }
            }
        }

        public void CheckOfNull() {
            foreach (var zombi in Zombis.ToArray()) {
                if (zombi == null) Zombis.Remove(zombi);
            }
        }
    }
}