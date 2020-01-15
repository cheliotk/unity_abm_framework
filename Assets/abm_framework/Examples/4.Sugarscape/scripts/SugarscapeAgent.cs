using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABMU.Core;
using ABMU;

public class SugarscapeAgent : AbstractAgent
{
    SugarscapeController sCont;
    int sugar;
    int metabolism;
    int maxAge;
    int age = 0;
    int visionRange;
    SugarscapePatch currentPatch;

    public LayerMask patchesLm;

    public void InitAtPatch(SugarscapePatch patchToSpawnAt){
        base.Init();

        sCont = GameObject.FindObjectOfType<SugarscapeController>();
        
        RelocateToPatch(patchToSpawnAt);

        sugar = currentPatch.HarvestSugar();
        metabolism = Random.Range(1,4);
        maxAge = Random.Range(60,100);
        visionRange = Random.Range(1,6);

        CreateStepper(Move, 1, 105);
        CreateStepper(Eat, 1, 106);
        CreateStepper(Age, 1, 107);
    }

    void Move(){
        List<SugarscapePatch> validPatches = GetPatchesInRange();
        validPatches.Shuffle();
        SugarscapePatch spToMoveTo = currentPatch;
        
        float minDist = float.MaxValue;
        foreach (var sp in validPatches)
        {
            float dist = Vector3.Distance(this.transform.position, sp.transform.position);
            
            if(dist <= minDist){
                spToMoveTo = sp;
                minDist = dist;
            }
        }

        RelocateToPatch(spToMoveTo);
    }

    void RelocateToPatch(SugarscapePatch sp){
        Vector3 newPos = sp.transform.position;
        newPos.y = this.transform.localScale.y/2f;
        this.transform.position = newPos;

        if(currentPatch){
            currentPatch.Occupy(null);
        }
        currentPatch = sp;
        currentPatch.Occupy(this);
    }

    void Eat(){
        sugar -= metabolism;
        sugar += currentPatch.HarvestSugar();
    }

    void Age(){
        age ++;
        if(age > maxAge || sugar <= 0){
            Die();
        }
    }

    void Die(){
        if(currentPatch){
            currentPatch.Occupy(null);
        }
        sCont.CreateAgent();
        GameObject.Destroy(this.gameObject);
    }

    List<SugarscapePatch> GetPatchesInRange(){
        Collider[] colsInRange = Physics.OverlapSphere(this.transform.position,
            visionRange,
            patchesLm);

        int maxSugar = -1;
        List<SugarscapePatch> allPatchesInRange = new List<SugarscapePatch>();
        foreach (Collider c in colsInRange)
        {
            SugarscapePatch sp = c.GetComponent<SugarscapePatch>();

            if(!sp.IsOccupied()){
                allPatchesInRange.Add(sp);

                if(sp.pSugar > maxSugar){
                    maxSugar = sp.pSugar;
                }
            }
        }

        return allPatchesInRange.FindAll(p => p.pSugar == maxSugar);
    }
}