using System;
using Unity.FPS.Game;
using UnityEngine;

namespace Game.Scripts.Behaviours
{
    public class DisableOnDeath : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private Collider target;

        private void OnValidate()
        {
            TryGetComponent(out target);
        }

        private void Awake()
        {
            health.OnDie += OnDie;
        }

        private void OnEnable()
        {
            target.enabled = true;
        }

        private void OnDestroy()
        {
            health.OnDie -= OnDie;
        }

        private void OnDie()
        {
            target.enabled = false;
        }
    }
}