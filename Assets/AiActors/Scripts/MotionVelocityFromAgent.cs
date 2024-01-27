using System;
using UnityEngine;
using UnityEngine.AI;

namespace AiActors.Scripts
{
    public class MotionVelocityFromAgent : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private MotionAnimationHandler motionAnimationHandler;

        private void Update()
        {
            motionAnimationHandler.Velocity = navMeshAgent.velocity;
        }
    }
}