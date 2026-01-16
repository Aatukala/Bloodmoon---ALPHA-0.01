using Mono.Cecil.Cil;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public bool building = false;

    public GameObject Ghoust;

    public List<GameObject> buildings;

    public LayerMask mask;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            building = !building;
        }
        if (building)
        {
            if (Ghoust == null)
            {
                Ghoust = Instantiate(buildings[0]);
                BoxCollider[] box = Ghoust.GetComponentsInChildren<BoxCollider>();
                for (int i = 0; i < box.Length; ++i)
                {
                    box[i].enabled = false;
                }
            }
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 50f, mask))
            {
                Ghoust.transform.position = hit.point;
                Ghoust.SetActive(true);
            }
            else { Ghoust.SetActive(false); }
            if (Ghoust.active && Input.GetMouseButtonDown(0))
            {
                Instantiate(buildings[0],hit.point,new Quaternion(), transform);
            }
        }
        else if (Ghoust != null)
        {
            Destroy(Ghoust);
            Ghoust = null;
        }
    }
}
