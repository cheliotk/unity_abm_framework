namespace ABM
{
    namespace Core
    {
        using System.Collections.Generic;
        using UnityEngine;
        using ABM.Core;
        using ABM;
        public class AbstractAgent : MonoBehaviour, ISteppable, IInitializable
        {
            List<Stepper> _steppers;
            public List<Stepper> steppers{
                get{
                    return _steppers;
                }
                set{
                    _steppers = value;
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
            }

            public virtual void Step(){
                foreach (Stepper s in _steppers)
                {
                    if(Time.frameCount % s.step == 0){
                        s.Step();
                    }
                }
            }

            public void RegisterStepper(Stepper s){
                _steppers.Add(s);
                _steppers.Sort();
            }
            public void CreateStepper(int _stepValue, Utilities.Del callback, int _priorityValue = 100){
                Stepper s = new Stepper(_stepValue, callback, _priorityValue);
                RegisterStepper(s);
            }
        }
    }
}