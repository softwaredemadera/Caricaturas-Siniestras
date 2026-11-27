
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshTestMover : MonoBehaviour
{
    [SerializeField] private Transform target;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("NavMeshTestMover: No se ha asignado un objetivo en el Inspector.", this);
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("NavMeshTestMover: El enemigo NO está sobre un NavMesh válido.", this);
            return;
        }

        agent.SetDestination(target.position);
    }
}