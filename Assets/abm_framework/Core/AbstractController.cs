namespace ABM
{
    namespace Core
    {
        using System.Collections.Generic;
        using UnityEngine;
        using System.Diagnostics;

        /// <summary>
        /// Abstract controller class. To be used as a blueprint for simulation-specific controllers. A simulation-specific controller should inherit from this class
        /// </summary>
        public class AbstractController : MonoBehaviour, IInitializable{

            int milisDelayCum = 0;
            float millisDelayAvg = 0f;
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
            /// The simulation start time, in frames sinceapplication started
            /// </summary>
            int simStartTime;
            
            /// <summary>
            /// Controller initializer method. Initializes the agent list and records simulation start time (in frames)
            /// </summary>
            public virtual void Init(){
                _agents = new List<AbstractAgent>();
                simStartTime = Time.frameCount;

                scheduler = new Scheduler();
                // scheduler.Init();
            }

            /// <summary>
            /// Default method for starting the loop to execute steppers on agents for this frame.
            /// </summary>
            public virtual void Step(){
                if(Time.frameCount - simStartTime < 0){
                    return;
                }
                
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                scheduler.Tick();

                stopwatch.Stop();
                milisDelayCum += stopwatch.Elapsed.Milliseconds;
                if(Time.frameCount - simStartTime != 0)
                    millisDelayAvg  = (float)milisDelayCum / (Time.frameCount - simStartTime);
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
            /// Starts the whole simulation. Is called at the beginning of the simulation from UnityEngine.
            /// </summary>
            void Start(){
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