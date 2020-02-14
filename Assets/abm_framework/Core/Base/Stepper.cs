/*
This code is licensed under MIT license, Copyright (c) 2019 Kostas Cheliotis
https://github.com/cheliotk/unity_abm_framework/blob/master/LICENSE
*/

namespace ABMU
{
    namespace Core
    {
        using System;
        using UnityEngine;

        /// <summary>
        /// The Stepper class defines behaviours that can be added to the scheduler and executed at regular intervals.
        /// </summary>
        public class Stepper: IComparable<Stepper>{
            
            /// <summary>
            /// Definition of stepper queue enumerator, with three values: EARLY, NORMAL, LATE
            /// </summary>
            
            [Flags]
            public enum StepperQueueOrder
            {
                EARLY,
                NORMAL,
                LATE
            }
            
            /// <summary>
            /// Method delegate placeholder, stores the name of the function to be called when this stepper is executed.
            /// </summary>
            Utilities.Del funcToCall;

            /// <summary>
            /// The step frequency for this stepper, defines how often this stepper is executed (min = default = 1 is every frame, 2 is every other frame, etc)
            /// </summary>
            int _step;
            public int step{
                get
                {
                    return _step;
                }
            }

            /// <summary>
            /// Frame at which the stepper should be registered and added to the scheduler for consequent execution. To be used when a stepper needs to start execution at a later frame. (default = 0 means stepper is registered and executed during the frame it was created) 
            /// </summary>
            int _startFrame;
            public int startFrame{
                get{
                    return _startFrame;
                }
            }

            /// <summary>
            /// Steper name, is the same as the delegate method name
            /// </summary>
            string _name;
            public string name{
                get{
                    return _name;
                }
            }
            
            /// <summary>
            /// The queue slot this stepper should run in (EARLY, NORMAL, LATE)
            /// </summary>
            Stepper.StepperQueueOrder _stepperQueue;
            public Stepper.StepperQueueOrder stepperQueue{
                get{
                    return _stepperQueue;
                }
            }

            /// <summary>
            /// The priority value in the scheduler for this stepper (0-1000 range, lower is earlier)
            /// </summary>
            int _priority;
            public int priority{
                get
                {
                    return _priority;
                }
            }

            /// <summary>
            /// The Steppable this stepper belongs to
            /// </summary>
            Steppable owner;

            /// <summary>
            /// Default comparer for Stepper type, using priorities. Extends the Stepper class and allows sorting of Stepper IEnumerables 
            /// </summary>
            /// <param name="otherStepper"></param>
            /// <returns> \< 0: This precedes otherStepper, 0: Same position as otherStepper in order, \> 0: This follows otherStepper</returns>
            public int CompareTo(Stepper otherStepper)
            {
                // A null value means that this object is greater.
                if (otherStepper == null)
                    return 1;
                    
                else
                    return this.priority.CompareTo(otherStepper.priority);
            }

            /// <summary>
            /// Public method that runs the actual associated method
            /// </summary>
            public virtual void Step(){
                if(owner){
                    funcToCall();
                }
            }
            
            /// <summary>
            /// Constructor method for the Stepper class.
            /// </summary>
            /// <param name="_stepValue">Execution frequency for this stepper. Default (1) means every frame</param>
            /// <param name="callback">Name of method to be called. Should be provided as the method name, i.e. MethodName, not "MethodName" or MethodName()</param>
            /// <param name="_priorityValue">Where to place the stepper in the scheduler queue, using priority values (soft range 0-1000, lower is earlier)</param>
            /// <param name="_owner">The agent this stepper belongs to</param>
            public Stepper(int _stepValue,
                            Utilities.Del callback,
                            int _priorityValue,
                            Steppable _owner){
                _step = _stepValue;
                funcToCall = callback;
                _priority = _priorityValue;
                _name = callback.Method.Name;
                _startFrame = Time.frameCount;
                owner = _owner;
                if(priority < 333){
                    _stepperQueue = Stepper.StepperQueueOrder.EARLY;
                }
                else if (priority < 666){
                    _stepperQueue = Stepper.StepperQueueOrder.NORMAL;
                }
                else{
                    _stepperQueue = Stepper.StepperQueueOrder.LATE;
                }
            }

            /// <summary>
            /// Constructor method for the Stepper class.
            /// </summary>
            /// <param name="_stepValue">Execution frequency for this stepper. Default (1) means every frame</param>
            /// <param name="callback">Name of method to be called. Should be provided as the method name, i.e. MethodName, not "MethodName" or MethodName()</param>
            /// <param name="_stepperQueuePrompt">Where to place the stepper in the scheduler queue, using StepperQueueOrder values (EARLY, NORMAL, LATE)</param>
            /// <param name="_owner">The agent this stepper belongs to</param>
            public Stepper(int _stepValue,
                            Utilities.Del callback,
                            StepperQueueOrder _stepperQueuePrompt,
                            Steppable _owner){
                _step = _stepValue;
                funcToCall = callback;
                _stepperQueue = _stepperQueuePrompt;
                _name = callback.Method.Name;
                _startFrame = Time.frameCount;
                owner = _owner;
                switch (stepperQueue)
                {
                    case StepperQueueOrder.EARLY:
                        _priority = 166;
                        break;
                    case StepperQueueOrder.NORMAL:
                        _priority = 500;
                        break;
                    case StepperQueueOrder.LATE:
                        _priority = 833;
                        break;
                    default:
                        _priority = 500;
                        break;
                }
            }
        }
    }
}