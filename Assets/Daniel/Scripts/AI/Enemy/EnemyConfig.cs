using UnityEngine;

namespace AI.Enemy
{
    [CreateAssetMenu(fileName = "NewEnemyConfig", menuName = "AI/Enemy Config")]
    public class EnemyConfig : ScriptableObject
    {
        [Header("Velocidades")]
        [Min(0f)] public float patrolSpeed = 2.0f;
        [Min(0f)] public float followSpeed = 4.5f;

        [Header("Distancias")]
        [Min(0f)] public float detectionRange = 10.0f;
        [Min(0f)] public float loseTargetRange = 14.0f;
        [Min(0f)] public float attackRange = 2.0f;
        [Min(0.1f)] public float waypointTolerance = 0.5f;

        [Header("Tiempos")]
        [Min(0f)] public float idleDuration = 2.0f;
        [Min(0.1f)] public float attackDuration = 0.8f;
        [Min(0f)] public float attackCooldown = 1.5f;
        [Min(0f)] public float reachedDuration = 1.0f;
        [Min(0.05f)] public float destinationRefreshRate = 0.2f;

        [Header("Rotación")]
        [Min(1f)] public float attackRotationSpeed = 8.0f;
    }
}