/*
Boids - Flocking behavior simulation.

Copyright (C) 2014 Keijiro Takahashi

Edited by Kostas Cheliotis (2019)

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using UnityEditor;

using ABMU.Core;
using ABMU;

public class BoidController : AbstractController
{
    [Header("Sim Parameters")]
    public Bounds bounds;
    public int frameCount = 0;
    public int renderFrameCount = 0;

    public float avgDistCovered = 0f;

    [Header("Boid Parameters")]
    public GameObject boidPrefab;

    public int spawnCount = 10;

    public float spawnRadius = 4.0f;

    [Range(0.1f, 100.0f)]
    public float velocity = 6.0f;

    [Range(0.0f, 0.9f)]
    public float velocityVariation = 0.5f;

    [Range(0.1f, 20.0f)]
    public float rotationCoeff = 4.0f;

    [Range(0.1f, 50.0f)]
    public float neighborDist = 2.0f;

    public LayerMask searchLayer;
    
    public override void Init()
    {
        base.Init();

        frameCount = 0;

        for (var i = 0; i < spawnCount; i++) {
            Vector3 pos = Utilities.RandomPointInBounds(bounds);
            GameObject boid = Spawn();
            boid.GetComponent<BoidAgent>().Init(bounds, pos, Random.rotation);
        }
    }

    public override void Step(){
        frameCount ++;
        renderFrameCount = Time.frameCount;
        avgDistCovered = 0f;

        base.Step();

        avgDistCovered /= agents.Count;
    }

    [ExecuteInEditMode]
    private void OnDrawGizmos() {
        DrawBoundsBox();    
    }

    void DrawBoundsBox(){
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(bounds.center, bounds.extents * 2f);
    }

    public GameObject Spawn()
    {
        return Instantiate(boidPrefab);
    }

    public void AgentReportDistCovered(float d){
        avgDistCovered += d;
    }
}
