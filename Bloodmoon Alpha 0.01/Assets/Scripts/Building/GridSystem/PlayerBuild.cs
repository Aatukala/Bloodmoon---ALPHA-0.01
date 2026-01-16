using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuild : MonoBehaviour
{
    public bool building = false;
    
    public List<GameObject> Floors;

    private GameObject Ghoust;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            building = !building;
        }
        if (building)
        {
            if (Ghoust == null)
            {
                Ghoust = Instantiate(Floors[0]);
                BoxCollider[] box = Ghoust.GetComponentsInChildren<BoxCollider>();
                for (int i = 0; i < box.Length; i++)
                {
                    box[i].enabled = false;
                }
            }
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 10, Color.pink);
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 10))
            {
                Ghoust.transform.position = hit.point;
            }
            //do shit
        }
    }
}
