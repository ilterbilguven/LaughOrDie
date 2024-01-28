using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CivilianState
{
    Idle,
    Moving,
    Dead
}

public class CivilianBehaviour : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    private Coroutine _movementCoroutine;
    private CivilianState _state;

    public void StartMovement()
    {
        animator.SetTrigger("Run");
        _movementCoroutine = StartCoroutine(MovementCoroutine());
    }
    
    public void StopMovement()
    {
        animator.SetTrigger("Idle");
        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;
        }
    }

    private IEnumerator MovementCoroutine()
    {
        while (true)
        {
            if (_state == CivilianState.Idle)
            {
                _state = CivilianState.Moving;
                agent.SetDestination(RandomNavmeshLocation(transform.position, 100f));
            }
            else if (_state == CivilianState.Moving)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    _state = CivilianState.Idle;
                }
            }
            else if (_state == CivilianState.Dead)
            {
                break;
            }

            yield return null;
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
