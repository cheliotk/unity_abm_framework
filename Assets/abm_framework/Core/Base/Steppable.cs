/*
This code is licensed under MIT license, Copyright (c) 2019 Kostas Cheliotis
https://github.com/cheliotk/unity_abm_framework/blob/master/LICENSE
*/

namespace ABMU
{
    namespace Core
    {
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;

        /// <summary>
        /// Steppable class. Any classes that want to implement a stepper should inherit from this class 
        /// </summary>
        public class Steppable : MonoBehaviour, IInitializable
        {

            /// <summary>
            /// List of all steppers on this steppable
            /// </summary>
            List<Stepper> _steppers;
            public List<Stepper> steppers{
                get{
                    return _steppers;
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
            /// Steppable Awake method. Is called automatically by UnityEngine when the steppable GameObject is instantiated
            /// </summary>
            public virtual void Awake() {
                if(controller == null){
                    _controller = GameObject.FindObjectOfType<AbstractController>();
                }
            }

            /// <summary>
            /// steppable initializer method.
            /// </summary>
            public virtual void Init(){
                _steppers = new List<Stepper>();
            }

            /// <summary>
            /// Adds a stepper to the steppers list
            /// </summary>
            /// <param name="s">Stepper to be added</param>
            void RegisterStepper(Stepper s){
                _steppers.Add(s);
                _steppers.Sort();

                // if(controller == null){
                //     Start();
                // }
                
                controller.RegisterStepper(s);
            }

            /// <summary>
            /// Removes a stepper from the steppers List
            /// </summary>
            /// <param name="s">Stepper to be removed</param>
            void DeregisterStepper(Stepper s){
                _steppers.Remove(s);
                _steppers.Sort();

                controller.DeregisterStepper(s);
            }

            /// <summary>
            /// Creates a stepper
            /// </summary>
            /// <param name="callback">Name of method to be called. Should be provided as the method name, i.e. MethodName, not "MethodName" or MethodName()</param>
            public void CreateStepper(Utilities.Del callback){
                CreateStepper(callback, 1, 500, 0);
            }

            /// <summary>
            /// Creates a stepper
            /// </summary>
            /// <param name="callback">Name of method to be called. Should be provided as the method name, i.e. MethodName, not "MethodName" or MethodName()</param>
            /// <param name="_stepValue">The stepper execution frequency, in positive integer values (min = default = 1)</param>
            public void CreateStepper(Utilities.Del callback,
                            int _stepValue){
                CreateStepper(callback, _stepValue, 500, 0);
            }

            /// <summary>
            /// Creates a stepper
            /// </summary>
            /// <param name="callback">Name of method to be called. Should be provided as the method name, i.e. MethodName, not "MethodName" or MethodName()</param>
            /// <param name="_stepValue">The stepper execution frequency, in positive integer values (min = default = 1)</param>
            /// <param name="_priorityValue">Where to place the stepper in the scheduler queue, using priority values (soft range 0-1000, lower is earlier)</param>
            public void CreateStepper(Utilities.Del callback,
                            int _stepValue,
                            int _priorityValue){
                CreateStepper(callback, _stepValue, _priorityValue, 0);
            }
            
            /// <summary>
            /// Creates a stepper
            /// </summary>
            /// <param name="callback">Name of method to be called. Should be provided as the method name, i.e. MethodName, not "MethodName" or MethodName()</param>
            /// <param name="_stepValue">The stepper execution frequency, in positive integer values (min = default = 1)</param>
            /// <param name="_stepperQueuePrompt">Where to place the stepper in the scheduler queue, using StepperQueueOrder values (EARLY, NORMAL, LATE) (default = NORMAL)</param>
            public void CreateStepper(Utilities.Del callback,
                            int _stepValue,
                            Stepper.StepperQueueOrder _stepperQueuePrompt){
                CreateStepper(callback, _stepValue, _stepperQueuePrompt, 0);
            }

            /// <summary>
            /// Creates a stepper
            /// </summary>
            /// <param name="callback">Name of method to be called. Should be provided as the method name, i.e. MethodName, not "MethodName" or MethodName()</param>
            /// <param name="_stepValue">The stepper execution frequency, in positive integer values (min = default = 1)</param>
            /// <param name="_priorityValue">Where to place the stepper in the scheduler queue, using priority values (soft range 0-1000, lower is earlier)</param>
            /// <param name="_delayStartByFrames">The number of frames to delay stepper registration by (default = 0)</param>
            public void CreateStepper(Utilities.Del callback,
                            int _stepValue = 1,
                            int _priorityValue = 500,
                            int _delayStartByFrames = 0){
                
                if(_delayStartByFrames == 0){
                    Stepper s = new Stepper(_stepValue, callback, _priorityValue, this);
                    RegisterStepper(s);
                }
                else{
                    StartCoroutine(CreateStepperAfterFrames(_delayStartByFrames, _stepValue, callback, _priorityValue));
                }
            }

            /// <summary>
            /// Creates a stepper
            /// </summary>
            /// <param name="callback">Name of method to be called. Should be provided as the method name, i.e. MethodName, not "MethodName" or MethodName()</param>
            /// <param name="_stepValue">The stepper execution frequency, in positive integer values (min = default = 1)</param>
            /// <param name="_stepperQueuePrompt">Where to place the stepper in the scheduler queue, using StepperQueueOrder values (EARLY, NORMAL, LATE) (default = NORMAL)</param>
            /// <param name="_delayStartByFrames">The number of frames to delay stepper registration by (default = 0)</param>
            public void CreateStepper(Utilities.Del callback,
                            int _stepValue = 1,
                            Stepper.StepperQueueOrder _stepperQueuePrompt = Stepper.StepperQueueOrder.NORMAL,
                            int _delayStartByFrames = 0){
                
                if(_delayStartByFrames == 0){
                    Stepper s = new Stepper(_stepValue, callback, _stepperQueuePrompt, this);
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
                Stepper s = new Stepper(_stepValue, callback, _priorityValue, this);
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
                Stepper s = new Stepper(_stepValue, callback, _stepperQueuePrompt, this);
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
            /// Deletes a stepper
            /// </summary>
            /// <param name="stepperName">The name of the stepper (usually the method name) to be removed</param>
            public void DestroyStepper(string stepperName){
                DeregisterStepper(steppers.Find(s => s.name == stepperName));
            }

            /// <summary>
            /// OnDestroy is called when the steppable GameObject is destroyed.
            /// This ensures that all references to steppers on this steppable as well as the steppable itself are cleared
            /// </summary>
            private void OnDestroy() {
                for (int i = 0; i < steppers.Count; i++)
                {
                    Stepper s = steppers[i];
                    DestroyStepper(s);
                }
            }
        }
    }
}