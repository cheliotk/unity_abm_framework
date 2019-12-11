using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABM.Core;
using ABM;

public class SegregationAgent : AbstractAgent
{
    public SegregationController segController;
    public SegregationUtilities.agentType agentType;
    public List<SegregationAgent> neighbours;
    cellScript currentCell;
    bool needToMove = false;
    public void Init(cellScript cell){
        base.Init();
        segController = GameObject.FindObjectOfType<SegregationController>();

        currentCell = cell;
        this.transform.position = cell.transform.position;
        currentCell.isOccupied = true;

        this.transform.localScale = Vector3.one * segController.cellSizeInGameWorld;

        agentType = SegregationUtilities.agentType.RED;
        if(Random.Range(0f,1.0f) < 0.5f){
            agentType = SegregationUtilities.agentType.GREEN;
        }

        if(agentType == SegregationUtilities.agentType.RED){
            GetComponent<Renderer>().material.color = Color.red;
        }
        else{
            GetComponent<Renderer>().material.color = Color.green;
        }

        neighbours = new List<SegregationAgent>();

        CreateStepper(1, FindNeighbours, 100);
        CreateStepper(1, CheckNeighbourhoodIsOK, 200);
        CreateStepper(1, Move, 300);
    }

    void FindNeighbours(){
        Collider[] others = Physics.OverlapBox(this.transform.transform.position, Vector3.one*segController.cellSizeInGameWorld,Quaternion.identity,segController.agentLm);
        
        neighbours = new List<SegregationAgent>();
        
        foreach (Collider other in others)
        {
            if(other.gameObject != this.gameObject){
                neighbours.Add(other.GetComponent<SegregationAgent>());
            }
        }
    }

    void CheckNeighbourhoodIsOK(){
        needToMove = IsNeighbourhoodOK();
    }

    bool IsNeighbourhoodOK(){
        bool isOK = true;
        
        if(neighbours.Count == 0){

        }
        else{
            int counter = 0;
            foreach (var n in neighbours)
            {
                if(n.agentType == this.agentType){
                    counter++;
                }
            }
            float val = (float)counter/neighbours.Count;
            if(val > segController.similarityThreshold){
                isOK = false;
            }
        }

        return isOK;
    }

    void Move(){
        Color c = GetComponent<Renderer>().material.color;
        if(needToMove){
            cellScript newCell = segController.FindFreeCell();
            currentCell.isOccupied = false;
            this.transform.position = newCell.transform.position;
            currentCell = newCell;
            currentCell.isOccupied = true;
            c.a = 1.0f;
        }
        else{
            c.a = 0.2f;
        }

        GetComponent<Renderer>().material.color = c;
    }
}

public class SegregationUtilities{
    public enum agentType
    {
        RED, GREEN
    }
}