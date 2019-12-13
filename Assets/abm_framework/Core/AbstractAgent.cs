namespace ABM
{
    namespace Core
    {
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;

        /// <summary>
        /// Abstract agent class. To be used as a blueprint for specific simulation agents. A simulation-specific agent class should inherit from this class
        /// </summary>
        public class AbstractAgent : MonoBehaviour, ISteppable, IInitializable
        {

            /// <summary>
            /// List of all steppers on this agent
            /// </summary>
            List<Stepper> _steppers;
            public List<Stepper> steppers{
                get{
                    return _steppers;
                }
            }

            /// <summary>
            /// List of stepper priority values for all steppers on this agent
            /// </summary>
            List<int> _steppersPriorityList;
            public List<int> steppersPriorityList{
                get{
                    return _steppersPriorityList;
                }
            }

            /// <summary>
            /// List of stepperQueue slots of all steppers on this agent
            /// </summary>
            List<Stepper.StepperQueueOrder> _steppersQueueList;
            public List<Stepper.StepperQueueOrder> steppersQueueList{
                get{
                    return _steppersQueueList;
                }
            }

            /// <summary>
            /// Reference to the simulation controller
            /// </summary>
            AbstractController _controller;
            public AbstractController controller{
                get{
                    return _controller;
                }
            }

            /// <summary>
            /// Agent awake method. Is called automatically by UnityEngine when the agent GameObject is instantiated and registers it with the controller
            /// </summary>
            public virtual void Awake() {
                _controller = GameObject.FindObjectOfType<AbstractController>();
                controller.RegisterAgent(this);
            }

            /// <summary>
            /// Agent initializer method.
            /// </summary>
            public virtual void Init(){
                _steppers = new List<Stepper>();
                _steppersPriorityList = new List<int>();
            }

            /// <summary>
            /// Executes all steppers on this agent
            /// </summary>
            public virtual void Step(){
                Step(int.MinValue, int.MaxValue);
            }

            /// <summary>
            /// Filters the agent's steppers by stepper priority value range and proceeds with execution of valid steppers 
            /// </summary>
            /// <param name="priorityS">The lower bound for the stepper priority range (inclusive)</param>
            /// <param name="priorityE">The upper bound for the stepper priority range (exclusive)</param>
            public virtual void Step(int priorityS = int.MinValue, int priorityE = int.MaxValue){
                var steppersFiltered = steppers.FindAll(s => s.priority >= priorityS && s.priority < priorityE);
                StepSteppers(steppersFiltered);
            }

            /// <summary>
            /// Filters the agent's steppers by stepperQueue slot and proceeds with execution of valid steppers 
            /// </summary>
            /// <param name="_stepperQueuePrompt">The stepperQueue slot (EARLY, NORMAL, LATE) for which to execute steppers</param>
            public virtual void Step(Stepper.StepperQueueOrder _stepperQueuePrompt){
                var steppersFiltered = steppers.FindAll(s => s.stepperQueue == _stepperQueuePrompt);
                StepSteppers(steppersFiltered);
            }

            /// <summary>
            /// Executes all provided steppers, if their frequency is execution frequency is valid for this frame
            /// </summary>
            /// <param name="steppers">List of steppers to execute</param>
            void StepSteppers(List<Stepper> steppers){
                steppers.Sort();

                foreach (Stepper s in steppers)
                {
                    if((s.startFrame + Time.frameCount) % s.step == 0 || s.startFrame == Time.frameCount){
                        s.Step();
                    }
                }
            }

            /// <summary>
            /// Adds a stepper to the steppers list
            /// </summary>
            /// <param name="s">Stepper to be added</param>
            void RegisterStepper(Stepper s){
                _steppers.Add(s);
                _steppers.Sort();
                ResetSteppersPriorityList();
            }

            /// <summary>
            /// Removes a stepper from the steppers List
            /// </summary>
            /// <param name="s">Stepper to be removed</param>
            void DeregisterStepper(Stepper s){
                _steppers.Remove(s);
                _steppers.Sort();
                ResetSteppersPriorityList();
            }

            /// <summary>
            /// Creates a stepper
            /// </summary>
            /// <param name="_stepValue">The stepper execution frequency, in positive integer values (min = default = 1)</param>
            /// <param name="callback">Name of method to be called. Should be provided as the method name, i.e. MethodName, not "MethodName" or MethodName()</param>
            public void CreateStepper(int _stepValue,
                            Utilities.Del callback){
                CreateStepper(_stepValue, callback, 500);
            }

            /// <summary>
            /// Creates a stepper
            /// </summary>
            /// <param name="_stepValue">The stepper execution frequency, in positive integer values (min = default = 1)</param>
            /// <param name="callback">Name of method to be called. Should be provided as the method name, i.e. MethodName, not "MethodName" or MethodName()</param>
            /// <param name="_priorityValue">Where to place the stepper in the scheduler queue, using priority values (soft range 0-1000, lower is earlier)</param>
            /// <param name="_delayStartByFrames">The number of frames to delay stepper registration by (default = 0)</param>
            public void CreateStepper(int _stepValue,
                            Utilities.Del callback,
                            int _priorityValue = 500,
                            int _delayStartByFrames = 0){
                if(_delayStartByFrames == 0){
                    Stepper s = new Stepper(_stepValue, callback, _priorityValue);
                    RegisterStepper(s);
                }
                else{
                    StartCoroutine(CreateStepperAfterFrames(_delayStartByFrames, _stepValue, callback, _priorityValue));
                }
            }

            /// <summary>
            /// Creates a stepper
            /// </summary>
            /// <param name="_stepValue">The stepper execution frequency, in positive integer values (min = default = 1)</param>
            /// <param name="callback">Name of method to be called. Should be provided as the method name, i.e. MethodName, not "MethodName" or MethodName()</param>
            /// <param name="_stepperQueuePrompt">Where to place the stepper in the scheduler queue, using StepperQueueOrder values (EARLY, NORMAL, LATE) (default = NORMAL)</param>
            /// <param name="_delayStartByFrames">The number of frames to delay stepper registration by (default = 0)</param>
            public void CreateStepper(int _stepValue,
                            Utilities.Del callback,
                            Stepper.StepperQueueOrder _stepperQueuePrompt = Stepper.StepperQueueOrder.NORMAL,
                            int _delayStartByFrames = 0){
                if(_delayStartByFrames == 0){
                    Stepper s = new Stepper(_stepValue, callback, _stepperQueuePrompt);
                    RegisterStepper(s);
                }
                else{
                    StartCoroutine(CreateStepperAfterFrames(_delayStartByFrames, _stepValue, callback, _stepperQueuePrompt));
                }
            }

            /// <summary>
            /// Delayed registration of stepper
            /// </summary>
            /// <param name="_delayStartByFrames">The number of frames to delay stepper registration by</param>
            /// <param name="_stepValue">The stepper step value, to be used for the actual stepper registration</param>
            /// <param name="callback">Name of the stepper callback method</param>
            /// <param name="_priorityValue">The stepper priority value, to be used for the actual stepper registration</param>
            /// <returns>null, standard Coroutine return value</returns>
            IEnumerator CreateStepperAfterFrames(int _delayStartByFrames,
                            int _stepValue,
                            Utilities.Del callback,
                            int _priorityValue){
                int frameToRegisterOn = Time.frameCount + _delayStartByFrames;
                while(Time.frameCount < frameToRegisterOn){
                    yield return null;
                }
                Stepper s = new Stepper(_stepValue, callback, _priorityValue);
                RegisterStepper(s);
            }

            /// <summary>
            /// Delayed registration of stepper
            /// </summary>
            /// <param name="_delayStartByFrames">The number of frames to delay stepper registration by</param>
            /// <param name="_stepValue">The stepper step value, to be used for the actual stepper registration</param>
            /// <param name="callback">Name of the stepper callback method</param>
            /// <param name="_stepperQueuePrompt">The stepper stepperQueue slot, to be used for the actual stepper registration</param>
            /// <returns>null, standard Coroutine return value</returns>
            IEnumerator CreateStepperAfterFrames(int _delayStartByFrames,
                            int _stepValue,
                            Utilities.Del callback, 
                            Stepper.StepperQueueOrder _stepperQueuePrompt){
                int frameToRegisterOn = Time.frameCount + _delayStartByFrames;
                while(Time.frameCount < frameToRegisterOn){
                    yield return null;
                }
                Stepper s = new Stepper(_stepValue, callback, _stepperQueuePrompt);
                RegisterStepper(s);
            }

            /// <summary>
            /// Deletes a stepper
            /// </summary>
            /// <param name="s">The stepper to be removed</param>
            public void DestroyStepper(Stepper s){
                DeregisterStepper(s);
                s = null;
            }

            /// <summary>
            /// Populates and sorts the steppers priority and queue lists, for all currently registered steppers.
            /// </summary>
            void ResetSteppersPriorityList(){
                _steppersPriorityList = new List<int>();
                _steppersQueueList = new List<Stepper.StepperQueueOrder>();
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
                _steppersPriorityList.Sort();
            }

            /// <summary>
            /// Checks whether this agent has any steppers registered within a particular stepper priority value range (range should be within 0-1000, lower is earlier)
            /// </summary>
            /// <param name="priorityS">The lower bound for the stepper priority range (inclusive)</param>
            /// <param name="priorityE">The upper bound for the stepper priority range (exclusive)</param>
            /// <returns>True if steppers exist within the given range, otherwise false</returns>
            public bool HasSteppersInPriorityRange(int priorityS, int priorityE){
                foreach (int p in steppersPriorityList)
                {
                    if(p >= priorityS && p < priorityE){
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Checks whether this agent has any steppers registered for a particular stepperQueue slot.
            /// </summary>
            /// <param name="_stepperQueuePrompt">The stepperQueue slot to check for</param>
            /// <returns>True if steppers exist for the stepperQueue slot, otherwise false</returns>
            public bool HasSteppersInQueue(Stepper.StepperQueueOrder _stepperQueuePrompt){
                foreach (var stepperQ in steppersQueueList)
                {
                    if(stepperQ == _stepperQueuePrompt){
                        return true;
                    }
                }
                return false;
            }
        }
    }
}