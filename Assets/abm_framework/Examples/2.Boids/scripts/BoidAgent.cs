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

public class BoidAgent : AbstractAgent
{
    // Reference to the boidController.
    public BoidController boidController;

    // Random seed.
    float noiseOffset;
    float velocity;

    Bounds bounds;

    Collider[] nearbyBoids;

    public float distCovered = 0f;

    float norm = 0.01f;

    public void Init(Bounds _bounds, Vector3 _pos, Quaternion _rot)
    {
        base.Init();
        boidController = GameObject.FindObjectOfType<BoidController>();
        noiseOffset = Random.value * 10.0f;
        bounds = _bounds;

        this.transform.position = _pos;
        this.transform.rotation = _rot;

        CreateStepper(BoidBehaviourFindNeighbours, 1, Stepper.StepperQueueOrder.EARLY);
        CreateStepper(BoidBehaviourMove, 1, Stepper.StepperQueueOrder.NORMAL);
        CreateStepper(CheckOutOfBounds, 1, Stepper.StepperQueueOrder.LATE);
    }

    void BoidBehaviourFindNeighbours(){
        // Looks up nearby boids.
        nearbyBoids = Physics.OverlapSphere(this.transform.position, boidController.neighborDist, boidController.searchLayer);
        if(Selection.Contains(this.gameObject)){
            for (int i = 0; i < nearbyBoids.Length; i++)
            {
                Vector3 p = nearbyBoids[i].transform.position;
                Debug.DrawLine(this.transform.position, p, Color.cyan);
            }
        }
    }

    void BoidBehaviourMove(){
        var currentPosition = transform.position;
        var currentRotation = transform.rotation;

        // Current velocity randomized with noise.
        var noise = Mathf.PerlinNoise(Time.time, noiseOffset) * 2.0f - 1.0f;
        velocity = boidController.velocity * (1.0f + noise * boidController.velocityVariation);

        // Initializes the vectors.
        var separation = Vector3.zero;
        var alignment = this.transform.forward;
        var cohesion = this.transform.position;

        // Accumulates the vectors.
        foreach (var boid in nearbyBoids)
        {
            if (boid.gameObject == gameObject) continue;
            var t = boid.transform;
            separation += GetSeparationVector(t);
            alignment += t.forward;
            cohesion += t.position;
        }

        var avg = 1.0f / nearbyBoids.Length;
        alignment *= avg;
        cohesion *= avg;
        cohesion = (cohesion - currentPosition).normalized;

        // Calculates a rotation from the vectors.
        var direction = separation + alignment + cohesion;
        var rotation = Quaternion.FromToRotation(Vector3.forward, direction.normalized);

        // Applys the rotation with interpolation.
        if (rotation != currentRotation)
        {
            // var ip = Mathf.Exp(-boidController.rotationCoeff * Time.deltaTime);
            var ip = Mathf.Exp(-boidController.rotationCoeff * norm);
            transform.rotation = Quaternion.Slerp(rotation, currentRotation, ip);
        }

        Vector3 prevPos = this.transform.position;
        // Moves forawrd.
        // transform.position = currentPosition + transform.forward * (velocity * Time.deltaTime);
        transform.position = currentPosition + transform.forward * (velocity * norm);
        distCovered += Vector3.Distance(this.transform.position,prevPos);
        boidController.AgentReportDistCovered(distCovered);
    }

    // Caluculates the separation vector with a target.
    Vector3 GetSeparationVector(Transform target)
    {
        var diff = transform.position - target.transform.position;
        var diffLen = diff.magnitude;
        var scaler = Mathf.Clamp01(1.0f - diffLen / boidController.neighborDist);
        return diff * (scaler / diffLen);
    }

    void CheckOutOfBounds(){
        Vector3 pos = this.transform.position;
        if(!bounds.Contains(this.transform.position)){
            Vector3 fromCenterToHere = this.transform.position - bounds.center;
            this.transform.position = bounds.center - fromCenterToHere;
            // transform.position = this.transform.position + transform.forward * (velocity * Time.deltaTime);
            transform.position = this.transform.position + transform.forward * (velocity * norm);
        }
    }
}
