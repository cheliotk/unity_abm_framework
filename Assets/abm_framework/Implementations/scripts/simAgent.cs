using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABM.Core;
using ABM;

// TODO: Find a proper way too instantiate abstract agent class with parameters. We need a proper constructor

public class simAgent : MonoBehaviour, IAgent
{
    // [SerializeField] int _step;
    // public int step{
    //     get
    //     {
    //         return _step;
    //     }
    //     set{
    //         _step = value;
    //     }
    // }

    List<Stepper> _steppers;
    public List<Stepper> steppers{
        get{
            return _steppers;
        }
        set{
            _steppers = value;
        }
    }

    void RegisterStepper(Stepper s){
        _steppers.Add(s);
        _steppers.Sort();
    }

    public void Step(){
        foreach (Stepper s in _steppers)
        {
            if(Time.frameCount % s.step == 0){
                s.Step();
            }
        }
        // Move();
        // Look();
        // Mark();
        // CheckOutOfBounds();
    }

    public void Init(object _pos = null){
        //TODO: figure out a way to find a generic type, i.e. IController, and not the implementation
        c = GameObject.FindObjectOfType<simController>();
        c.RegisterAgent(this);
        
        Vector3 pos = (Vector3)_pos;
        pos *= 15f;

        pos = Utilities.RandomPointInBounds(c.bounds);

        pos.y = 0f;
        this.transform.position = pos;

        _steppers = new List<Stepper>();

        MoveSt = new Stepper(3, 100, Move);
        LookSt = new Stepper(1, 200, Look);
        MarkSt = new Stepper(1, 300, Mark);
        CheckOOBSt = new Stepper(3, 400, CheckOutOfBounds);

        RegisterStepper(MoveSt);
        RegisterStepper(LookSt);
        RegisterStepper(MarkSt);
        RegisterStepper(CheckOOBSt);
    }

    /*
    Sim vars
     */
    simController c;
    [Range(1f,180f)]
    public float angle;
    [Range(1f,50f)]
    public float viewRange;
    [Range(1f,3f)]
    public float speed;
    public LayerMask agentLm;
    Collider[] agentsVisible;


    public bool isOutOfBounds = false;

    Stepper MoveSt;
    Stepper LookSt;
    Stepper MarkSt;
    Stepper CheckOOBSt;
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
        Bounds bounds = c.bounds;
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