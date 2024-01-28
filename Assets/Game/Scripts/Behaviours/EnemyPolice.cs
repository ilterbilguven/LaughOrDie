using System;
using System.Collections;
using Game.Scripts.Controllers;
using Game.Scripts.Helpers;
using Unity.FPS.AI;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Behaviours
{
    public class EnemyPolice : MonoBehaviour
    {
        public enum AIState
        {
            Follow,
            Attack,
        }

        public Animator Animator;

        [Tooltip("Fraction of the enemy's attack range at which it will stop moving towards target while attacking")]
        [Range(0f, 1f)]
        public float AttackStopDistanceRatio = 0.5f;

        [Tooltip("The random hit damage effects")]
        public ParticleSystem[] RandomHitSparks;

        public ParticleSystem[] OnDetectVfx;
        public AudioClip OnDetectSfx;

        [Header("Sound")] public AudioClip MovementSound;
        public MinMaxFloat PitchDistortionMovementSpeed;

        [SerializeField] private RagdollHelper ragdollHelper;

        public AIState AiState { get; private set; }
        EnemyController m_EnemyController;
        AudioSource m_AudioSource;

        const string k_AnimMoveSpeedParameter = "MoveSpeed";
        const string k_AnimAttackParameter = "Attack";
        const string k_AnimAlertedParameter = "Alerted";
        const string k_AnimOnDamagedParameter = "OnDamaged";

        private PlayerCharacterController _player;

        public PlayerCharacterController Player
        {
            get
            {
                if(_player == null)
                    _player = FindObjectOfType<PlayerCharacterController>();

                return _player;
            }
        }
        
        private Coroutine _enemyLoopCoroutine;

        void Start()
        {
            m_EnemyController = GetComponent<EnemyController>();
            DebugUtility.HandleErrorIfNullGetComponent<EnemyController, EnemyMobile>(m_EnemyController, this,
                gameObject);

            m_EnemyController.onAttack += OnAttack;
            m_EnemyController.onDamaged += OnDamaged;
            m_EnemyController.EnemyDied += OnEnemyDied;
            m_EnemyController.SetPathDestinationToClosestNode();

            AiState = AIState.Follow;

            // adding a audio source to play the movement sound on it
            m_AudioSource = GetComponent<AudioSource>();
            DebugUtility.HandleErrorIfNullGetComponent<AudioSource, EnemyMobile>(m_AudioSource, this, gameObject);
            m_AudioSource.clip = MovementSound;
            m_AudioSource.Play();
            
            ragdollHelper.DisableRagdoll();

            _enemyLoopCoroutine = StartCoroutine(EnemyLoopCoroutine());
            GameController.GameEnded += OnGameEnded;
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void OnGameEnded()
        {
            Dispose();
        }

        private void Dispose()
        {
            GameController.GameEnded -= OnGameEnded;

            if (_enemyLoopCoroutine != null)
            {
                StopCoroutine(_enemyLoopCoroutine);
                _enemyLoopCoroutine = null;
            }
        }

        private IEnumerator EnemyLoopCoroutine()
        {
            yield return null;
            
            while (true)
            {
                UpdateAiStateTransitions();
                UpdateCurrentAiState();

                float moveSpeed = m_EnemyController.NavMeshAgent.velocity.magnitude;

                // Update animator speed parameter
                Animator.SetFloat(k_AnimMoveSpeedParameter, moveSpeed);

                // changing the pitch of the movement sound depending on the movement speed
                m_AudioSource.pitch = Mathf.Lerp(PitchDistortionMovementSpeed.Min, PitchDistortionMovementSpeed.Max,
                    moveSpeed / m_EnemyController.NavMeshAgent.speed);

                yield return null;
            }
        }

        void UpdateAiStateTransitions()
        {
            // Handle transitions 
            switch (AiState)
            {
                case AIState.Follow:
                    // Transition to attack when there is a line of sight to the target
                    if (m_EnemyController.IsSeeingTarget && m_EnemyController.IsTargetInAttackRange)
                    {
                        AiState = AIState.Attack;
                        m_EnemyController.SetNavDestination(transform.position);
                    }
                    break;
                
                case AIState.Attack:
                    // Transition to follow when no longer a target in attack range
                    if (!m_EnemyController.IsTargetInAttackRange)
                    {
                        AiState = AIState.Follow;
                    }
                    break;
            }
        }

        void UpdateCurrentAiState()
        {
            // Handle logic 
            switch (AiState)
            {
                case AIState.Follow:
                    m_EnemyController.SetNavDestination(Player.transform.position);
                    m_EnemyController.OrientTowards(Player.transform.position);
                    m_EnemyController.OrientWeaponsTowards(Player.transform.position);
                    break;
                case AIState.Attack:
                    if (Vector3.Distance(Player.transform.position,
                            m_EnemyController.DetectionModule.DetectionSourcePoint.position)
                        >= (AttackStopDistanceRatio * m_EnemyController.DetectionModule.AttackRange))
                    {
                        m_EnemyController.SetNavDestination(Player.transform.position);
                    }
                    else
                    {
                        m_EnemyController.SetNavDestination(transform.position);
                    }

                    m_EnemyController.OrientTowards(m_EnemyController.KnownDetectedTarget.transform.position);
                    m_EnemyController.TryAtack(m_EnemyController.KnownDetectedTarget.transform.position);
                    break;
            }
        }
        
        void OnAttack()
        {
            Animator.SetTrigger(k_AnimAttackParameter);
        }

        void OnDamaged()
        {
            if (RandomHitSparks.Length > 0)
            {
                int n = Random.Range(0, RandomHitSparks.Length - 1);
                RandomHitSparks[n].Play();
            }

            Animator.SetTrigger(k_AnimOnDamagedParameter);
        }
        
        private void OnEnemyDied()
        {
            ragdollHelper.EnableRagdoll();
            StartCoroutine(Explode());
            Dispose();
        }

        private IEnumerator Explode()
        {
            yield return new WaitForSeconds(5);
            Destroy(gameObject);
        }
    }
}
