using System;
using AI.FSM;

namespace AI.Enemy.States
{
    public class ReachedState : BaseState
    {
        private readonly EnemyContext context;
        private float elapsedTime;

        public bool IsRecoveryComplete { get; private set; }

        public event Action OnEntered;
        public event Action OnExited;

        public ReachedState(EnemyContext context)
        {
            this.context = context;
        }

        public override void Enter()
        {
            elapsedTime = 0f;
            IsRecoveryComplete = false;

            if (context.Agent.isOnNavMesh)
            {
                context.Agent.isStopped = true;
                context.Agent.ResetPath();
            }

            if (context.Animator != null)
            {
                context.Animator.SetFloat(EnemyContext.SpeedHash, 0f);
                context.Animator.SetBool(EnemyContext.IsReachedHash, true);
            }

            OnEntered?.Invoke();
        }

        public override void Tick(float deltaTime)
        {
            if (IsRecoveryComplete)
            {
                return;
            }

            elapsedTime += deltaTime;
            if (elapsedTime >= context.Config.reachedDuration)
            {
                NotifyRecoveryFinished();
            }
        }

        public override void Exit()
        {
            context.Animator?.SetBool(EnemyContext.IsReachedHash, false);
            OnExited?.Invoke();
        }

        public void NotifyRecoveryFinished()
        {
            IsRecoveryComplete = true;
        }
    }
}