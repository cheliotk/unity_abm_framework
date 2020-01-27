using UnityEngine;
using ABMU.Core;
using ABMU;

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
        Vector2 tiling = new Vector2(groundPlane.transform.localScale.x, groundPlane.transform.localScale.z);
        groundPlane.GetComponent<Renderer>().material.mainTextureScale = tiling;
        
        for (int i = 0; i < numAgents; i++)
        {
            Vector3 pos = Utilities.RandomPointInBounds(bounds);

            GameObject a = Instantiate(agentPrefab, pos, Quaternion.AngleAxis(Random.Range(0f,360f), Vector3.up));
            
            simAgent ascript = a.GetComponent<simAgent>();
            ascript.Init(pos, bounds, speed, rotationAngle, viewRange);
        }
    }
}
