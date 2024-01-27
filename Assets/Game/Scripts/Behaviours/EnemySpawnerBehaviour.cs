using System;
using System.Collections;
using System.Collections.Generic;
using Unity.FPS.AI;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game.Scripts.Behaviours
{
    public class EnemySpawnerBehaviour : MonoBehaviour
    {
        public delegate void EnemySpawnedHandler(EnemyPolice enemy);
        public static event EnemySpawnedHandler EnemySpawned;

        [SerializeField] private int startingEnemy;
        [SerializeField] private int maxEnemy;
        [SerializeField] private float spawnInterval;
        
        [SerializeField] private EnemyPolice enemyPrefab;
        [SerializeField] private List<Transform> spawnPoints;
        
        private List<EnemyController> _currentEnemies;
        private Coroutine _enemySpawnerCoroutine;

        private void Start()
        {
            _currentEnemies = new List<EnemyController>();

            for (int i = 0; i < startingEnemy; i++) 
            {
                SpawnEnemy();
            }

            _enemySpawnerCoroutine = StartCoroutine(EnemySpawnerCoroutine());
            EnemyManager.EnemyKilled += OnEnemyKilled;
        }

        private void OnDestroy()
        {
            if (_enemySpawnerCoroutine != null)
            {
                StopCoroutine(_enemySpawnerCoroutine);
                _enemySpawnerCoroutine = null;
            }
        }

        private IEnumerator EnemySpawnerCoroutine()
        {
            // Initial delay for spawned enemies
            yield return new WaitForSeconds(spawnInterval);

            while (true)
            {
                if (_currentEnemies.Count < maxEnemy)
                {
                    SpawnEnemy();
                    yield return new WaitForSeconds(spawnInterval);
                }
                
                yield return null;
            }
        }

        public void SpawnEnemy() {
            var spawnPoint = RandomNavmeshLocation(spawnPoints[Random.Range(0, spawnPoints.Count)].position, 10f);
            var enemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
            enemy.GetComponent<NavMeshAgent>().Warp(spawnPoint);
            
            EnemySpawned?.Invoke(enemy);
            _currentEnemies.Add(enemy.GetComponent<EnemyController>());
        }
        
        private void OnEnemyKilled(EnemyController killedEnemy)
        {
            if (_currentEnemies.Contains(killedEnemy))
            {
                _currentEnemies.Remove(killedEnemy);
            }
            else
            {
                Debug.LogError("Could not find killed enemy in current enemies list!");
            }
        }
        
        public Vector3 RandomNavmeshLocation(Vector3 point, float radius) {
            Vector3 randomDirection = point + Random.insideUnitSphere * radius;
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
                finalPosition = hit.position;            
            }
            return finalPosition;
        }
    }
}
