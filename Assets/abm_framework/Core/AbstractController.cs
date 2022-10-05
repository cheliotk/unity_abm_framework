/*
This code is licensed under MIT license, Copyright (c) 2019 Kostas Cheliotis
https://github.com/cheliotk/unity_abm_framework/blob/master/LICENSE
*/

namespace ABMU
{
    namespace Core
    {
        using System.Collections.Generic;
        using UnityEngine;
        using System.Diagnostics;
        using UnityEditor;

        /// <summary>
        /// Abstract controller class. To be used as a blueprint for simulation-specific controllers. A simulation-specific controller should inherit from this class
        /// </summary>
        public class AbstractController : MonoBehaviour, IInitializable
        {
            int milisDelayCum = 0;
            float millisDelayAvg = 0f;
            Stopwatch stopwatch = new Stopwatch();
            Scheduler scheduler;

            /// <summary>
            /// A register of all agents currently in the simulation
            /// </summary>
            List<AbstractAgent> _agents;
            public List<AbstractAgent> agents{
                get{
                    return _agents;
                }
            }

            /// <summary>
            /// The simulation start time, in frames since application started
            /// </summary>
            int simStartTime;

            /// <summary>
            /// The current tick in the simulation
            /// </summary>
            public int currentTick {get; private set;}

            [SerializeField] protected bool isSimulationPaused = false;
            public bool IsSimulationPaused => isSimulationPaused;

            [SerializeField] protected int endFrame = -1;

            [SerializeField] protected bool useSeed = false;
            [SerializeField] protected int seed = 0;
            [SerializeField] protected bool monitorPerformance = false;
            
            /// <summary>
            /// Controller initializer method. Initializes the agent list and records simulation start time (in frames)
            /// </summary>
            public virtual void Init(){
                _agents = new List<AbstractAgent>();
                simStartTime = Time.frameCount;
                currentTick = 0;
                scheduler = new Scheduler();

                if (!useSeed)
                    seed = Random.Range(int.MinValue, int.MaxValue);

                Random.InitState(seed);
            }

            /// <summary>
            /// Default method for starting the loop to execute steppers on agents for this frame.
            /// </summary>
            public virtual void Step(){
                if(Time.frameCount - simStartTime <= 0){
                    return;
                }

                if(!isSimulationPaused){

                    if (endFrame > 0 && currentTick - simStartTime > endFrame)
                    {
                        isSimulationPaused = true;
                        return;
                    }

                    currentTick ++;

                    if (monitorPerformance)
                    {
                        stopwatch.Start();
                    }                

                    scheduler.Tick();

                    if (monitorPerformance)
                    {
                        stopwatch.Stop();
                        milisDelayCum += stopwatch.Elapsed.Milliseconds;
                        millisDelayAvg  = (float)milisDelayCum / (Time.frameCount - simStartTime);
                    }
                }
            }

            /// <summary>
            /// Adds and agent to the register list.
            /// </summary>
            /// <param name="a">The agent to add</param>
            public void RegisterAgent(AbstractAgent a){
                _agents.Add(a);
            }

            /// <summary>
            /// Removes an agent from the register list. DOES NOT destroy the agent object, this should be taken care of by the script calling this function. 
            /// </summary>
            /// <param name="a">The agent to remove</param>
            public void DeregisterAgent(AbstractAgent a){
                _agents.Remove(a);
            }

            /// <summary>
            /// Passes a stepper to the scheduler, to be registered on the scheduler queue
            /// </summary>
            /// <param name="s">The stepper to register</param>
            public void RegisterStepper(Stepper s){
                scheduler.RegisterStepper(s);
            }

            /// <summary>
            /// Passes a stepper to the scheduler to be removed from the scheduler queue.
            /// NOTE: Does not actually delete the stepper object, object removal should be taken care of in the script that initiated the de-registration.
            /// </summary>
            /// <param name="s">The stepper to be removed from the queue</param>
            public void DeregisterStepper(Stepper s){
                scheduler.DeregisterStepper(s);
            }

            /// <summary>
            /// Pauses the simulation after a predetermined number of ticks
            /// </summary>
            public void PauseAtFrame(){
                #if UNITY_EDITOR
                    if(endFrame > -1 && currentTick >= endFrame){
                        EditorApplication.isPaused = true;
                    }
                #endif
            }

            /// <summary>
            /// Starts the whole simulation. Is called at the beginning of the simulation from UnityEngine.
            /// </summary>
            void Awake(){
                Init();
            }

            /// <summary>
            /// Default method for executing all steppers on all agents this frame. Is called from UnityEngine ath the end of the frame, after all other executions (physics, user input, etc) have taken place 
            /// </summary>
            void LateUpdate(){
                Step();
            }
        }
    }
}