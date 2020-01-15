using UnityEngine;
using ABMU.Core;

public class SimpleController : AbstractController
{
    public GameObject agentPrefab;
    public int numberOfAgents = 100;

    public override void Init(){
        base.Init();

        for (int i = 0; i < numberOfAgents; i++)
        {
            GameObject a = Instantiate(agentPrefab);
            a.GetComponent<SimpleAgent>().Init();
        }
    }
}