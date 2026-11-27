using UnityEngine;

namespace AI.Enemy
{
    public class EnemyAnimationEventRelay : MonoBehaviour
    {
        private EnemyController controller;

        private void Awake()
        {
            controller = GetComponentInParent<EnemyController>();
        }

        public void OnAttackHit()
        {
            controller?.OnAttackHit();
        }

        public void OnAttackFinished()
        {
            controller?.OnAttackFinished();
        }

        public void OnReachedFinished()
        {
            controller?.OnReachedFinished();
        }
    }
}