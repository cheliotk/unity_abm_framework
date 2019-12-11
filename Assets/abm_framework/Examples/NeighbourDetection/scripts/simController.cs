using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABM.Core;
using ABM;

public class simController : AbstractController
{
    public GameObject agentPrefab;
    public int numAgents;
    public Bounds bounds;

    public override void Init(){
        base.Init();

        if(bounds == null){
            bounds = new Bounds(Vector3.zero, new Vector3(100f,10f,100f));
        }
        
        for (int i = 0; i < numAgents; i++)
        {
            GameObject a = Instantiate(agentPrefab);

            Vector3 pos = Utilities.RandomPointInBounds(bounds);
            float speed = RandomFromDistribution.RandomRangeNormalDistribution(1f,3f,RandomFromDistribution.ConfidenceLevel_e._999);

            simAgent ascript = a.GetComponent<simAgent>();
            ascript.Init(pos, bounds, speed);
        }
    }

    public override void Step(){
        // AgentStepLoop(0,300);
        // AgentStepLoop(300,int.MaxValue);

        Step(0,300);
        Step(300,int.MaxValue);
        Step(800,int.MaxValue);
    }
}
