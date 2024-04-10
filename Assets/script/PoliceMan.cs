using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace script
{
    public class PoliceMan:GridAgent
    {
        public TriggerZoneDetector TriggerZoneDetector;
        [Header("Attacks")] 
        public float AttackDelay;
        public int AttackDamage;
        public float AttackRange;
        public ParticleSystem PSAttack;
        public ZombieAgent Target;
        [Header("Death")]
        public ZombieAgent PrefabsZombieAgent;
        
        public GameObject PrefabsDeathPS;
        [Header("Sound")]
        public AudioSource AudioSource;
        public AudioClip[] ShotingSound;
        public AudioClip[] GettingTarget;
        public AudioClip[] GetKill;
        
        
        private float _timer;

        protected override void Start()
        {
            TriggerZoneDetector.MaxDistance = AttackRange;
            TriggerZoneDetector.transform.GetComponent<SphereCollider>().radius = AttackRange;
            base.Start();
        }

        protected override void Update() {
            
            _animator.SetBool("HaveTarget", Target!= null);
            base.Update();
        }

        protected override void ManageStat() {
            base.ManageStat();
            if (Stat == GridActorStat.Idle || Stat == GridActorStat.Move) { 
                ManageLookingForTarget();
            }
        }

        private void ManageLookingForTarget() {
            TriggerZoneDetector.CheckOfNull();
            
            if (TriggerZoneDetector.Zombis.Count <= 0) return;
            Target =GetTheClosest(TriggerZoneDetector.Zombis);
            AudioSource.clip = GettingTarget[Random.Range(0, GettingTarget.Length)];
            AudioSource.Play();
            ChangeStat(GridActorStat.Attack);

        }

        protected override void ManageAttack()
        {
            if (Target == null) ManageLookingForTarget();
            if (Target == null) {
                ChangeStat(GridActorStat.Idle);
                return;
            }
            ManagerFire();
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

        //private void OnCollisionEnter(Collision other) {
        //    if (other.gameObject.CompareTag("Zombi")) {
        //        ZombieAgent z =Instantiate(PrefabsZombieAgent, transform.position, quaternion.identity);
        //        z.Generate(GridManager);
        //        Instantiate(PrefabsDeathPS, transform.position, Quaternion.identity);
        //        AudioSource.PlayClipAtPoint(GetKill[Random.Range(0,GetKill.Length)], transform.position);
        //        Destroy(gameObject);
        //    }
        //}

        private void OnDrawGizmos() {
            if (EditorControlStatics.DisplayEnnemisDangerZone) {
                Gizmos.color = Color.red * new Color(1, 1, 1, 0.4f);
                Gizmos.DrawSphere(transform.position,  AttackRange);
            }
        }
    }
}