using UnityEngine;
using UnityEngine.AI;

namespace AI.Enemy
{
    public class EnemyContext
    {
        public static readonly int SpeedHash = Animator.StringToHash("Speed");
        public static readonly int AttackHash = Animator.StringToHash("Attack");
        public static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
        public static readonly int IsReachedHash = Animator.StringToHash("IsReached");

        public Transform Transform { get; }
        public NavMeshAgent Agent { get; }
        public Animator Animator { get; }
        public EnemyConfig Config { get; }
        public Transform Target { get; set; }
        public Transform[] PatrolPoints { get; }

        public int CurrentPatrolIndex { get; set; }
        public float LastAttackTime { get; set; } = float.NegativeInfinity;

        public EnemyContext(
            Transform transform,
            NavMeshAgent agent,
            Animator animator,
            EnemyConfig config,
            Transform target,
            Transform[] patrolPoints)
        {
            Transform = transform;
            Agent = agent;
            Animator = animator;
            Config = config;
            Target = target;
            PatrolPoints = patrolPoints;
        }

        public bool HasValidTarget()
        {
            return Target != null && Target.gameObject.activeInHierarchy;
        }

        public bool HasPatrolPoints()
        {
            return PatrolPoints != null && PatrolPoints.Length > 0;
        }

        public float GetDistanceToTarget()
        {
            if (!HasValidTarget())
            {
                return float.PositiveInfinity;
            }

            return Vector3.Distance(Transform.position, Target.position);
        }

        public bool IsAttackOffCooldown()
        {
            return Time.time >= LastAttackTime + Config.attackCooldown;
        }
    }
}