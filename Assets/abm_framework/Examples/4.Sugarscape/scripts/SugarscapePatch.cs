using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABMU.Core;
public class SugarscapePatch : AbstractAgent
{
    int maxSugar;
    public int pSugar {get; private set;}
    Renderer renderer;
    bool isOccupied = false;
    SugarscapeAgent tenant;

    public void Init(int sugarAmount){
        base.Init();
        renderer = this.GetComponent<Renderer>();
        
        maxSugar = sugarAmount;
        pSugar = maxSugar;
        SetColor();

        CreateStepper(Growback, 1, 200);
        CreateStepper(SetColor, 1, 201);
    }

    void SetColor(){
        float v = (float)pSugar/4f;
        Color c = Color.Lerp(Color.white, Color.yellow, v);
        if(maxSugar == 0){
            c = Color.white;
        }
        renderer.material.color = c;
        Vector3 pos = this.transform.position;
        pos.y = v/2f;
        this.transform.position = pos;
    }

    void Growback(){
        pSugar++;

        if(pSugar > maxSugar){
            pSugar = maxSugar;
            DestroyStepper("Growback");
            DestroyStepper("SetColor");
        }
    }

    public int HarvestSugar(){
        int v = pSugar;
        pSugar = 0;
        
        if(base.steppers.Count == 0){
            CreateStepper(Growback, 1, 200);
            CreateStepper(SetColor, 1, 201);
        }
        return v;
    }

    public bool IsOccupied(){
        return isOccupied;
    }

    public void Occupy(SugarscapeAgent a){
        tenant = a;
        if(tenant){
            isOccupied = true;
        }
        else{
            isOccupied = false;
        }
    }

}
