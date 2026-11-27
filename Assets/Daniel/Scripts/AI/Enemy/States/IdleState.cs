using AI.FSM;

namespace AI.Enemy.States
{
    public class IdleState : BaseState
    {
        private readonly EnemyContext context;
        private float elapsedTime;

        public bool IsIdleComplete => elapsedTime >= context.Config.idleDuration;

        public IdleState(EnemyContext context)
        {
            this.context = context;
        }

        public override void Enter()
        {
            elapsedTime = 0f;

            if (context.Agent.isOnNavMesh)
            {
                context.Agent.isStopped = true;
                context.Agent.ResetPath();
            }

            context.Animator?.SetFloat(EnemyContext.SpeedHash, 0f);
        }

        public override void Tick(float deltaTime)
        {
            elapsedTime += deltaTime;
        }
    }
}