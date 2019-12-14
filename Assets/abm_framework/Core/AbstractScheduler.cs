using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ABM.Core;
using ABM;
public class AbstractScheduler : MonoBehaviour, ISchedulable, IInitializable
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
        // Dictionary<int, Stepper> steppers_step = new Dictionary<int, Stepper>();
        // Dictionary<int, Dictionary<int, Stepper>> steppers_pval = new Dictionary<int, Dictionary<int, Stepper>>();
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

    public void RegisterFutureStepper(Stepper s){
        Dictionary<int, Dictionary<int, List<Stepper>>> steppers_pval;
        if(steppers.TryGetValue(s.stepperQueue, out steppers_pval)){
            Dictionary<int, List<Stepper>> steppers_step;
            if(steppers_pval.TryGetValue(s.priority, out steppers_step)){
                if(steppers_step.ContainsKey(s.step)){
                    steppers_step[s.step].Add(s);
                }
                else{
                    steppers_step.Add(s.step, new List<Stepper>());
                    steppers_step[s.step].Add(s);
                }
            }
            else{
                steppers_pval.Add(s.priority, new Dictionary<int, List<Stepper>>());
                steppers_pval[s.priority].Add(s.step, new List<Stepper>());
                steppers_pval[s.priority][s.step].Add(s);
            }
        }
        else{
            steppers.Add(s.stepperQueue, new Dictionary<int, Dictionary<int, List<Stepper>>>());
            steppers[s.stepperQueue].Add(s.priority, new Dictionary<int, List<Stepper>>());
            steppers[s.stepperQueue][s.priority].Add(s.step, new List<Stepper>());
            steppers[s.stepperQueue][s.priority][s.step].Add(s);
        }
    }

    /*
    TODO: Something's buggered here:
        InvalidOperationException: Collection was modified; enumeration operation may not execute.
        System.Collections.Generic.Dictionary`2+KeyCollection+Enumerator[TKey,TValue].MoveNext () (at <ac823e2bb42b41bda67924a45a0173c3>:0)
        AbstractScheduler.AdvanceSchedulerTick () (at Assets/abm_framework/Core/AbstractScheduler.cs:86)
        AbstractScheduler.Tick () (at Assets/abm_framework/Core/AbstractScheduler.cs:122)
        ABM.Core.AbstractController.Step () (at Assets/abm_framework/Core/AbstractController.cs:72)
        ABM.Core.AbstractController.LateUpdate () (at Assets/abm_framework/Core/AbstractController.cs:141)
        UnityEngine.GUIUtility:ProcessEvent(Int32, IntPtr)
    */
    void AdvanceSchedulerTick(){
        var bob = steppers;
        foreach (var stepperQ in steppers.Keys)
        {
            foreach (int stepperP in steppers[stepperQ].Keys)
            {
                List<Stepper> steppersAt0 = new List<Stepper>();
                int stepper_S = -1;
                foreach (int stepperS in steppers[stepperQ][stepperP].Keys)
                {
                    if(stepperS == 0){
                        steppersAt0 = steppers[stepperQ][stepperP][stepperS];
                    }
                    else{
                        bob[stepperQ][stepperP][stepperS-1] = steppers[stepperQ][stepperP][stepperS]; //need to check if this copy is deep or shallow, if shallow may break everything
                                               
                    }
                    stepper_S = stepperS;
                }
                print(stepper_S);
                steppers[stepperQ][stepperP].Remove(stepper_S);
                foreach (Stepper s in steppersAt0)
                {
                    RegisterFutureStepper(s);
                }
            }
        }
    }

    public void Tick(){
        foreach (var stepperQ in steppers.Keys)
        {
            foreach (int stepperP in steppers[stepperQ].Keys)
            {
                foreach (int stepperS in steppers[stepperQ][stepperP].Keys)
                {
                    foreach (Stepper s in steppers[stepperQ][stepperP][stepperS])
                    {
                        s.Step();
                    }
                }
            }
        }

        AdvanceSchedulerTick();
    }
}
