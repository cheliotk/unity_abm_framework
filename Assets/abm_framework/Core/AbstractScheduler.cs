using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ABM.Core;
using ABM;
public class AbstractScheduler : ISchedulable, IInitializable
{
    /*
    Stepper Dictionary structure

    key- StepperQOrder (EARLY, NORMAL, LATE)
    key-- StepperPriorityValue (0-1000)
    key--- step (0 = this frame, 1 = next frame, etc)
    val---- List of Stepper objects
    */
    Dictionary<Stepper.StepperQueueOrder, Dictionary<int, Dictionary<int, List<Stepper>>>> steppers;
    public virtual void Init(){
        steppers = new Dictionary<Stepper.StepperQueueOrder, Dictionary<int, Dictionary<int, List<Stepper> >>>();
    }

    public void RegisterStepper(Stepper s){
        Dictionary<int, Dictionary<int, List<Stepper>>> steppers_pval;
        if(steppers.TryGetValue(s.stepperQueue, out steppers_pval)){
            Dictionary<int, List<Stepper>> steppers_step;
            if(steppers_pval.TryGetValue(s.priority, out steppers_step)){
                if(steppers_step.ContainsKey(0)){
                    steppers_step[0].Add(s);
                }
                else{
                    steppers_step.Add(0, new List<Stepper>());
                    steppers_step[0].Add(s);
                }
            }
            else{
                steppers_pval.Add(s.priority, new Dictionary<int, List<Stepper>>());
                steppers_pval[s.priority].Add(0, new List<Stepper>());
                steppers_pval[s.priority][0].Add(s);
            }
        }
        else{
            steppers.Add(s.stepperQueue, new Dictionary<int, Dictionary<int, List<Stepper>>>());
            steppers[s.stepperQueue].Add(s.priority, new Dictionary<int, List<Stepper>>());
            steppers[s.stepperQueue][s.priority].Add(0, new List<Stepper>());
            steppers[s.stepperQueue][s.priority][0].Add(s);
        }
    }

    public void DeregisterStepper(Stepper s){
        Dictionary<int, List<Stepper>> steppersTemp = new Dictionary<int, List<Stepper>>(steppers[s.stepperQueue][s.priority]);
        foreach (int k in steppers[s.stepperQueue][s.priority].Keys)
        {
            if(steppers[s.stepperQueue][s.priority][k].Contains(s)){
                steppersTemp[k].Remove(s);                
                break;
            }
        }
        steppers[s.stepperQueue][s.priority] = new Dictionary<int, List<Stepper>>(steppersTemp);
    }

    public void RegisterFutureStepper(Stepper s){
        Dictionary<int, Dictionary<int, List<Stepper>>> steppers_pval;
        if(steppers.TryGetValue(s.stepperQueue, out steppers_pval)){
            Dictionary<int, List<Stepper>> steppers_step;
            if(steppers_pval.TryGetValue(s.priority, out steppers_step)){
                if(steppers_step.ContainsKey(s.step-1)){
                    steppers_step[s.step-1].Add(s);
                }
                else{
                    steppers_step.Add(s.step-1, new List<Stepper>());
                    steppers_step[s.step-1].Add(s);
                }
            }
            else{
                steppers_pval.Add(s.priority, new Dictionary<int, List<Stepper>>());
                steppers_pval[s.priority].Add(s.step-1, new List<Stepper>());
                steppers_pval[s.priority][s.step-1].Add(s);
            }
        }
        else{
            steppers.Add(s.stepperQueue, new Dictionary<int, Dictionary<int, List<Stepper>>>());
            steppers[s.stepperQueue].Add(s.priority, new Dictionary<int, List<Stepper>>());
            steppers[s.stepperQueue][s.priority].Add(s.step-1, new List<Stepper>());
            steppers[s.stepperQueue][s.priority][s.step-1].Add(s);
        }
    }

    void AdvanceSchedulerTick(){
        Dictionary<Stepper.StepperQueueOrder, Dictionary<int, Dictionary<int, List<Stepper>>>> bob;
        bob = new Dictionary<Stepper.StepperQueueOrder, Dictionary<int, Dictionary<int, List<Stepper>>>>(steppers);
        foreach (var stepperQ in steppers.Keys)
        {
            bob[stepperQ] = new Dictionary<int, Dictionary<int, List<Stepper>>>(steppers[stepperQ]);
            foreach (int stepperP in steppers[stepperQ].Keys)
            {
                int stepper_S = -1;
                bob[stepperQ][stepperP] = new Dictionary<int, List<Stepper>>(steppers[stepperQ][stepperP]);
                foreach (int stepperS in steppers[stepperQ][stepperP].Keys)
                {
                    if(bob[stepperQ][stepperP].ContainsKey(stepperS-1)){
                        bob[stepperQ][stepperP][stepperS-1] = steppers[stepperQ][stepperP][stepperS];
                    }
                    else{
                        bob[stepperQ][stepperP].Add(stepperS-1, steppers[stepperQ][stepperP][stepperS]);
                    }

                    stepper_S = stepperS;
                }
                bob[stepperQ][stepperP].Remove(stepper_S);
            }
        }

        foreach (var stepperQ in bob.Keys)
        {
            foreach (int stepperP in bob[stepperQ].Keys)
            {
                List<Stepper> steppersAt0 = new List<Stepper>();
                steppers[stepperQ][stepperP] = new Dictionary<int, List<Stepper>>(bob[stepperQ][stepperP]);
                
                if(bob[stepperQ][stepperP].ContainsKey(-1)){
                    steppersAt0 = new List<Stepper>(bob[stepperQ][stepperP][-1]);
                    steppers[stepperQ][stepperP].Remove(-1);
                    foreach (Stepper s in steppersAt0)
                    {
                        RegisterFutureStepper(s);
                    }
                }
            }
        }
    }

    public void Tick(){
        foreach (var stepperQ in steppers.Keys)
        {
            foreach (int stepperP in steppers[stepperQ].Keys)
            {
                if(steppers[stepperQ][stepperP].ContainsKey(0)){

                    foreach (Stepper s in steppers[stepperQ][stepperP][0])
                    {
                        s.Step();
                    }
                }
                
            }
        }

        AdvanceSchedulerTick();
    }
}
