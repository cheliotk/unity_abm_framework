using UnityEngine;
using ABMU.Core;

public class DiffusionController: AbstractController
{
    public GameObject agentPrefab;
    public int numRowsX, numRowsY;
    public float size;

    public override void Init(){
        base.Init();
        for (int i = 0; i < numRowsX; i++)
        {
            for (int j = 0; j < numRowsY; j++)
            {
                Vector3 p = new Vector3(i * size, 0, j * size);
                GameObject a = Instantiate(agentPrefab);
                a.GetComponent<DiffusionAgent>().Init(p, size, Random.Range(0f,1f));
            }
        }
    }
}