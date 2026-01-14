using UnityEngine;
using UnityEngine.AI;

public class EnemyUpdate : MonoBehaviour
{

    private EnemyMovement move;
    private Roaming roam;
    private Vision see;

    private NavMeshAgent agent;

    private Vector3 moveto;
    public void NewCheck(float range, GameObject player)
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if (move == null)
        {
            move = GetComponent<EnemyMovement>();
        }
        if (roam == null)
        {
            roam = GetComponent<Roaming>();
        }
        if (see == null)
        {
            see = GetComponent<Vision>();
        }
        if (see.vision(player))
        {
            move.MoveTo(player.transform.position, agent, 10f);
        }
        else
        {
            moveto = roam.Roam(range);
            move.MoveTo(moveto, agent, range);
        }
    }
}
