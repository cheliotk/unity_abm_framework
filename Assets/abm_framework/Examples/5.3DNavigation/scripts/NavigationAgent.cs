using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ABMU.Core;

using UnityEngine.AI;

public class NavigationAgent : AbstractAgent
{
    NavigationController nCont;
    NavMeshAgent nmAgent;
    Vector3 target;
    public GameObject targetRoom;
    public bool isNearTarget = false;

    int timeSpentSitting = 0;
    int stationayrDuration = -1;

    float fracOfSecPerFrame = 30f;

    public override void Init(){
        base.Init();
        nCont = GameObject.FindObjectOfType<NavigationController>();
        nmAgent = GetComponent<NavMeshAgent>();
        fracOfSecPerFrame = nCont.fracOfSecPerFrame;

        SetNMAgentProperties();
        SetupStationary();
    }

    public void SetTarget(GameObject room){
        targetRoom = room;
        target = nCont.GetRandomPointInRoom(targetRoom);
        nmAgent.SetDestination(target);
        nmAgent.isStopped = false;

        CreateStepper(CheckDistToTarget, 1, 100);
        CreateStepper(Move, 1, 105);
    }

    void CheckDistToTarget(){
        float d = Vector3.Distance(this.transform.position, target);
        if(d < nCont.distToTargetThreshold){ //reached target
            isNearTarget = true;

            nmAgent.isStopped = true;
            
            DestroyStepper("CheckDistToTarget");
            DestroyStepper("Move");

            SetupStationary();
        }
        else{
            isNearTarget = false;
        }
    }

    void Move(){
        nmAgent.nextPosition = this.transform.position + nmAgent.desiredVelocity * (1/fracOfSecPerFrame);
        transform.LookAt(nmAgent.nextPosition, Vector3.up);
        transform.position =  nmAgent.nextPosition;
    }
    
    void SetupStationary(){
        stationayrDuration = Random.Range(50,1000);
        timeSpentSitting = 0;
        CreateStepper(Stay);
    }

    void Stay(){
        timeSpentSitting ++;
        if(timeSpentSitting > stationayrDuration){
            SetNewTarget();
            DestroyStepper("Stay");
        }
    }

    void SetNewTarget(){
        SetTarget(nCont.GetRandomRoom());
    }

    void SetNMAgentProperties(){
        nmAgent.updatePosition = false;
        nmAgent.velocity = Vector3.zero;
        nmAgent.acceleration = 0f;
    }
}
