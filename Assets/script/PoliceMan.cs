using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace script
{
    public class PoliceMan:MonoBehaviour
    {
        public TriggerZoneDetector TriggerZoneDetector;
        public Animator Animator;
        [Header("Attacks")] 
        public float AttackDelay;
        public int AttackDamage;
        public float AttackRange;
        public ParticleSystem PSAttack;
        public ZombieAgent Target;
        [Header("Death")]
        public ZombieAgent PrefabsZombieAgent;
        public GridManager GridManager;
        public GameObject PrefabsDeathPS;
        [Header("Sound")]
        public AudioSource AudioSource;
        public AudioClip[] ShotingSound;
        public AudioClip[] GettingTarget;
        public AudioClip[] GetKill;
        
        
        private float _timer;

        public void Start()
        {
            GridManager = GridManager.Instance;
            if( !GridManager) Debug.LogWarning(" GridManager non Assigner sur Maison "+name);
            TriggerZoneDetector.MaxDistance = AttackRange;
            TriggerZoneDetector.transform.GetComponent<SphereCollider>().radius = AttackRange;
        }

        public void Update()
        {
            if (Target == null) {
                TriggerZoneDetector.CheckOfNull();
                if (TriggerZoneDetector.Zombis.Count > 0)
                {
                    Target =GetTheClosest(TriggerZoneDetector.Zombis);
                    AudioSource.clip = GettingTarget[Random.Range(0, GettingTarget.Length)];
                    AudioSource.Play();
                }
            }
            else ManagerFire();
            Animator.SetBool("HaveTarget", Target!= null);
        }

        public void ManagerFire() {
            
            if (AttackRange < Vector3.Distance(transform.position, Target.transform.position)) {
                Target = null;
                return;
            }

            transform.forward = Target.transform.position - transform.position;
            _timer += Time.deltaTime;
            if (_timer >= AttackDelay) {
                Target.TakeDamage(AttackDamage);
                PSAttack.Play();
                AudioSource.clip = ShotingSound[Random.Range(0, ShotingSound.Length)];
                AudioSource.Play();
                _timer = 0;
            }
        }

        public ZombieAgent GetTheClosest(List<ZombieAgent> zombies) {
            ZombieAgent z = null;
            float distance = Mathf.Infinity;
            foreach (var zombie in zombies) {
                if (distance > Vector3.Distance(zombie.transform.position, transform.position)) {
                    z = zombie;
                    distance = Vector3.Distance(zombie.transform.position, transform.position);
                }
            }
            return z;
        }

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.CompareTag("Zombi")) {
                ZombieAgent z =Instantiate(PrefabsZombieAgent, transform.position, quaternion.identity);
                z.Generate(GridManager);
                Instantiate(PrefabsDeathPS, transform.position, Quaternion.identity);
                AudioSource.PlayClipAtPoint(GetKill[Random.Range(0,GetKill.Length)], transform.position);
                Destroy(gameObject);
            }
        }

        private void OnDrawGizmos() {
            if (EditorControlStatics.DisplayEnnemisDangerZone) {
                Gizmos.color = Color.red * new Color(1, 1, 1, 0.4f);
                Gizmos.DrawSphere(transform.position,  AttackRange);
            }
        }
    }
}