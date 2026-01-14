using UnityEngine;
using UnityEngine.Rendering;

public class Roaming : MonoBehaviour
{
    public Vector3 Roam(float range)
    {
        float dir = Random.Range(-180,180);
        Vector3 moveto = Random.insideUnitSphere * range;
        moveto += transform.position;
        return moveto;
    }
}
