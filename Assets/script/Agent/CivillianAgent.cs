using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace script
{
    [SelectionBase]
    public class CivillianAgent : MonoBehaviour
    {
        [SerializeField] private Rigidbody Rigidbody;
        [SerializeField] private float MaxMoveSpeed;
        [SerializeField] private float Drag;
        [SerializeField] private GridManager GridManager;
        
        [Header("Agent Parameters")] 
        [SerializeField] private int _hp = 3;
        [SerializeField]private GameObject _prefabDeathPS;
        [SerializeField] private Animator _animator;
        [Space(5)] 
        [SerializeField] private float WonderringDelayMin=1;
        [SerializeField] private float WonderringDelayMax=10;
        [SerializeField] private int Wonderringdistance=3;
        [Space(5)] 
        [SerializeField] public float DetectionDistance =10;
        [SerializeField] private TriggerZoneDetector TriggerZoneDetector;
        [SerializeField] private int RunAwayDistance = 10;
        [Space(5)] 
        [SerializeField] private float MaxStamina = 10;
        [SerializeField] private float MoveSpeed=15;
        [SerializeField] private float RunAwayMoveSpeed =30;
        [SerializeField] private float SlowMoveSpeed = 5;
        [SerializeField] private float StaminaRegenRate = 0.5f;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _attackSound;
        [SerializeField] private AudioClip[] _spawnSound;
        [SerializeField] private AudioClip[] _dieSound;

        private float _wonderingTimer;
        private float _wonderingDelay;
        private float _currentMoveSpeed;
        private float _currentStamina;
        private Cell _wonderingTarget;
        private Subgrid _subgrid;

        public bool IsWondering;
        public bool IsRunAway;

        // Start is called before the first frame update
        void Start()
        {
            _currentMoveSpeed = MoveSpeed;
            TriggerZoneDetector.MaxDistance = DetectionDistance;
            _currentStamina = MaxStamina;
            _wonderingDelay = Random.Range(WonderringDelayMin, WonderringDelayMax);
            if (GridManager == null) GridManager = GridManager.Instance;
            if (GridManager == null)
            {
                Debug.LogWarning("GridManagerNot Found ", this);
            }
        }

        // Update is called once per frame
        void Update() {
            ManageWondering();
            ManageRunAway();
            ManageMoveSpeed();
            if (Rigidbody.velocity.magnitude > 0.5f) transform.forward = Rigidbody.velocity;
            _animator.SetFloat("Velocity", Rigidbody.velocity.magnitude/MaxMoveSpeed);
        }
        private void FixedUpdate() {
            ManageMovement();   
        }

        private void ManageWondering() {
            if (IsWondering || IsRunAway) return;

            _wonderingTimer += Time.deltaTime;
            if (_wonderingTimer >= _wonderingDelay) {
                SetNewWonderingTarget();
                _wonderingTimer = 0;
                IsWondering = true;
            }
        }

        private void SetNewWonderingTarget() {
            Cell currentcell = GridManager.GetCellFromWorldPos(transform.position);
            
            Cell[] possibleTargets =GridManager.GetBreathFirstCells(currentcell, Wonderringdistance);
            if (possibleTargets == null || possibleTargets.Length == 0) {
                Debug.LogWarning("Agents Don't find wondering target", this);
                return;
            }
            _wonderingTarget = possibleTargets[Random.Range(0, possibleTargets.Length)];
            SetNewMoveDestination(_wonderingTarget);
        }

        private void ManageMovement() {
            if (_subgrid!=null) {
                Cell cell =_subgrid.GetCellFromWorldPos(transform.position);
                if (cell == null) {
                    Cell currentPos = GridManager.GetCellFromWorldPos(transform.position);
                    if (currentPos == null) {
                        Debug.LogWarning("Agent out of the Game Zone", this);
                        return;
                    }
                    SetNewWonderingTarget();
                    return;
                }

                if (cell.Pos == _wonderingTarget.Pos) {
                    _wonderingDelay = Random.Range(WonderringDelayMin, WonderringDelayMax);
                    IsWondering = false;
                    IsRunAway = false;
                    Rigidbody.velocity =Vector3.zero;
                    return;
                }
                Rigidbody.AddForce(new Vector3(cell.DirectionTarget.x, 0, cell.DirectionTarget.y) * MoveSpeed);
                Rigidbody.velocity -= Rigidbody.velocity * Drag;
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

        private void ManageMoveSpeed()
        {
            if (IsRunAway&& _currentStamina>0) {
                Debug.Log("IsRunnig");
                _currentStamina -= Time.deltaTime;
                if (_currentStamina <= 0) {
                    _currentStamina = 0;
                    _currentMoveSpeed = SlowMoveSpeed;
                }
            }
            else if (!IsRunAway && _currentStamina < MaxStamina) {
                _currentStamina += Time.deltaTime * StaminaRegenRate;
                if (_currentStamina >= MaxStamina) {
                    _currentStamina = MaxStamina;
                    _currentMoveSpeed = MoveSpeed;
                }
            }

            if (_currentStamina == 0) _currentMoveSpeed = SlowMoveSpeed;
        }

        private void SetNewMoveDestination(Cell targetCell) {
            Cell currentcell = GridManager.GetCellFromWorldPos(transform.position);
            _wonderingTarget = targetCell;
            List<Chunk> totalChunks = GridManager.GetAStartPath(currentcell.Chunk , _wonderingTarget.Chunk);
            _subgrid = new Subgrid();
            _subgrid.GenerateSubGrid(totalChunks.ToArray(), GridManager.Size, GridManager.Offset);
            _subgrid.StartCalcFlowfield(_wonderingTarget);
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
