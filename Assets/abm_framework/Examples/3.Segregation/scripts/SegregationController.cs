using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using ABMU.Core;

public class SegregationController : AbstractController
{
    [Header("Environment Variables")]
    public GameObject environmentCellPf;
    public Vector3Int environmentExtents;
    public cellScript[] cells;
    public float cellSizeInGameWorld = 20f;

    [Header("Agent Variables")]
    public GameObject agentPf;
    public LayerMask agentLm;
    public LayerMask cellLm;

    [Header("Simulation Variables")]
    [Range(0f,1.0f)]
    public float similarityThreshold = 0.4f;
    [Range(0f,1.0f)]
    public float agentPopulationPerc = 0.7f;
    public System.Random rand;
    
    [Range(int.MinValue + 64, int.MaxValue - 64)]
    public int randomSeed = 0;

    public override void Init(){
        base.Init();

        rand = new System.Random(randomSeed);

        GenerateEnvironment();
        SetEnvironmentCellNeighbourhoods();
        GenerateAgents();
    }

    public override void Step(){
        int agentsSettled = 0;
        base.Step();
        foreach (SegregationAgent agent in agents)
        {
            if(!agent.needToMove){
                agentsSettled ++;
            }
        }
        if(agentsSettled == agents.Count){
            EditorApplication.isPaused = true;
        }

    }

    void GenerateEnvironment(){
        List<cellScript> cellsList = new List<cellScript>();
        for (int i = 0; i < environmentExtents.y; i++)
        {
            for (int j = 0; j < environmentExtents.x; j++)
            {
                for (int k = 0; k < environmentExtents.z; k++)
                {
                    Vector3 pos = new Vector3(j,i,k) * cellSizeInGameWorld;
                    pos -= new Vector3(environmentExtents.x/2f, environmentExtents.y/2f, environmentExtents.z/2f) * cellSizeInGameWorld;
                    GameObject bob = Instantiate(environmentCellPf, pos, Quaternion.identity);
                    cellsList.Add(bob.GetComponent<cellScript>());
                }
            }
        }
        cells = cellsList.ToArray();
    }

    void SetEnvironmentCellNeighbourhoods(){
        cellScript[] cells = GameObject.FindObjectsOfType<cellScript>();
        foreach (var cell in cells)
        {
            cell.GetCellNeighbours(this);
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

    public cellScript FindFreeCell(){
        // cellScript cs = cells[Random.Range(0,cells.Length)];

        // while(cs.isOccupied){
        //     cs = cells[Random.Range(0,cells.Length)];
        // }
        // return (cs);

        cellScript cs = cells[rand.Next(cells.Length)];

        while(cs.isOccupied){
            cs = cells[rand.Next(cells.Length)];
        }
        return (cs);
    }

    public void RecordAgentSettled(bool isSettled){

    }
}