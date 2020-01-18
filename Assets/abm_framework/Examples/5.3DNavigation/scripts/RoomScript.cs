using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ABMU;

public class RoomScript : MonoBehaviour
{
    NavigationController nCont;
    int numAgents = 0;
    Renderer floorRenderer;
    Bounds bounds;

    Color c;
    
    void Start()
    {
        nCont = GameObject.FindObjectOfType<NavigationController>();

        floorRenderer = this.transform.GetChild(0).GetComponent<Renderer>();
        this.GetComponent<Renderer>().enabled = false;

        bounds = this.GetComponent<Renderer>().bounds;
    }

    private void LateUpdate() {
        DetectAgentsInRoom();
        SetColor(); 
    }

    void SetColor(){
        float val = (float)numAgents/nCont.agents.Count;
        val = Utilities.Map(val, 0f, nCont.heatmapUpperBound, 0f, 1f);
        c = nCont.heatmapGradient.Evaluate(val);
        floorRenderer.material.color = c;  
    }

    void DetectAgentsInRoom(){
        Collider[] cols = Physics.OverlapBox(bounds.center, bounds.extents*0.9f, this.transform.rotation, nCont.agentLm);
        numAgents = cols.Length;
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(c.r, c.g, c.b, 1f);
        Gizmos.DrawWireCube(bounds.center, bounds.size*0.9f);
    }
}
