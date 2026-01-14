using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public void MoveTo(Vector3 here, NavMeshAgent agent, float range)
    {
        if (NavMesh.SamplePosition(here, out NavMeshHit hit, range, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
