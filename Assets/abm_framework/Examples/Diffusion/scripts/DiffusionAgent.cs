using UnityEngine;
using ABM.Core;

public class DiffusionAgent : AbstractAgent
{
    DiffusionController diffController;
    public float value = 0.5f;

    public LayerMask agentLm;

    public Collider[] neighbours;

    public void Init(Vector3 pos, float scale, float startingValue){
        base.Init();
        diffController = GameObject.FindObjectOfType<DiffusionController>();

        this.transform.position = pos;
        this.transform.localScale = Vector3.one * scale;
        value = startingValue;
        Draw();

        CreateStepper(1, FindNeighbours, 100);
        CreateStepper(1, CalculateNewValue, 200);
        CreateStepper(1, Draw, 300);

    }

    void FindNeighbours(){
        Vector3 s = this.transform.localScale;
        neighbours = Physics.OverlapBox(this.transform.position, new Vector3(s.x, 100f,s.z), Quaternion.Euler(Vector3.zero),agentLm);
    }

    void CalculateNewValue(){
        if (Random.Range(0f,1f) < 0.001f){
            value = Random.Range(0f,1f);
        }
        else{
            float valsum = 0f;
            foreach (Collider c in neighbours)
            {
                DiffusionAgent a = c.GetComponent<DiffusionAgent>();
                valsum += a.value;
            }

            valsum /= neighbours.Length;
            value = valsum;
        }        
    }

    void Draw(){
        Vector3 s = this.transform.localScale;
        Vector3 newScale = new Vector3(s.x, 30f * value, s.z);
        this.transform.localScale = newScale;
    }
}