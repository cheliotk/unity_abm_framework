using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABM.Core;
using ABM;
public class SegregationController : AbstractController
{
    [Header("Environment Variables")]
    public GameObject environmentCellPf;
    public Vector3Int environmentExtents;
    public float cellSizeInGameWorld = 20f;

    [Header("Agent Variables")]
    public GameObject agentPf;
    public LayerMask agentLm;

    [Header("Simulation Variables")]
    [Range(0f,1.0f)]
    public float similarityThreshold = 0.4f;
    [Range(0f,1.0f)]
    public float agentPopulationPerc = 0.7f;
    public override void Init(){
        base.Init();

        GenerateEnvironment();
        GenerateAgents();
    }

    void GenerateEnvironment(){
        for (int i = 0; i < environmentExtents.y; i++)
        {
            for (int j = 0; j < environmentExtents.x; j++)
            {
                for (int k = 0; k < environmentExtents.z; k++)
                {
                    Vector3 pos = new Vector3(j,i,k) * cellSizeInGameWorld;
                    Instantiate(environmentCellPf, pos, Quaternion.identity);
                }
            }
        }
    }

    void GenerateAgents(){
        int envCapacity = environmentExtents.x * environmentExtents.y * environmentExtents.z;
        int numAgents = Mathf.CeilToInt(envCapacity * agentPopulationPerc);
        for (int i = 0; i < numAgents; i++)
        {
            GameObject a = Instantiate(agentPf);
            SegregationAgent sa = a.GetComponent<SegregationAgent>();
            cellScript cell = FindFreeCell();

            sa.Init(cell);
        }
    }

    // void PlaceAndInstantiateAgents(){
    //     print(agents.Count);
    //     foreach (var a in agents)
    //     {
    //         PlaceAgent(a);
    //     }
    // }

    public override void Step(){
        Step(0,150);
        Step(150,250);
        Step(250,350);
    }

    public cellScript FindFreeCell(){

        cellScript[] cells = GameObject.FindObjectsOfType<cellScript>();
        cellScript cs = cells[Random.Range(0,cells.Length)];

        while(cs.isOccupied){
            cs = cells[Random.Range(0,cells.Length)];
        }

        return (cs);

    }
}
