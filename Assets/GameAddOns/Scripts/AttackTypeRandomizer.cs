using System;
using Unity.FPS.Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameAddOns.Scripts
{
    public class AttackTypeRandomizer : MonoBehaviour
    {
        [SerializeField] private int attackTypes;
        
        private Animator _animator;
        private WeaponController _controller;

        private void Awake()
        {
            TryGetComponent(out _animator);
            TryGetComponent(out _controller);
        }

        private void OnEnable()
        {
            _controller.OnShootProcessed += OnShootProcessed;
        }

        private void OnDisable()
        {
            _controller.OnShootProcessed -= OnShootProcessed;
        }

        private void OnShootProcessed()
        {
            _animator.SetInteger("AttackType", Random.Range(0, attackTypes));
        }
    }
}