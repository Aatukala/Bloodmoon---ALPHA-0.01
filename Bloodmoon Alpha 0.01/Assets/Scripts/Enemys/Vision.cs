using UnityEngine;

public class Vision : MonoBehaviour
{
    public bool vision(GameObject player)
    {
        if (Physics.Raycast(transform.position, player.transform.position, out RaycastHit hit)) 
        {
            return false;
        }
        return true;
    }
}
