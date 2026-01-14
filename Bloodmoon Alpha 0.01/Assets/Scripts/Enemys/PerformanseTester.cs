using UnityEngine;

public class PerformanseTester : MonoBehaviour
{
    public int numbers = 1000;
    public GameObject enemy;
    private void Start()
    {
        for (int i = 0; i < numbers; i++)
        {
            Instantiate(enemy, new Vector3(Random.Range(-45, 45), 1, Random.Range(-45, 45)), new Quaternion(), transform);
        }
    }
}
