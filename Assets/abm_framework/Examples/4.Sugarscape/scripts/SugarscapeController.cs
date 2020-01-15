using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABMU.Core;

public class SugarscapeController : AbstractController
{
    public GameObject agentPf;
    public GameObject patchPf;
    public TextAsset sugarMap;
    public int numAgents = 50;

    List<SugarscapePatch> patches;
    public override void Init(){
        base.Init();

        patches = new List<SugarscapePatch>();

        GeneratePatches();

        for (int i = 0; i < numAgents; i++)
        {
            CreateAgent();
        }
    }

    public void CreateAgent(){
        GameObject a = Instantiate(agentPf);

        List<SugarscapePatch> unoccupiedPatches = patches.FindAll(p => p.IsOccupied() == false);
        a.GetComponent<SugarscapeAgent>().InitAtPatch(unoccupiedPatches[Random.Range(0,unoccupiedPatches.Count)]);

    }

    void GeneratePatches(){
        string[] lines = sugarMap.text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string[] cells = lines[i].Split(' ');
            for (int j = 0; j < cells.Length; j++)
            {
                GameObject p = Instantiate(patchPf);
                Vector3 pos = new Vector3(i,0,j);
                p.transform.transform.position = pos;

                int sugarVal = int.Parse(cells[j]);
                p.GetComponent<SugarscapePatch>().Init(sugarVal);
                
                patches.Add(p.GetComponent<SugarscapePatch>());
            }
        }
    }
}
