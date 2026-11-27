using AI.Enemy.States;
using AI.FSM;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace AI.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        [Header("Configuración")]
        [SerializeField] private EnemyConfig config;

        [Header("Referencias")]
        [SerializeField] private Animator animator;
        [SerializeField] private Transform targetOverride;
        [SerializeField] private Transform[] patrolPoints;

        [Header("Eventos de Cinemática (Reached)")]
        [SerializeField] private UnityEvent onReachedEnter;
        [SerializeField] private UnityEvent onReachedExit;

        [Header("Depuración")]
        [SerializeField] private bool showDebugLogs = true;

        private NavMeshAgent agent;
        private EnemyContext context;
        private StateMachine stateMachine;

        private IdleState idleState;
        private PatrolState patrolState;
        private FollowState followState;
        private AttackState attackState;
        private ReachedState reachedState;

        public string CurrentStateName => stateMachine?.CurrentState?.GetType().Name ?? "None";

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();

            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }

            if (config == null)
            {
                Debug.LogError("EnemyController: Falta asignar un EnemyConfig en el Inspector.", this);
                enabled = false;
                return;
            }

            Transform target = ResolveTarget();

            context = new EnemyContext(transform, agent, animator, config, target, patrolPoints);
            stateMachine = new StateMachine();

            SetupStatesAndTransitions();
        }

        private void Start()
        {
            stateMachine.SetInitialState(idleState);

            if (showDebugLogs)
            {
                Debug.Log($"[{name}] Estado inicial: {CurrentStateName}", this);
            }
        }

        private void Update()
        {
            string previousState = CurrentStateName;
            stateMachine.Tick(Time.deltaTime);

            if (showDebugLogs && previousState != CurrentStateName)
            {
                Debug.Log($"[{name}] Transición: {previousState} -> {CurrentStateName}", this);
            }
        }

        private void FixedUpdate()
        {
            stateMachine.FixedTick(Time.fixedDeltaTime);
        }

        private void OnDestroy()
        {
            if (reachedState != null)
            {
                reachedState.OnEntered -= HandleReachedEntered;
                reachedState.OnExited -= HandleReachedExited;
            }
        }

        public void OnAttackHit()
        {
            if (stateMachine.CurrentState == attackState)
            {
                attackState.NotifyAttackHit();
            }
        }

        public void OnAttackFinished()
        {
            if (stateMachine.CurrentState == attackState)
            {
                attackState.NotifyAttackFinished();
            }
        }

        public void OnReachedFinished()
        {
            if (stateMachine.CurrentState == reachedState)
            {
                reachedState.NotifyRecoveryFinished();
            }
        }

        private void HandleReachedEntered()
        {
            onReachedEnter?.Invoke();
        }

        private void HandleReachedExited()
        {
            onReachedExit?.Invoke();
        }

        private Transform ResolveTarget()
        {
            if (targetOverride != null)
            {
                return targetOverride;
            }

            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                return playerObject.transform;
            }

            Debug.LogWarning("EnemyController: No se encontró ningún objeto con el tag 'Player'.", this);
            return null;
        }

        private void SetupStatesAndTransitions()
        {
            idleState = new IdleState(context);
            patrolState = new PatrolState(context);
            followState = new FollowState(context);
            attackState = new AttackState(context);
            reachedState = new ReachedState(context);

            reachedState.OnEntered += HandleReachedEntered;
            reachedState.OnExited += HandleReachedExited;

            // Transiciones desde Idle
            stateMachine.AddTransition(idleState, followState, IsTargetInDetectionRange);
            stateMachine.AddTransition(idleState, patrolState, () => idleState.IsIdleComplete && context.HasPatrolPoints());

            // Transiciones desde Patrol
            stateMachine.AddTransition(patrolState, followState, IsTargetInDetectionRange);
            stateMachine.AddTransition(patrolState, idleState, () => patrolState.HasReachedWaypoint);

            // Transiciones desde Follow
            stateMachine.AddTransition(followState, attackState, CanAttackTarget);
            stateMachine.AddTransition(followState, idleState, HasLostTarget);

            // Transiciones desde Attack (acertado -> Reached | fallido -> Follow o Idle)
            stateMachine.AddTransition(attackState, reachedState, () => attackState.IsAttackFinished && attackState.DidHitTarget);
            stateMachine.AddTransition(attackState, followState, () => attackState.IsAttackFinished && !attackState.DidHitTarget && IsTargetInDetectionRange());
            stateMachine.AddTransition(attackState, idleState, () => attackState.IsAttackFinished && !attackState.DidHitTarget && !IsTargetInDetectionRange());

            // Transiciones desde Reached (prioridad: perseguir > patrullar > reposo)
            stateMachine.AddTransition(reachedState, followState, () => reachedState.IsRecoveryComplete && IsTargetInDetectionRange());
            stateMachine.AddTransition(reachedState, patrolState, () => reachedState.IsRecoveryComplete && !IsTargetInDetectionRange() && context.HasPatrolPoints());
            stateMachine.AddTransition(reachedState, idleState, () => reachedState.IsRecoveryComplete && !IsTargetInDetectionRange() && !context.HasPatrolPoints());
        }

        private bool IsTargetInDetectionRange()
        {
            return context.HasValidTarget() && context.GetDistanceToTarget() <= config.detectionRange;
        }

        private bool HasLostTarget()
        {
            return !context.HasValidTarget() || context.GetDistanceToTarget() > config.loseTargetRange;
        }

        private bool CanAttackTarget()
        {
            return context.HasValidTarget()
                && context.GetDistanceToTarget() <= config.attackRange
                && context.IsAttackOffCooldown();
        }

        private void OnDrawGizmosSelected()
        {
            if (config != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, config.detectionRange);

                Gizmos.color = new Color(1f, 0.5f, 0f);
                Gizmos.DrawWireSphere(transform.position, config.loseTargetRange);

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, config.attackRange);
            }

            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                return;
            }

            Gizmos.color = Color.cyan;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] == null)
                {
                    continue;
                }

                Gizmos.DrawSphere(patrolPoints[i].position, 0.25f);

                Transform nextPoint = patrolPoints[(i + 1) % patrolPoints.Length];
                if (nextPoint != null && patrolPoints.Length > 1)
                {
                    Gizmos.DrawLine(patrolPoints[i].position, nextPoint.position);
                }
            }
        }
    }
}