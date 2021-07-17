using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABMU.Core;
using ABMU;

public class SegregationAgent : AbstractAgent
{
    public SegregationController segController;
    public SegregationUtilities.agentType agentType;
    public List<SegregationAgent> neighbours;
    cellScript currentCell;
    public bool needToMove = false;

    public int likeNeighbours = 0;
    public int oppositeNeighbours = 0;
    public float likeNeighboursPerc = 0f;
    public float oppositeNeighboursPerc = 0f;
    
    public void Init(cellScript cell){
        base.Init();
        segController = GameObject.FindObjectOfType<SegregationController>();

        currentCell = cell;
        this.transform.position = cell.transform.position;
        currentCell.isOccupied = true;
        currentCell.occupier = this;

        this.transform.localScale = Vector3.one * segController.cellSizeInGameWorld;

        agentType = SegregationUtilities.agentType.RED;
        // if(Random.Range(0f,1.0f) < 0.5f){
        if(segController.rand.NextDouble() < 0.5f){
            agentType = SegregationUtilities.agentType.GREEN;
        }

        if(agentType == SegregationUtilities.agentType.RED){
            GetComponent<Renderer>().material.color = Color.red;
        }
        else{
            GetComponent<Renderer>().material.color = Color.green;
        }

        CreateStepper(FindNeighbours, 2, 100);
        CreateStepper(CheckNeighbourhoodIsOK, 2, 200);
        CreateStepper(Move, 2, 300, 1);
        CreateStepper(CalculateNeighbourhoodStats, 2, 400);
    }

    public void CalculateInitialStats() => CalculateNeighbourhoodStats();

    void FindNeighbours(){
        neighbours = new List<SegregationAgent>();

        foreach (var cell in currentCell.neighbours)
        {
            if(cell.occupier){
                neighbours.Add(cell.occupier);
            }
        }
    }

    void CheckNeighbourhoodIsOK(){
        needToMove = IsNeighbourhoodOK();

        Color c = GetComponent<Renderer>().material.color;
        
        if (needToMove){
            c.a = 1.0f;
        }
        else{
            c.a = 0.2f;
        }
        GetComponent<Renderer>().material.color = c;
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
            if(val >= segController.similarityThreshold){
                isOK = false;
            }
        }

        return isOK;
    }

    void Move(){
        if(needToMove){
            cellScript newCell = segController.FindFreeCell();
            
            currentCell.isOccupied = false;
            currentCell.occupier = null;

            this.transform.position = newCell.transform.position;

            currentCell = newCell;
            currentCell.isOccupied = true;
            currentCell.occupier = this;

            Color c = GetComponent<Renderer>().material.color;
            c.a = 0.6f;
            GetComponent<Renderer>().material.color = c;
        }
    }

    void CalculateNeighbourhoodStats()
    {
        FindNeighbours();
        likeNeighbours = 0;
        oppositeNeighbours = 0;
        foreach (var n in neighbours)
        {
            if(n.agentType == this.agentType)
            {
                likeNeighbours++;
            }
            else
            {
                oppositeNeighbours++;
            }
        }

        likeNeighboursPerc = (float)likeNeighbours / (float)neighbours.Count;
        oppositeNeighboursPerc = (float)oppositeNeighbours / (float)neighbours.Count;
    }
}

public class SegregationUtilities{
    public enum agentType
    {
        RED, GREEN
    }
}