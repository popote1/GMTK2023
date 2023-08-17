using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace script
{
    public class ZombieAgent : MonoBehaviour
    {
        [SerializeField] private Rigidbody Rigidbody;
        [SerializeField] private float MaxMoveSpeed;
        [SerializeField] private float MoveSpeed;
        [SerializeField] private float Drag;
        [SerializeField] private GridManager GridManager;
        [Header("Zombie Reference")]
        public GameObject SelectionSprite;
        public GameObject PSEmoteRedSquare;
        [Header("Zombie Parameters")] 
        [SerializeField] private int _hp = 3;
        [SerializeField]private GameObject _prefabDeathPS;
        [SerializeField] private Animator _animator;
        [Header("Attack Parameters")]
        [SerializeField] private float _attackDelay =2;
        [SerializeField] private int _attackDamage =1;
        [SerializeField] private GameObject _prefabsAttackEffect;
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _attackSound;
        [SerializeField] private AudioClip[] _spawnSound;
        [SerializeField] private AudioClip[] _dieSound;
        [Header("HeightOffSetting")] 
        public float HeightOffSetting;
        public LayerMask GroundLayer;
        
        public Subgrid Subgrid;
        
        private IDestructible _target;
        private bool _isAttcking;
        private float _attacktimer;
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set {
                if (SelectionSprite) SelectionSprite.SetActive(value);
                _isSelected = value;
            }
        }

        public void Start() {
            StaticData.ZombieCount++;
            StaticData.OnZombieGain?.Invoke();
            _audioSource.clip = _spawnSound[Random.Range(0, _spawnSound.Length)];
            _audioSource.Play();
        }

        public void OnDestroy() => StaticData.ZombieLose();

        public void TakeDamage(int damage)
        {
            _hp -= damage;
            if (_hp <= 0) {
                Instantiate(_prefabDeathPS, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
        

        public void Generate(GridManager gridManager) {
            GridManager = gridManager;
        }

        private void FixedUpdate() {
            if (Subgrid!=null&&!_isAttcking) {
                Cell cell =Subgrid.GetCellFromWorldPos(transform.position);
                if (cell == null) {
                    PSEmoteRedSquare.SetActive(true);
                    return;
                }
                PSEmoteRedSquare.SetActive(false);
                Rigidbody.AddForce(new Vector3(cell.DirectionTarget.x, 0, cell.DirectionTarget.y) * MoveSpeed);
                Rigidbody.velocity -= Rigidbody.velocity * Drag;
                //transform.position = new Vector3(transform.position.x, 0.5f,transform.position.z );
            }
        }

        private void ManageAttack() {
            if (_target==null||!_target.IsAlive()) {
                _isAttcking = false;
                Rigidbody.isKinematic = false;
            }
            _attacktimer += Time.deltaTime;
            if (_attacktimer >= _attackDelay) {
                _target.TakeDamage(_attackDamage);
                _attacktimer = 0;
                _animator.SetTrigger("Attack");
                _audioSource.clip = _attackSound[Random.Range(0, _attackSound.Length)];
                _audioSource.Play();
                Instantiate(_prefabsAttackEffect, transform.position, quaternion.identity);
            }
        }

        private void Update() {
            if (_isAttcking) {
                ManageAttack();
            }
            else
            {
                if (Rigidbody.velocity.magnitude > 0.5f) transform.forward = Rigidbody.velocity;
                RaycastHit hit;
                if (Physics.Raycast(new Ray(transform.position, Vector3.down), out hit, 4, GroundLayer)) {
                    transform.position = new Vector3(transform.position.x, hit.point.y + HeightOffSetting, transform.position.z);
                }
            }
            _animator.SetFloat("Velocity", Rigidbody.velocity.magnitude/MaxMoveSpeed);
        }

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.CompareTag("Destructible")) {
                if (other.gameObject.GetComponent<IDestructible>()!=null) {
                    _target = other.gameObject.GetComponent<IDestructible>();
                    _isAttcking = true;
                    Rigidbody.velocity = Vector3.zero;
                    transform.forward = other.transform.position -transform.position;
                    Rigidbody.isKinematic = true;
                }
            }
        }
    }
}