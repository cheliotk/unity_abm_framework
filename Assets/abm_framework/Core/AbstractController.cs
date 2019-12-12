namespace ABM
{
    namespace Core
    {
        using System.Collections.Generic;
        using UnityEngine;
        public class AbstractController : MonoBehaviour, ISteppable, IInitializable{
            List<AbstractAgent> _agents;
            public List<AbstractAgent> agents{
                get{
                    return _agents;
                }
                private set{
                    _agents = value;
                }
            }

            int simStartTime;

            public void RegisterAgent(AbstractAgent a){
                _agents.Add(a);
            }

            public void DeregisterAgent(AbstractAgent a){
                _agents.Remove(a);
            }

            public virtual void Init(){
                agents = new List<AbstractAgent>();
                simStartTime = Time.frameCount;
            }

            public virtual void Step(){
                if(Time.frameCount - simStartTime < 0){
                    return;
                }
                AgentStepLoop(int.MinValue, int.MaxValue);
            }

            public virtual void Step(int priorityS = int.MinValue, int priorityE = int.MaxValue){
                if(Time.frameCount - simStartTime < 0){
                    return;
                }

                AgentStepLoop(priorityS, priorityE);
            }

            public virtual void Step(Utilities.StepperQueueOrder stepperQueuePrompt){
                if(Time.frameCount - simStartTime < 0){
                    return;
                }

                AgentStepLoop(stepperQueuePrompt);
            }

            public void AgentStepLoop(int s, int e){
                
                var agentsFiltered = agents.FindAll(a => a.HasSteppersInPriorityRange(s, e));

                foreach (AbstractAgent a in agentsFiltered)
                {
                    a.Step(s,e);
                }
            }

            public void AgentStepLoop(Utilities.StepperQueueOrder stepperQueuePrompt){
                
                var agentsFiltered = agents.FindAll(a => a.HasSteppersInQueue(stepperQueuePrompt));

                foreach (AbstractAgent a in agentsFiltered)
                {
                    a.Step(stepperQueuePrompt);
                }
            }

            void Start(){
                Init();
            }

            void LateUpdate(){
                Step();
            }
        }
    }
}