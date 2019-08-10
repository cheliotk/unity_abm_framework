using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABM.Core;

public class simAgent : AbstractAgent
{
    public void Init(Vector3 _pos, Bounds _bounds, float _speed = 1.5f){
        base.Init();

        Vector3 pos = _pos;
        pos.y = 0f;
        this.transform.position = pos;

        speed = _speed;
        bounds = _bounds;

        CreateStepper(3, Move, 100);
        CreateStepper(1, Look, 200);
        CreateStepper(1, Mark, 300);
        CreateStepper(3, CheckOutOfBounds, 400);
    }

    [Range(1f,180f)]
    public float angle;
    [Range(1f,50f)]
    public float viewRange;
    [Range(1f,3f)]
    public float speed;
    public LayerMask agentLm;
    Collider[] agentsVisible;
    Bounds bounds;
    public bool isOutOfBounds = false;

    void Move(){
        this.transform.RotateAround(this.transform.position, Vector3.up, Random.Range(-angle,angle));

        float _speed = speed;
        if(agentsVisible != null){
            _speed /= (agentsVisible.Length + 1);
        }
        this.transform.position += this.transform.forward * _speed;
    }

    void Look(){
        agentsVisible = Physics.OverlapSphere(this.transform.position, viewRange, agentLm);
    }

    void Mark(){
        if(agentsVisible != null){
            foreach(Collider c in agentsVisible){
                Debug.DrawLine(this.transform.position, c.transform.position);
            }
        }
    }

    void CheckOutOfBounds(){
        Vector3 pos = this.transform.position;
        if(!bounds.Contains(this.transform.position)){
            this.transform.RotateAround(this.transform.position, Vector3.up, 180f);
            this.transform.position += this.transform.forward * speed;

            isOutOfBounds = true;
        }
        else{
            isOutOfBounds = false;
        }
    }
}