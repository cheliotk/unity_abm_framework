using UnityEngine;
using ABMU.Core;

public class simAgent : AbstractAgent
{
    public void Init(Vector3 _pos, Bounds _bounds, float _speed = 1.5f, float _angle = 90f, float _viewRange = 25f){
        base.Init();

        Vector3 pos = _pos;
        pos.y = 0f;
        this.transform.position = pos;

        speed = _speed;
        angle = _angle;
        viewRange = _viewRange;

        bounds = _bounds;

        CreateStepper(BehaviourQueue);
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

    void BehaviourQueue(){
        Move();
        CheckOutOfBounds();
        Look();
        Mark();
    }

    void Move(){
        this.transform.RotateAround(this.transform.position, Vector3.up, Random.Range(-angle/2f,angle/2f));

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
            foreach(BoxCollider c in agentsVisible){
                Debug.DrawLine(this.transform.position, c.transform.position);
            }
            this.transform.localScale = Vector3.one * agentsVisible.Length;
            GetComponent<BoxCollider>().size = Vector3.one/(float)agentsVisible.Length;
        }
        else{
            this.transform.localScale = Vector3.one;
            GetComponent<BoxCollider>().size = Vector3.one;
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