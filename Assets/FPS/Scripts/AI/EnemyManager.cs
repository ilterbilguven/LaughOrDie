using System;
using System.Collections.Generic;
using Game.Scripts.Behaviours;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.AI
{
    public class EnemyManager : MonoBehaviour
    {
        public static event Action<EnemyController> EnemyKilled;
        public List<EnemyController> Enemies { get; private set; }
        public int NumberOfEnemiesTotal { get; private set; }
        public int NumberOfEnemiesRemaining => Enemies.Count;

        void Awake()
        {
            Enemies = new List<EnemyController>();
            EnemySpawnerBehaviour.EnemySpawned += RegisterEnemy;
        }

        public void RegisterEnemy(EnemyController enemy)
        {
            Enemies.Add(enemy);

            NumberOfEnemiesTotal++;
        }

        private void RegisterEnemy(EnemyPolice enemy)
        {
            RegisterEnemy(enemy.GetComponent<EnemyController>());
        }

        public void UnregisterEnemy(EnemyController enemyKilled)
        {
            int enemiesRemainingNotification = NumberOfEnemiesRemaining - 1;

            EnemyKillEvent evt = Events.EnemyKillEvent;
            evt.Enemy = enemyKilled.gameObject;
            evt.RemainingEnemyCount = enemiesRemainingNotification;
            EventManager.Broadcast(evt);

            // removes the enemy from the list, so that we can keep track of how many are left on the map
            Enemies.Remove(enemyKilled);
            
            EnemyKilled?.Invoke(enemyKilled);
        }
    }
}