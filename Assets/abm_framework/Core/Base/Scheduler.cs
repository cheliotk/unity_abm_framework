/*
This code is licensed under MIT license, Copyright (c) 2019 Kostas Cheliotis
https://github.com/cheliotk/unity_abm_framework/blob/master/LICENSE
*/

using System.Collections.Generic;

namespace ABMU
{
    namespace Core
    {
        /// <summary>
        /// The Scheduler class manages the behaviour execution queue and handles the actual stepper execution calls.
        /// </summary>
        public class Scheduler
        {
            /// <summary>
            /// Dictionary structure for the scheduler queue,
            /// holds all steppers currently registered in the simulation
            /// that DO NOT run every tick
            /// Structure:
            /// key 1- StepperQOrder (EARLY, NORMAL, LATE)
            /// key 2-- StepperPriorityValue (0-1000)
            /// key 3--- step (0 = this frame, 1 = next frame, etc)
            /// key 4---- int index for List of Stepper objects for current frame
            /// val 5----- Stepper object
            /// </summary>
            Dictionary<Stepper.StepperQueueOrder, Dictionary<int, Dictionary<int, List<Stepper>>>> steppers;

            /// <summary>
            /// Dictionary structure for the scheduler queue for steppers running every tick,
            /// holds all steppers currently registered in the simulation that have a step = 1
            /// Structure:
            /// key 1- StepperQOrder (EARLY, NORMAL, LATE)
            /// key 2-- StepperPriorityValue (0-1000)
            /// key 3--- int index for List of Stepper objects for current frame
            /// val 4---- Stepper object
            /// </summary>
            public Dictionary<Stepper.StepperQueueOrder, Dictionary<int, List<Stepper>>> steppersEveryTick;

            /// <summary>
            /// List of steppers created during this frame
            /// </summary>
            List<Stepper> steppersCreatedThisFrame;

            /// <summary>
            /// List of steppers created during this frame that should also run this frame
            /// </summary>
            List<Stepper> steppersToRunThisFrame;

            /// <summary>
            /// List of steppers flagged to be destroyed during this queue
            /// </summary>
            List<Stepper> steppersDestroyedThisFrame;

            /// <summary>
            /// The current stepper priority value,
            /// loops through the entirety of existing priority values during the Tick() method
            /// and resets at the end of the frame.
            /// Is used to establish proper executino of steppers created this tick 
            /// </summary>
            int currentStepperP = -1;

            /// <summary>
            /// Constructor method for the Scheduler class
            /// </summary>
            public Scheduler(){
                Init();
            }
            
            /// <summary>
            /// Initializer method for this scheduler. Stepper Dict structure initialization
            /// </summary>
            void Init(){
                steppers = new Dictionary<Stepper.StepperQueueOrder, Dictionary<int, Dictionary<int, List<Stepper> >>>();
                steppersEveryTick = new Dictionary<Stepper.StepperQueueOrder, Dictionary<int, List<Stepper>>>();
                steppersCreatedThisFrame = new List<Stepper>();
                steppersDestroyedThisFrame = new List<Stepper>();
                steppersToRunThisFrame = new List<Stepper>();
            }

            /// <summary>
            /// Adds a stepper to the schedule
            /// </summary>
            /// <param name="s">The stepper to be added</param>
            public void RegisterStepper(Stepper s){
                steppersCreatedThisFrame.Add(s);
                if(s.priority > currentStepperP && s.startFrame == 0){
                    steppersToRunThisFrame.Add(s);
                }
            }
            
            /// <summary>
            /// Adds to the scheduler queue all steppers created this frame, but not yet registered
            /// </summary>
            void RegisterSteppersCreated(){
                foreach (Stepper s in steppersCreatedThisFrame)
                {
                    RegisterFutureStepper(s);
                }
            }

            /// <summary>
            /// Removes from the queue all steppers that have been destroyed this frame, but are still on the queue
            /// </summary>
            void DeregisterSteppersDestroyed(){
                foreach (Stepper s in steppersDestroyedThisFrame)
                {
                    DeregisterDestroyedStepper(s);
                }
            }

            /// <summary>
            /// Removes a given stepper from the scheduler queue
            /// </summary>
            /// <param name="s">The stepper to be removed</param>
            public void DeregisterStepper(Stepper s){
                steppersDestroyedThisFrame.Add(s);
            }

            /// <summary>
            /// Removes from the queue a stepper that has been destroyed, but not yet de-registered.
            /// </summary>
            /// <param name="s">The stepper to be removed from the queue</param>
            void DeregisterDestroyedStepper(Stepper s){
                if(s.step == 1){
                    steppersEveryTick[s.stepperQueue][s.priority].Remove(s);
                }
                else{
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
                
            }

            /// <summary>
            /// Registers a stepper to be run at a future timestep.
            /// This is used to re-register steppers that have just executed this frame, as well as steppers created during the scheduler tick.
            /// </summary>
            /// <param name="s">The stepper to be registered</param>
            void RegisterFutureStepper(Stepper s){
                if(s.step == 1){
                    RegisterFutureStepperWithStep_1(s);
                }
                else{
                    RegisterFutureStepperWithStepNot_1(s);
                }
            }

            /// <summary>
            /// Registers a stepper to be run at all future timesteps (for steppers with step = 1)
            /// </summary>
            /// <param name="s">The stepper to be registered</param>
            void RegisterFutureStepperWithStep_1(Stepper s){
                Dictionary<int, List<Stepper>> steppersEveryTick_pval;
                if(steppersEveryTick.TryGetValue(s.stepperQueue, out steppersEveryTick_pval)){
                    if(steppersEveryTick_pval.ContainsKey(s.priority)){
                        steppersEveryTick_pval[s.priority].Add(s);
                    }
                    else{
                        steppersEveryTick_pval.Add(s.priority, new List<Stepper>());
                        steppersEveryTick_pval[s.priority].Add(s);
                    }
                }
                else{
                    steppersEveryTick.Add(s.stepperQueue, new Dictionary<int, List<Stepper>>());
                    steppersEveryTick[s.stepperQueue].Add(s.priority, new List<Stepper>());
                    steppersEveryTick[s.stepperQueue][s.priority].Add(s);
                }
            }

            /// <summary>
            /// Registers a stepper to be run at future intervals (for steppers with step > 1)
            /// </summary>
            /// <param name="s">The stepper to be registered</param>
            void RegisterFutureStepperWithStepNot_1(Stepper s){
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

            /// <summary>
            /// Advances the scheduler stepper queue by one tick, moving all currently registered steppers one tick forward, closer to execution
            /// </summary>
            void AdvanceSchedulerTick(){
                Dictionary<Stepper.StepperQueueOrder, Dictionary<int, Dictionary<int, List<Stepper>>>> steppersTemp;
                steppersTemp = new Dictionary<Stepper.StepperQueueOrder, Dictionary<int, Dictionary<int, List<Stepper>>>>(steppers);
                foreach (var stepperQ in steppers.Keys)
                {
                    steppersTemp[stepperQ] = new Dictionary<int, Dictionary<int, List<Stepper>>>(steppers[stepperQ]);
                    foreach (int stepperP in steppers[stepperQ].Keys)
                    {
                        int stepper_S = -1;
                        steppersTemp[stepperQ][stepperP] = new Dictionary<int, List<Stepper>>(steppers[stepperQ][stepperP]);
                        foreach (int stepperS in steppers[stepperQ][stepperP].Keys)
                        {
                            if(steppersTemp[stepperQ][stepperP].ContainsKey(stepperS-1)){
                                steppersTemp[stepperQ][stepperP][stepperS-1] = steppers[stepperQ][stepperP][stepperS];
                            }
                            else{
                                steppersTemp[stepperQ][stepperP].Add(stepperS-1, steppers[stepperQ][stepperP][stepperS]);
                            }

                            stepper_S = stepperS;
                        }
                        steppersTemp[stepperQ][stepperP].Remove(stepper_S);
                    }
                }

                foreach (var stepperQ in steppersTemp.Keys)
                {
                    foreach (int stepperP in steppersTemp[stepperQ].Keys)
                    {
                        List<Stepper> steppersAt0 = new List<Stepper>();
                        steppers[stepperQ][stepperP] = new Dictionary<int, List<Stepper>>(steppersTemp[stepperQ][stepperP]);
                        
                        if(steppersTemp[stepperQ][stepperP].ContainsKey(-1)){
                            steppersAt0 = new List<Stepper>(steppersTemp[stepperQ][stepperP][-1]);
                            steppers[stepperQ][stepperP].Remove(-1);
                            foreach (Stepper s in steppersAt0)
                            {
                                RegisterFutureStepper(s);
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Executes all steppers registered for execution at this update tick.
            /// Stepper execution is performed synchronously for priority-sensitive steppers by filtering by stepperQueue order and stepper priority value,
            /// but is a-synchronous for contemporaneous steppers.
            /// i.e. for steppers with equal priority, all steppers on one agent are executed first before moving to the next agent.
            /// </summary>
            public void Tick(){
                RegisterSteppersCreated();
                DeregisterSteppersDestroyed();

                steppersCreatedThisFrame = new List<Stepper>();
                steppersToRunThisFrame = new List<Stepper>();
                steppersDestroyedThisFrame = new List<Stepper>();

                List<Stepper.StepperQueueOrder> stepperQValsUnique = GetListOfRegisteredStepperQValues();
                
                // Loop through all available stepper queue values in order (EARLY, NORMAL, LATE)
                foreach (var stepperQ in stepperQValsUnique)
                {
                    int prevStepperP = -1;

                    List<int> stepperPValsUnique = GetListOfRegisteredPriorityValuesAtQ(stepperQ);
                    
                    // Loop through all available stepper priority values in order for this stepper queue 
                    foreach (int stepperP in stepperPValsUnique)
                    {
                        currentStepperP = stepperP;
                        foreach (Stepper s in steppersToRunThisFrame.FindAll(a => a.priority > prevStepperP && a.priority < stepperP))
                        {
                            s.Step();
                        }
                        steppersToRunThisFrame = steppersToRunThisFrame.FindAll(a => a.priority >= stepperP);

                        if(steppersEveryTick.ContainsKey(stepperQ))
                        {
                            if(steppersEveryTick[stepperQ].ContainsKey(stepperP)){
                                foreach (Stepper s in steppersEveryTick[stepperQ][stepperP])
                                {
                                    if(!steppersDestroyedThisFrame.Contains(s))
                                        s.Step();
                                }
                            }
                        }

                        if(steppers.ContainsKey(stepperQ)){
                            if (steppers[stepperQ].ContainsKey(stepperP))
                            {
                                if(steppers[stepperQ][stepperP].ContainsKey(0)){

                                    foreach (Stepper s in steppers[stepperQ][stepperP][0])
                                    {
                                        if(!steppersDestroyedThisFrame.Contains(s))
                                            s.Step();
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < steppersToRunThisFrame.FindAll(a => a.priority == stepperP).Count; i++)
                        {
                            steppersToRunThisFrame[i].Step();
                        }
                        steppersToRunThisFrame = steppersToRunThisFrame.FindAll(a => a.priority != stepperP);
                        
                        prevStepperP = stepperP;
                    }
                    
                }
                steppersToRunThisFrame.Sort((a,b) => a.priority.CompareTo(b.priority));
                foreach (Stepper s in steppersToRunThisFrame)
                {
                    s.Step();
                }

                currentStepperP = -1;
                AdvanceSchedulerTick();
            }

            /// <summary>
            /// Get a list of all available stepper queue values currently in the simulation
            /// </summary>
            /// <returns>An ordered Stepper.StepperQueueOrder List of all available stepper queue values</returns>
            List<Stepper.StepperQueueOrder> GetListOfRegisteredStepperQValues(){
                List<Stepper.StepperQueueOrder> stepperQVals = new List<Stepper.StepperQueueOrder>(steppers.Keys);
                stepperQVals.AddRange(steppersEveryTick.Keys);
                HashSet<Stepper.StepperQueueOrder> stepperQValsUniqueTemp = new HashSet<Stepper.StepperQueueOrder>(stepperQVals);
                List<Stepper.StepperQueueOrder> stepperQValsUnique = new List<Stepper.StepperQueueOrder>(stepperQValsUniqueTemp);
                stepperQValsUnique.Sort((a,b) => a.CompareTo(b));

                return stepperQValsUnique;
            }

            /// <summary>
            /// Get a list of all available stepper priorities for a given stepper queue value
            /// </summary>
            /// <param name="stepperQ">The stepper queue value to check in</param>
            /// <returns>An ordered int List of all available stepper priority values</returns>
            List<int> GetListOfRegisteredPriorityValuesAtQ(Stepper.StepperQueueOrder stepperQ){
                List<int> stepperPVals = new List<int>();
                    
                if(steppers.ContainsKey(stepperQ))
                    stepperPVals.AddRange(steppers[stepperQ].Keys);
                
                if(steppersEveryTick.ContainsKey(stepperQ))
                    stepperPVals.AddRange(steppersEveryTick[stepperQ].Keys);

                HashSet<int> stepperPValsUniqueTemp = new HashSet<int>(stepperPVals);
                List<int> stepperPValsUnique = new List<int>(stepperPValsUniqueTemp);
                stepperPValsUnique.Sort((a,b) => a.CompareTo(b));

                return stepperPValsUnique;
            }
        }
    }
}