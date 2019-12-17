using UnityEngine;
using ABM.Core;
using ABM;

public class simController : AbstractController
{
    [Header("Simulation Variables")]
    public GameObject groundPrefab;
    public Bounds bounds;

    [Header("Agent Variables")]
    public GameObject agentPrefab;
    public int numAgents;
    [Range(1f, 3f)]
    public float speed = 1.5f;
    [Range(1f, 180f)]
    public float rotationAngle = 90f;
    [Range(1f, 50f)]
    public float viewRange = 25f;

    public override void Init(){
        base.Init();

        if(bounds == null){
            bounds = new Bounds(Vector3.zero, new Vector3(100f,10f,100f));
        }
        GameObject groundPlane = Instantiate(groundPrefab, bounds.center, Quaternion.identity);
        groundPlane.transform.localScale = bounds.extents*2f/10f;
        
        for (int i = 0; i < numAgents; i++)
        {
            GameObject a = Instantiate(agentPrefab);

            Vector3 pos = Utilities.RandomPointInBounds(bounds);
            
            simAgent ascript = a.GetComponent<simAgent>();
            ascript.Init(pos, bounds, speed, rotationAngle, viewRange);
        }
    }
}
