namespace ABM
{
    namespace Core
    {
        using System.Collections;
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

            List<Utilities.StepperQueueOrder> _steppersQueueList;
            public List<Utilities.StepperQueueOrder> steppersQueueList{
                get{
                    return _steppersQueueList;
                }
                private set{
                    _steppersQueueList = value;
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

            public virtual void Step(){
                Step(int.MinValue, int.MaxValue);
            }

            public virtual void Step(int priorityS = int.MinValue, int priorityE = int.MaxValue){
                var steppersFiltered = steppers.FindAll(s => s.priority >= priorityS && s.priority < priorityE);
                StepSteppers(steppersFiltered);
            }

            public virtual void Step(Utilities.StepperQueueOrder stepperQueuePrompt){
                var steppersFiltered = steppers.FindAll(s => s.stepperQueue == stepperQueuePrompt);
                StepSteppers(steppersFiltered);
            }

            void StepSteppers(List<Stepper> steppers){
                steppers.Sort();

                foreach (Stepper s in steppers)
                {
                    if((s.startFrame + Time.frameCount) % s.step == 0 || s.startFrame == Time.frameCount){
                        s.Step();
                    }
                }
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

            public void CreateStepper(int _stepValue, Utilities.Del callback, int _priorityValue = 100, int _delayFrames = 0){
                if(_delayFrames == 0){
                    Stepper s = new Stepper(_stepValue, callback, _priorityValue);
                    RegisterStepper(s);
                }
                else{
                    StartCoroutine(CreateStepperAfterFrames(_delayFrames,_stepValue, callback, _priorityValue));
                }
            }

            IEnumerator CreateStepperAfterFrames(int _delayFrames, int _stepValue, Utilities.Del callback, int _priorityValue){
                int frameToRegisterOn = Time.frameCount + _delayFrames;
                while(Time.frameCount < frameToRegisterOn){
                    yield return null;
                }
                Stepper s = new Stepper(_stepValue, callback, _priorityValue);
                RegisterStepper(s);
            }

            public void DestroyStepper(Stepper s){
                DeregisterStepper(s);
                s = null;
            }

            void ResetSteppersPriorityList(){
                _steppersPriorityList = new List<int>();
                _steppersQueueList = new List<Utilities.StepperQueueOrder>();
                for (int i = 0; i < steppers.Count; i++)
                {
                    Stepper s = steppers[i];
                    if (!_steppersPriorityList.Contains(s.priority)){
                        _steppersPriorityList.Add(s.priority);
                    }

                    if(!_steppersQueueList.Contains(s.stepperQueue)){
                        _steppersQueueList.Add(s.stepperQueue);
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

            public bool HasSteppersInQueue(Utilities.StepperQueueOrder stepperQueuePrompt){
                foreach (var stepperQ in steppersQueueList)
                {
                    if(stepperQ == stepperQueuePrompt){
                        return true;
                    }
                }
                return false;
            }
        }
    }
}