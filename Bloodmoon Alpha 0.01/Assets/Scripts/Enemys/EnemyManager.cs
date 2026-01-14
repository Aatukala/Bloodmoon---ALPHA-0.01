using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    private EnemyUpdate[] Enemys;
    private GameObject player;
    private int count = 0;
    private int limiter = 0;
    public float range = 20f;

    private void Start()
    {
        player = GameObject.Find("Player");
        Enemys = GetComponentsInChildren<EnemyUpdate>();
    }

    void Update()
    {
        for (; count < Enemys.Length && limiter < Enemys.Length / 100 + 1 ; count++)
        {
            Enemys[count].NewCheck(range, player);
            limiter++;
        }
        limiter = 0;
        if (count >= Enemys.Length)
        {
            count = 0;
            Enemys = GetComponentsInChildren<EnemyUpdate>();
        }
    }
}
