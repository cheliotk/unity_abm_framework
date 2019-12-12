namespace ABM
{
    namespace Core
    {
        using System;
        using UnityEngine;
        public class Stepper: IComparable<Stepper>{
            Utilities.Del funcToCall;
            int _step;
            public int step{
                get
                {
                    return _step;
                }
                set{
                    _step = value;
                }
            }
            int _startFrame;
            public int startFrame{
                get{
                    return _startFrame;
                }
            }
            string _name;
            public string name{
                get{
                    return _name;
                }
            }

            Utilities.StepperQueueOrder _stepperQueue;

            public Utilities.StepperQueueOrder stepperQueue{
                get{
                    return _stepperQueue;
                }
                set{
                    _stepperQueue = value;
                }
            }

            int _priority;
            public int priority{
                get
                {
                    return _priority;
                }
                set{
                    _priority = value;
                }
            }

            // Default comparer for Stepper type, using priorities.
            public int CompareTo(Stepper otherStepper)
            {
                // A null value means that this object is greater.
                if (otherStepper == null)
                    return 1;
                    
                else
                    return this.priority.CompareTo(otherStepper.priority);
            }

            public virtual void Step(){
                    funcToCall();
            }
            
            public Stepper(int _stepValue, Utilities.Del callback, int _priorityValue = 500){
                step = _stepValue;
                funcToCall = callback;
                priority = _priorityValue;
                _name = callback.Method.Name;
                _startFrame = Time.frameCount;
                if(priority < 333){
                    stepperQueue = Utilities.StepperQueueOrder.EARLY;
                }
                else if (priority < 666){
                    stepperQueue = Utilities.StepperQueueOrder.NORMAL;
                }
                else{
                    stepperQueue = Utilities.StepperQueueOrder.LATE;
                }
            }
        }
    }
}