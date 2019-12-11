namespace ABM
{
    namespace Core
    {
        using System.Collections.Generic;
        using UnityEngine;
        public class AbstractAgent : MonoBehaviour, ISteppable, IInitializable
        {
            List<Stepper> _steppers;
            public List<Stepper> steppers{
                get{
                    return _steppers;
                }
                private set{
                    _steppers = value;
                }
            }

            List<int> _steppersPriorityList;
            public List<int> steppersPriorityList{
                get{
                    return _steppersPriorityList;
                }
                private set{
                    _steppersPriorityList = value;
                }
            }

            AbstractController _controller;
            public AbstractController controller{
                get{
                    return _controller;
                }
            }

            public virtual void Awake() {
                _controller = GameObject.FindObjectOfType<AbstractController>();
                controller.RegisterAgent(this);
            }

            public virtual void Init(){
                _steppers = new List<Stepper>();
                _steppersPriorityList = new List<int>();
            }

            public virtual void Step(int priorityS = int.MinValue, int priorityE = int.MaxValue){
                
                var steppersFiltered = steppers.FindAll(s => s.priority >= priorityS && s.priority < priorityE);
                steppersFiltered.Sort();

                foreach (Stepper s in steppersFiltered)
                {
                    if(Time.frameCount % s.step == 0){
                        s.Step();
                    }
                }
            }

            public virtual void Step(){
                Step(int.MinValue, int.MaxValue);
            }

            public void RegisterStepper(Stepper s){
                _steppers.Add(s);
                _steppers.Sort();
                ResetSteppersPriorityList();
            }

            public void DeregisterStepper(Stepper s){
                _steppers.Remove(s);
                _steppers.Sort();
                ResetSteppersPriorityList();
            }

            public void CreateStepper(int _stepValue, Utilities.Del callback, int _priorityValue = 100){
                Stepper s = new Stepper(_stepValue, callback, _priorityValue);
                RegisterStepper(s);
            }

            public void DestroyStepper(Stepper s){
                DeregisterStepper(s);
                s = null;
            }

            void ResetSteppersPriorityList(){
                _steppersPriorityList = new List<int>();
                for (int i = 0; i < steppers.Count; i++)
                {
                    Stepper s = steppers[i];
                    if (!_steppersPriorityList.Contains(s.priority)){
                        _steppersPriorityList.Add(s.priority);
                    }
                }
            }

            public bool HasSteppersInPriorityRange(int priorityS, int priorityE){
                foreach (int p in steppersPriorityList)
                {
                    if(p >= priorityS && p < priorityE){
                        return true;
                    }
                }
                return false;
            }
        }
    }
}