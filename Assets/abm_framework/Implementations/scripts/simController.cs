using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABM.Core;

public class simController : MonoBehaviour, ISteppable, IController
{
    // [SerializeField] int _step;
    // public int step{
    //     get
    //     {
    //         return _step;
    //     }
    //     set{
    //         _step = value;
    //     }
    // }

    List<IAgent> _agents;
    public List<IAgent> agents{
        get{
            return _agents;
        }
    }

    public void RegisterAgent(IAgent a){
        _agents.Add(a);
    }

    public void DeregisterAgent(IAgent a){
        _agents.Remove(a);
    }

    public void Init(object bob = null){
        _agents = new List<IAgent>();
        for (int i = 0; i < numAgents; i++)
        {
            GameObject a = Instantiate(agentPrefab);
            simAgent ascript = a.GetComponent<simAgent>();
            ascript.Init(Random.insideUnitSphere);
            ascript.speed = RandomFromDistribution.RandomRangeNormalDistribution(1f,3f,RandomFromDistribution.ConfidenceLevel_e._999);

            
        }

        if(bounds == null){
            bounds = new Bounds(Vector3.zero, new Vector3(100f,10f,100f));
        }
    }

    public void Step(){
        foreach (IAgent a in agents)
        {
            a.Step();
            // if(Time.frameCount%a.step == 0){
            //     a.Step();
            // }
        }
    }

    public GameObject agentPrefab;
    public int numAgents;
    public Bounds bounds;

    void Start(){
        Init();
    }

    void Update(){
        // if(Time.frameCount % step == 0){
            Step();
        // }
    }
    
}
