using UnityEngine;
using ABM.Core;

public class SimpleAgent : AbstractAgent
{
    public override void Init(){
        base.Init();
        CreateStepper(Move);
    }

    void Move(){
        this.transform.position += Random.insideUnitSphere;
    }
}