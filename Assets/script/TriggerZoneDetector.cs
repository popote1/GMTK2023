using System;
using System.Collections.Generic;
using UnityEngine;

namespace script
{
    public class TriggerZoneDetector: MonoBehaviour
    {
        public List<ZombieAgent> Zombis;
        public float MaxDistance;
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
                if (z !=null && Zombis.Contains(z)) {
                    Zombis.Remove(z); 
                }
            }
        }

        public void CheckOfNull() {
            foreach (var zombi in Zombis.ToArray()) {
                if (zombi == null)
                {
                    Zombis.Remove(zombi);
                    continue;
                }
                //if( Vector3.Distance(transform.position, zombi.transform.position)>MaxDistance) Zombis.Remove(zombi);
            }
        }
        public ZombieAgent GetTheClosest() {
            ZombieAgent z = null;
            float distance = Mathf.Infinity;
            foreach (var zombie in Zombis) {
                if (distance > Vector3.Distance(zombie.transform.position, transform.position)) {
                    z = zombie;
                    distance = Vector3.Distance(zombie.transform.position, transform.position);
                }
            }
            return z;
        }
    }
}