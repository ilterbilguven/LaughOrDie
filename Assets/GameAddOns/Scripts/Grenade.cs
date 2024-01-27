using System;
using System.Collections;
using Unity.FPS.Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameAddOns.Scripts
{
    public class Grenade : MonoBehaviour
    {
        [SerializeField] private float damage;
        [SerializeField] private float radius;
        [SerializeField] private LayerMask explosionMask;
        [SerializeField] private float timeToExplosion;
        [SerializeField] private ParticleSystem explosionFx;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(timeToExplosion);
            Explode();
        }

        private void Explode()
        {
            Instantiate(explosionFx, transform.position, transform.rotation * Quaternion.Euler(0, Random.Range(0, 360), 0));
            var results = Physics.OverlapSphere(transform.position, radius, explosionMask);

            foreach (var result in results)
            {
                if (result.TryGetComponent(out Health health))
                {
                    health.TakeDamage(damage, gameObject);
                }
            }
        }
    }
}