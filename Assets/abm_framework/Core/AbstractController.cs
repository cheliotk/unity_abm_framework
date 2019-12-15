/*
TODO: Possibly extend ISteppable or remove from Controller altogether?
Maybe create a dedicated IScheduler interface used exclusively for controller scheduler,
and keep Isteppable for agent-like objects.
*/
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
        public class AbstractController : MonoBehaviour, ISteppable, IInitializable{

            int milisDelayCum = 0;
            float millisDelayAvg = 0f;
            AbstractScheduler scheduler;

            /// <summary>
            /// A register of all agents currently in the simulation
            /// </summary>
            List<AbstractAgent> _agents;
            public List<AbstractAgent> agents{
                get{
                    return _agents;
                }
                // private set{
                //     _agents = value;
                // }
            }

            /// <summary>
            /// The simulation start time, in frames sinceapplication started
            /// </summary>
            int simStartTime;

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
            /// Controller initializer method. Initializes the agent list and records simulation start time (in frames)
            /// </summary>
            public virtual void Init(){
                _agents = new List<AbstractAgent>();
                simStartTime = Time.frameCount;

                scheduler = new AbstractScheduler();
                scheduler.Init();
            }

            /// <summary>
            /// Default method for starting the loop to execute steppers on agents. Executes behaviours asynchronously (for each agent executes all steppers sorted by priority before moving to next agent)
            /// </summary>
            public virtual void Step(){
                if(Time.frameCount - simStartTime < 0){
                    return;
                }
                
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                scheduler.Tick();
                // AgentStepLoop(int.MinValue, int.MaxValue);

                stopwatch.Stop();

                // UnityEngine.Debug.Log("ControllerStep: " + stopwatch.Elapsed);
                milisDelayCum += stopwatch.Elapsed.Milliseconds;
                if(Time.frameCount - simStartTime != 0)
                    millisDelayAvg  = (float)milisDelayCum / (Time.frameCount - simStartTime);
            }

            /// <summary>
            /// Default method for starting the loop to execute steppers on agents. Executes all behaviours within a given priority value range, but calls steppers asynchronously WITHIN the range (for each agent executes all steppers in the priority range, sorted, before moving to next agent)
            /// </summary>
            /// <param name="priorityS">The lower bound for the stepper priority range (inclusive)</param>
            /// <param name="priorityE">The upper bound for the stepper priority range (exclusive)</param>
            public virtual void Step(int priorityS = int.MinValue, int priorityE = int.MaxValue){
                if(Time.frameCount - simStartTime < 0){
                    return;
                }

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // scheduler.Tick();
                AgentStepLoop(priorityS, priorityE);

                stopwatch.Stop();

                // UnityEngine.Debug.Log("ControllerStep: " + stopwatch.Elapsed);
                milisDelayCum += stopwatch.Elapsed.Milliseconds;
                if(Time.frameCount - simStartTime != 0)
                    millisDelayAvg  = (float)milisDelayCum / (Time.frameCount - simStartTime);

                
            }

            /// <summary>
            /// Default method for starting the loop to execute steppers on agents. Executes all behaviours within a given queue slot, but calls steppers asynchronously WITHIN the slot (for each agent executes all steppers in the given slot, sorted by priority, before moving to next agent) 
            /// </summary>
            /// <param name="_stepperQueuePrompt">The stepperQueue slot (EARLY, NORMAL, LATE) for which to execute steppers</param>
            public virtual void Step(Stepper.StepperQueueOrder _stepperQueuePrompt){
                if(Time.frameCount - simStartTime < 0){
                    return;
                }

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // scheduler.Tick();
                AgentStepLoop(_stepperQueuePrompt);

                stopwatch.Stop();

                // UnityEngine.Debug.Log("ControllerStep: " + stopwatch.Elapsed);
                milisDelayCum += stopwatch.Elapsed.Milliseconds;
                if(Time.frameCount - simStartTime != 0)
                    millisDelayAvg  = (float)milisDelayCum / (Time.frameCount - simStartTime);

                
            }

            /// <summary>
            /// Default method for sorting and executing steppers for this frame. For a given stepper priority value range (0-1000, lower is earlier) gets all agents with steppers at that range and executes them.
            /// </summary>
            /// <param name="s">The lower bound for the stepper priority range (inclusive)</param>
            /// <param name="e">The upper bound for the stepper priority range (exclusive)</param>
            void AgentStepLoop(int s, int e){
                
                var agentsFiltered = agents.FindAll(a => a.HasSteppersInPriorityRange(s, e));

                foreach (AbstractAgent a in agentsFiltered)
                {
                    a.Step(s,e);
                }
            }

            /// <summary>
            /// Default method for sorting and executing steppers for this frame. For a given stepperQueue slot (EARLY, NORMAL, LATE) gets all agents with steppers at that slot and executes them.
            /// </summary>
            /// <param name="_stepperQueuePrompt">The stepperQueue slot for which to execute steppers</param>
            void AgentStepLoop(Stepper.StepperQueueOrder _stepperQueuePrompt){
                
                var agentsFiltered = agents.FindAll(a => a.HasSteppersInQueue(_stepperQueuePrompt));

                foreach (AbstractAgent a in agentsFiltered)
                {
                    a.Step(_stepperQueuePrompt);
                }
            }

            public void RegisterStepper(Stepper s){
                scheduler.RegisterStepper(s);
            }

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