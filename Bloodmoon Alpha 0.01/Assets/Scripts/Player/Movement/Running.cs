using UnityEngine;

public class Running : MonoBehaviour
{
    public float run()     
    {
        float speedMultiplier = 1f;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speedMultiplier = 2.5f;
        }
        return speedMultiplier;
    }
}
