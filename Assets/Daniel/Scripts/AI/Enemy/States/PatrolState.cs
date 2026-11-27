using AI.FSM;
using UnityEngine;

namespace AI.Enemy.States
{
    public class PatrolState : BaseState
    {
        private readonly EnemyContext context;

        public bool HasReachedWaypoint { get; private set; }

        public PatrolState(EnemyContext context)
        {
            this.context = context;
        }

        public override void Enter()
        {
            HasReachedWaypoint = false;

            if (!context.HasPatrolPoints() || !context.Agent.isOnNavMesh)
            {
                HasReachedWaypoint = true;
                return;
            }

            Transform destinationPoint = context.PatrolPoints[context.CurrentPatrolIndex];
            if (destinationPoint == null)
            {
                AdvancePatrolIndex();
                HasReachedWaypoint = true;
                return;
            }

            context.Agent.isStopped = false;
            context.Agent.speed = context.Config.patrolSpeed;
            context.Agent.SetDestination(destinationPoint.position);
        }

        public override void Tick(float deltaTime)
        {
            context.Animator?.SetFloat(EnemyContext.SpeedHash, context.Agent.velocity.magnitude, 0.1f, deltaTime);

            if (HasReachedWaypoint || !context.Agent.isOnNavMesh)
            {
                return;
            }

            if (context.Agent.pathPending)
            {
                return;
            }

            if (context.Agent.remainingDistance <= context.Config.waypointTolerance)
            {
                AdvancePatrolIndex();
                HasReachedWaypoint = true;
            }
        }

        private void AdvancePatrolIndex()
        {
            context.CurrentPatrolIndex = (context.CurrentPatrolIndex + 1) % context.PatrolPoints.Length;
        }
    }
}