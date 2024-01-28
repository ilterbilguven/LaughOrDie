using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.AI;

public enum CivilianState
{
    Idle,
    Moving,
    Escaping,
    Dead
}

public class CivilianBehaviour : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    private Coroutine _movementCoroutine;
    private CivilianState _state;
    private PlayerCharacterController _player;

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
            // In every case, we want the civilian to be on move (except if dead)
            if (_state != CivilianState.Dead)
            {
                if (agent.isOnNavMesh == false)
                {
                    Debug.LogError("A civilian is not on the navmesh!");
                    agent.Warp(RandomNavmeshLocation(transform.position, 5));
                }
            }
            
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

                if (Vector3.Distance(transform.position, _player.transform.position) < 5f)
                {
                    _state = CivilianState.Escaping;
                    Vector3 APointAwayFromPlayer = transform.position +
                                                   (transform.position - _player.transform.position).normalized * 5f;
                    agent.SetDestination(RandomNavmeshLocation(APointAwayFromPlayer, 100f));
                }
            }
            else if (_state == CivilianState.Escaping)
            {
                if (Vector3.Distance(transform.position, _player.transform.position) > 5f)
                {
                    _state = CivilianState.Idle;
                }
                else if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    Vector3 APointAwayFromPlayer = transform.position +
                                                   (transform.position - _player.transform.position).normalized * 5f;
                    agent.SetDestination(RandomNavmeshLocation(APointAwayFromPlayer, 100f));
                }
            }
            else if (_state == CivilianState.Dead)
            {
                break;
            }

            yield return null;
        }
    }

    public Vector3 RandomNavmeshLocation(Vector3 point, float radius)
    {
        Vector3 randomDirection = point + Random.insideUnitSphere * radius;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }

    public void SetPlayer(PlayerCharacterController player)
    {
        _player = player;
    }
}