using AI.FSM;
using UnityEngine;

namespace AI.Enemy.States
{
    public class AttackState : BaseState
    {
        private readonly EnemyContext context;
        private float elapsedTime;
        private bool hasExecutedHit;

        public bool IsAttackFinished { get; private set; }
        public bool DidHitTarget { get; private set; }

        public AttackState(EnemyContext context)
        {
            this.context = context;
        }

        public override void Enter()
        {
            elapsedTime = 0f;
            hasExecutedHit = false;
            IsAttackFinished = false;
            DidHitTarget = false;

            context.LastAttackTime = Time.time;

            if (context.Agent.isOnNavMesh)
            {
                context.Agent.isStopped = true;
                context.Agent.ResetPath();
            }

            if (context.Animator != null)
            {
                context.Animator.SetFloat(EnemyContext.SpeedHash, 0f);
                context.Animator.SetBool(EnemyContext.IsAttackingHash, true);
                context.Animator.ResetTrigger(EnemyContext.AttackHash);
                context.Animator.SetTrigger(EnemyContext.AttackHash);
            }

            Debug.Log($"[{context.Transform.name}] Ejecutando ataque.");
        }

        public override void Tick(float deltaTime)
        {
            if (IsAttackFinished)
            {
                return;
            }

            RotateTowardsTarget(deltaTime);

            elapsedTime += deltaTime;
            if (elapsedTime >= context.Config.attackDuration)
            {
                if (!hasExecutedHit)
                {
                    NotifyAttackHit();
                }

                NotifyAttackFinished();
            }
        }

        public override void Exit()
        {
            if (context.Animator != null)
            {
                context.Animator.SetBool(EnemyContext.IsAttackingHash, false);
                context.Animator.ResetTrigger(EnemyContext.AttackHash);
            }
        }

        public void NotifyAttackHit()
        {
            if (hasExecutedHit)
            {
                return;
            }

            hasExecutedHit = true;

            if (context.HasValidTarget() && context.GetDistanceToTarget() <= context.Config.attackRange + 0.5f)
            {
                DidHitTarget = true;
                Debug.Log($"[{context.Transform.name}] Impacto confirmado sobre {context.Target.name}.");
            }
            else
            {
                DidHitTarget = false;
                Debug.Log($"[{context.Transform.name}] Ataque fallido: objetivo fuera de alcance.");
            }
        }

        public void NotifyAttackFinished()
        {
            IsAttackFinished = true;
        }

        private void RotateTowardsTarget(float deltaTime)
        {
            if (!context.HasValidTarget())
            {
                return;
            }

            Vector3 direction = context.Target.position - context.Transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            context.Transform.rotation = Quaternion.Slerp(
                context.Transform.rotation,
                targetRotation,
                context.Config.attackRotationSpeed * deltaTime
            );
        }
    }
}