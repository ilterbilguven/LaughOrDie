using System;
using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game.Scripts.Behaviours
{
    public class CivilianSpawnerBehaviour : MonoBehaviour
    {
        [SerializeField] private List<CivilianBehaviour> civilianPrefabs;
        [SerializeField] private int initialCivilianCount = 20;
        [SerializeField] private int maxCivilianCount = 50;
        [SerializeField] private int civilianSpawnInterval = 10;

        private List<CivilianBehaviour> _spawnedCivilians;
        private PlayerCharacterController _player;
        private Coroutine _civilianSpawnerCoroutine;

        private PlayerCharacterController Player
        {
            get
            {
                if(_player == null)
                    _player = FindObjectOfType<PlayerCharacterController>();
                return _player;
            }
            
        }

        private void Start()
        {
            _spawnedCivilians = new List<CivilianBehaviour>();
            for (int i = 0; i < initialCivilianCount; i++)
            {
                CivilianBehaviour civil = SpawnRandomCivilian();
                civil.StartMovement();
                
                _spawnedCivilians.Add(civil);
            }
            _civilianSpawnerCoroutine = StartCoroutine(CivilianSpawnerCoroutine());
        }

        private void OnDestroy()
        {
            if (_civilianSpawnerCoroutine != null)
            {
                StopCoroutine(_civilianSpawnerCoroutine);
                _civilianSpawnerCoroutine = null;
            }
        }

        private IEnumerator CivilianSpawnerCoroutine()
        {
            while (true)
            {
                if (_spawnedCivilians.Count < maxCivilianCount)
                {
                    CivilianBehaviour civil = SpawnRandomCivilian();
                    civil.StartMovement();
                    
                    _spawnedCivilians.Add(civil);
                }
                
                yield return new WaitForSeconds(civilianSpawnInterval);
            }
        }

        private CivilianBehaviour SpawnRandomCivilian()
        { 
            var civilianPrefab = civilianPrefabs[Random.Range(0, civilianPrefabs.Count)];
            var civilian = Instantiate(civilianPrefab, GetRandomPosAroundPlayer(), Quaternion.identity);
            civilian.SetPlayer(Player);
            return civilian;
        }

        private Vector3 GetRandomPosAroundPlayer()
        {
            float minRadius = 20;
            float maxRadius = 30;
            return RandomNavmeshLocation(Player.transform.position, minRadius, maxRadius);
        }
        
        public Vector3 RandomNavmeshLocation(Vector3 point, float minRadius, float maxRadius)
        {
            Vector3 randomDirection = RandomPointInAnnulus(point, minRadius, maxRadius);
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out hit, maxRadius, 1)) {
                finalPosition = hit.position;            
            }
            return finalPosition;
        }
        
        private Vector2 RandomPointInAnnulus(Vector2 origin, float minRadius, float maxRadius){
 
            var randomDirection = (Random.insideUnitCircle * origin).normalized;
            var randomDistance = Random.Range(minRadius, maxRadius);
            var point = origin + randomDirection * randomDistance;
            return point;
        }
    }
}
