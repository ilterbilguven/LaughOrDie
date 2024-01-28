using System;
using System.Collections.Generic;
using System.Linq;
using Unity.FPS.AI;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WeaponDrops.Scripts
{
    public class WeaponDropManager : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<WeaponController, WeaponController> weapons;
        [SerializeField] private SerializableDictionary<WeaponController, WeaponPickup> weaponPickups;
        [SerializeField] private SerializableDictionary<WeaponController, AmmoPickup> ammoPickups;

        private PlayerCharacterController _player;
        private List<WeaponPickup> _dropped;

        private void Awake()
        {//
            _dropped = new List<WeaponPickup>();
            _player = FindFirstObjectByType<PlayerCharacterController>();
            
            EnemyManager.EnemyKilled += OnEnemyKilled;
        }

        private void OnDestroy()
        {
            EnemyManager.EnemyKilled -= OnEnemyKilled;
        }

        public void DropWeapon(Vector3 position)
        {
            var viableWeapons = weapons.Select(e => e.Value)
                .Where(w => !_player.GetComponent<PlayerWeaponsManager>().HasWeapon(w) && !_dropped.Contains(weaponPickups[w]))
                .Select(w =>
                {
                    if (weaponPickups.TryGetValue(w, out var weaponPickup))
                        return weaponPickup;
                    return null;
                })
                .Where(w => w)
                .ToList();
            
            if (viableWeapons.Count > 0)
            {
                var dropped = viableWeapons[Random.Range(0, viableWeapons.Count)];
                _dropped.Add(dropped);
                
                Instantiate(dropped, GetRandomPosition(position, new Vector2(1, 2.5f)), Quaternion.identity);
            }
        }

        public void DropAmmo(Vector3 position)
        {
            var viableAmmoPickups = weapons.Select(e => e.Value)
                .Where(w => _player.GetComponent<PlayerWeaponsManager>().HasWeapon(w))
                .Select(w =>
                {
                    if (ammoPickups.TryGetValue(w, out var ammo))
                        return ammo;
                    return null;
                })
                .Where(a => a)
                .ToList();
            
            if (viableAmmoPickups.Count > 0)
            {
                var ammo = viableAmmoPickups[Random.Range(0, viableAmmoPickups.Count)];
                Instantiate(ammo, GetRandomPosition(position, new Vector2(1, 2.5f)), Quaternion.identity);
            }
        }

        private void OnEnemyKilled(EnemyController enemyController)
        {
            if (Random.value > 0.7f)
            {
                DropWeapon(enemyController.transform.position);
            }
            else if (Random.value > 0.5f)
            {
                DropAmmo(enemyController.transform.position);
            }
        }

        private Vector3 GetRandomPosition(Vector3 position, Vector2 range)
        {
            var randomRange = Random.Range(range.x, range.y);
            var randomDir = Random.insideUnitCircle;
            var vector = position + randomRange * new Vector3(randomDir.x, 0, randomDir.y);

            return vector;
        }
    }
}