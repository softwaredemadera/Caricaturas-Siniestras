using AI.FSM;

namespace AI.Enemy.States
{
    public class FollowState : BaseState
    {
        private readonly EnemyContext context;
        private float refreshTimer;

        public FollowState(EnemyContext context)
        {
            this.context = context;
        }

        public override void Enter()
        {
            refreshTimer = 0f;

            if (!context.Agent.isOnNavMesh)
            {
                return;
            }

            context.Agent.isStopped = false;
            context.Agent.speed = context.Config.followSpeed;
            UpdateDestination();
        }

        public override void Tick(float deltaTime)
        {
            context.Animator?.SetFloat(EnemyContext.SpeedHash, context.Agent.velocity.magnitude, 0.1f, deltaTime);

            if (!context.HasValidTarget() || !context.Agent.isOnNavMesh)
            {
                return;
            }

            refreshTimer += deltaTime;
            if (refreshTimer >= context.Config.destinationRefreshRate)
            {
                refreshTimer = 0f;
                UpdateDestination();
            }
        }

        private void UpdateDestination()
        {
            if (context.HasValidTarget())
            {
                context.Agent.SetDestination(context.Target.position);
            }
        }
    }
}