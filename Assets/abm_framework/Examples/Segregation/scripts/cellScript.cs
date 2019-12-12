using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cellScript : MonoBehaviour
{
    public bool isOccupied = false;
    public SegregationAgent occupier;
    public cellScript[] neighbours;

    public void GetCellNeighbours(SegregationController segController){
        Collider[] others = Physics.OverlapBox(this.transform.transform.position, Vector3.one*segController.cellSizeInGameWorld,Quaternion.identity,segController.cellLm);

        List<cellScript> ns = new List<cellScript>();
        
        foreach (Collider other in others)
        {
            if(other.gameObject != this.gameObject){
                ns.Add(other.GetComponent<cellScript>());
            }
        }
        neighbours = ns.ToArray();
    }
}
