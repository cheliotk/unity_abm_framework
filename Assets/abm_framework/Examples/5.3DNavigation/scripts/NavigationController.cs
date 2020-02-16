using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ABMU.Core;
using ABMU;

using UnityEngine.AI;

using UnityEditor;

public class NavigationController : AbstractController
{
    [Header("Simulation Parameters")]
    public GameObject agentPrefab;
    public List<GameObject> rooms;
    public float heatmapUpperBound = 0.15f;
    public Gradient heatmapGradient;
    // public int endFrame = 10000;
    // int frameCount;
    public int numAgents = 100;

    [Header("Agent Parameters")]
    public float distToTargetThreshold = 2f;
    [Tooltip("For agent speed calculation, e.g. a value of 30 means that each frame simulates one thirtieth of a second.")]
    public float fracOfSecPerFrame = 30f;
    public LayerMask agentLm;

    public override void Init(){
        base.Init();

        rooms = GetAllRooms();

        for (int i = 0; i < numAgents; i++)
        {
            GameObject agent = Instantiate(agentPrefab);
            NavMeshAgent nmAgent = agent.GetComponent<NavMeshAgent>();
            nmAgent.Warp(GetRandomPointInRoom(GetRandomRoom()));
            agent.transform.position = nmAgent.nextPosition;

            agent.GetComponent<NavigationAgent>().Init();
        }
    }

    public override void Step(){
        PauseAtFrame();
        base.Step();
    }

    public GameObject GetRandomRoom(){
        GameObject room = rooms[Random.Range(0,rooms.Count)];
        return room;
    }

    public Vector3 GetRandomPointInRoom(GameObject room){
        Bounds rb = room.GetComponent<Collider>().bounds;
        Vector3 cr = Utilities.RandomPointInBounds(rb);
        cr.y = rb.center.y;
        cr.y -= rb.extents.y;
        cr.y += agentPrefab.GetComponent<NavMeshAgent>().baseOffset;
        return cr;
    }

    public Vector3 GetCenterOfRoom(GameObject room){
        Bounds rb = room.GetComponent<Collider>().bounds;
        Vector3 cr = rb.center;
        cr.y -= rb.extents.y;
        cr.y += agentPrefab.transform.localScale.y/2f;
        return cr;
    }

    List<GameObject> GetAllRooms(){
        return new List<GameObject>(GameObject.FindGameObjectsWithTag("room"));
    }    
}
