using UnityEngine;
using ABMU.Core;

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

        CreateStepper(FindNeighbours, 1, 100);
        CreateStepper(CalculateNewValue, 1, 200);
        CreateStepper(Draw, 1, 300);

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