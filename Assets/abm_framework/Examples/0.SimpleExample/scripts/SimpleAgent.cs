using UnityEngine;
using ABMU.Core;

public class SimpleAgent : AbstractAgent
{
    Vector3 locationToMoveTo;

    public override void Init() {
        base.Init();
        CreateStepper(Move, 1, 300);
        CreateStepper(ChooseNextLocationToMoveTo, 1, 200);
    }

    void Move() {
        this.transform.position = locationToMoveTo;
    }

    void ChooseNextLocationToMoveTo() {
        locationToMoveTo = this.transform.position + Random.insideUnitSphere;
    }
}