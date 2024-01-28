using System;
using System.Collections;
using System.Collections.Generic;
using Unity.FPS.AI;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum CivilianState
{
    Idle,
    Moving,
    Escaping,
    Dead
}

[RequireComponent(typeof(Health))]
public class CivilianBehaviour : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    [Tooltip("The gradient representing the color of the flash on hit")] [GradientUsageAttribute(true)]
    public Gradient OnHitBodyGradient;

    [Tooltip("The duration of the flash on hit")]
    public float FlashOnHitDuration = 0.5f;

    [Header("Sounds")] [Tooltip("Sound played when recieving damages")]
    public AudioClip DamageTick;

    [Header("VFX")] [Tooltip("The VFX prefab spawned when the enemy dies")]
    public GameObject DeathVfx;

    [Tooltip("The point at which the death VFX is spawned")]
    public Transform DeathVfxSpawnPoint;

    [Tooltip("Delay after death where the GameObject is destroyed (to allow for animation)")]
    public float DeathDuration = 0f;
    
    private Coroutine _movementCoroutine;
    private CivilianState _state;
    private PlayerCharacterController _player;
    
    List<EnemyController.RendererIndexData> _bodyRenderers = new List<EnemyController.RendererIndexData>();

    private bool _wasDamagedThisFrame;
    private float _lastTimeDamaged = float.NegativeInfinity;

    private Health _health;
    private MaterialPropertyBlock _bodyFlashMaterialPropertyBlock;

    private void Start()
    {
        TryGetComponent(out _health);
        
        _bodyFlashMaterialPropertyBlock = new MaterialPropertyBlock();
        
        DebugUtility.HandleErrorIfNullGetComponent<Health, CivilianBehaviour>(_health, this, gameObject);
        
        _health.OnDamaged += OnDamaged;
        _health.OnDie += OnDie;
    }

    private void Update()
    {
        Color currentColor = OnHitBodyGradient.Evaluate((Time.time - _lastTimeDamaged) / FlashOnHitDuration);
        _bodyFlashMaterialPropertyBlock.SetColor("_EmissionColor", currentColor);
        foreach (var data in _bodyRenderers)
        {
            data.Renderer.SetPropertyBlock(_bodyFlashMaterialPropertyBlock, data.MaterialIndex);
        }
    }

    private void OnDestroy()
    {
        _health.OnDamaged -= OnDamaged;
        _health.OnDie -= OnDie;
    }

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

    private void OnDamaged(float arg0, GameObject damageSource)
    {
        // test if the damage source is the player
        if (damageSource && !damageSource.GetComponent<EnemyController>())
        {
            _lastTimeDamaged = Time.time;
            
            // play the damage tick sound
            if (DamageTick && !_wasDamagedThisFrame)
                AudioUtility.CreateSFX(DamageTick, transform.position, AudioUtility.AudioGroups.DamageTick, 0f);
            
            _wasDamagedThisFrame = true;
        }
    }

    private void OnDie()
    {
        // spawn a particle system when dying
        var vfx = Instantiate(DeathVfx, DeathVfxSpawnPoint.position, Quaternion.identity);
        Destroy(vfx, 5f);

        // this will call the OnDestroy function
        Destroy(gameObject, DeathDuration);
    }
}