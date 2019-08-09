namespace ABM
{
    namespace Core
    {
        using System;
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;

        public class Stepper: IComparable<Stepper>{
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
                if(Time.frameCount % _step == 0){
                    funcToCall();
                }
            }

            Del funcToCall;
            public delegate void Del();

            public Action myAction;

            public Stepper(int _stepValue, int _priorityValue, Del callback){
                step = _stepValue;
                funcToCall = callback;
                priority = _priorityValue;
            }
        }
    }
}