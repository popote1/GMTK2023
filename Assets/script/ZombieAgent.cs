﻿using System;
using Unity.Mathematics;
using UnityEngine;

namespace script
{
    public class ZombieAgent : MonoBehaviour
    {
        [SerializeField] private Rigidbody Rigidbody;
        [SerializeField] private float MaxMoveSpeed;
        [SerializeField] private float MoveSpeed;
        [SerializeField] private float Drag;
        [SerializeField] private GridManager GridManager;
        [Header("Zombie Parameters")] 
        [SerializeField] private int _hp = 3;
        [SerializeField]private GameObject _prefabDeathPS;
        [SerializeField] private Animator _animator;
        [Header("Attack Parameters")]
        [SerializeField] private float _attackDelay =2;
        [SerializeField] private int _attackDamage =1;
        [SerializeField] private GameObject _prefabsAttackEffect;

        private IDestructible _target;
        private bool _isAttcking;
        private float _attacktimer;

        public void Start() {
            StaticData.OnZombieGain?.Invoke();
            StaticData.ZombieCount++;
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
            if (GridManager&&!_isAttcking) {
                Cell cell =GridManager.GetCellFromWorldPos(transform.position);
                if (cell == null) return;
                Rigidbody.AddForce(new Vector3(cell.DirectionTarget.x, 0, cell.DirectionTarget.y) * MoveSpeed);
                Rigidbody.velocity -= Rigidbody.velocity * Drag;
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
                Instantiate(_prefabsAttackEffect, transform.position, quaternion.identity);
            }
        }

        private void Update() {
            if (_isAttcking) {
                ManageAttack();
            }
            else {
                transform.forward = Rigidbody.velocity;
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