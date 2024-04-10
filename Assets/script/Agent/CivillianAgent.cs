using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace script
{
    [SelectionBase]
    public class CivillianAgent : GridAgent
    {
        //[SerializeField] private Rigidbody Rigidbody;
        //[SerializeField] private float _maxMoveSpeed;
        //[SerializeField] private float _speedModulator;
        //[SerializeField] private GridManager GridManager;
        
        [Header("Agent Parameters")] 
        [SerializeField] private int _hp = 3;
        [SerializeField]private GameObject _prefabDeathPS;
        //[SerializeField] private Animator _animator;
        [Space(5)] 
        //[SerializeField] private float WonderringDelayMin=1;
        //[SerializeField] private float WonderringDelayMax=10;
        //[SerializeField] private int Wonderringdistance=3;
        [Space(5)] 
        [SerializeField] public float DetectionDistance =10;
        [SerializeField] private TriggerZoneDetector TriggerZoneDetector;
        [SerializeField] private int RunAwayDistance = 10;
        [Space(5)] 
        [SerializeField] private float MaxStamina = 10;
        //[SerializeField] private float _moveSpeed=15;
        [SerializeField] private float RunAwayMoveSpeed =30;
        [SerializeField] private float StaminaRegenRate = 0.5f;

        public ZombieAgent PrefabsZombieAgent;
        public GameObject PrefabsDeathPS;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _attackSound;
        [SerializeField] private AudioClip[] _spawnSound;
        [SerializeField] private AudioClip[] _dieSound;

        //private float _wonderingTimer;
        //private float _wonderingDelay;
        private float _currentMoveSpeed;
        private float _currentStamina;
        //private Cell _wonderingTarget;
        //private Subgrid _subgrid;

        //public bool IsWondering;
        public bool IsRunAway;

        // Start is called before the first frame update
        protected override void Start() {
            _currentMoveSpeed = _moveSpeed;
            TriggerZoneDetector.MaxDistance = DetectionDistance;
            _currentStamina = MaxStamina;
            base.Start();
        }

        public float GetCurrentStaminaStat() => _currentStamina / MaxStamina;
        public float GetDebugMoveSpeed() => GetMoveSpeed();
        
        protected override void Update() {
            //ManageWondering();
            ManageRunAway();
            ManageStamina();
            base.Update();
        }
      

        protected override void GetToMoveTarget()
        {
            if (Stat == GridActorStat.Grabed) return;
            IsRunAway = false;
            ManageRunAway();
            if(!IsRunAway){
                base.GetToMoveTarget();
            }
        }

        private void ManageRunAway()
        {
            if (!IsRunAway) {
                TriggerZoneDetector.CheckOfNull();
                if (TriggerZoneDetector.Zombis.Count > 0) {
                    ZombieAgent zombi = TriggerZoneDetector.GetTheClosest();
                    Cell[] cells = GridManager.GetBreathFirstCells(GridManager.GetCellFromWorldPos(transform.position),
                        RunAwayDistance);
                    _currentMoveSpeed = RunAwayMoveSpeed;
                    IsRunAway = true;
                    SetNewMoveDestination(GetFarCellFrom(cells, zombi.transform));
                }
            }
        }

        private Cell GetFarCellFrom(Cell[] cells, Transform target){
            Vector2Int targetPos = GridManager.GetCellFromWorldPos(target.position).Pos; 
            float bestDist = float.MinValue;
            Cell bestcell = null;
            foreach (var cell in cells) {
                if (Vector2.Distance(targetPos, cell.Pos) > bestDist) {
                    bestcell = cell;
                    bestDist = Vector2.Distance(targetPos, cell.Pos);
                }
            }

            return bestcell;
        }
        //private void OnCollisionEnter(Collision other) {
        //    if (other.gameObject.CompareTag("Zombi")) {
        //        ZombieAgent z =Instantiate(PrefabsZombieAgent, transform.position, quaternion.identity);
        //        z.Generate(GridManager);
        //        Instantiate(PrefabsDeathPS, transform.position, Quaternion.identity);
        //        Destroy(gameObject);
        //    }
        //}
        private void ManageStamina() {
            if (IsRunAway && _currentStamina>0) {
                _currentStamina = Mathf.Clamp(_currentStamina -Time.deltaTime,0,MaxStamina);
            }
            if (!IsRunAway && _currentStamina < MaxStamina) {
                _currentStamina = Mathf.Clamp(_currentStamina + Time.deltaTime * StaminaRegenRate,0,MaxStamina);
            }
        }

        protected override float GetMoveSpeed() {
            if (IsRunAway && _currentStamina > 0) return RunAwayMoveSpeed;
            return _moveSpeed;
        }

        private void OnDrawGizmos()
        {
            if (EditorControlStatics.DisplayDetectionZone) {
                Gizmos.color = new Color(0, 0.5f, 1, 0.25f);
                Gizmos.DrawSphere(transform.position , DetectionDistance);
            }
        }
    }
}
