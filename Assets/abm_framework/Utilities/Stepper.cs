namespace ABM
{
    namespace Core
    {
        using System;
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
                    funcToCall();
            }

            Utilities.Del funcToCall;
            
            public Stepper(int _stepValue, Utilities.Del callback, int _priorityValue = 100){
                step = _stepValue;
                funcToCall = callback;
                priority = _priorityValue;
            }
        }
    }
}