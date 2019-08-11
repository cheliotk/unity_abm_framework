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
                set{
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

            public virtual void Step(int priorityS = int.MinValue, int priorityE = int.MaxValue){
                foreach (AbstractAgent a in agents){
                    a.Step();
                }
            }

            public virtual void Step(){
                if(Time.frameCount - simStartTime <=0){
                    return;
                }
                AgentStepLoop(int.MinValue, int.MaxValue);
            }

            public void AgentStepLoop(int s, int e){
                foreach (AbstractAgent a in agents)
                {
                    a.Step(s,e);
                }
            }

            void Start(){
                Init();
            }

            void FixedUpdate(){
                Step();
            }
        }
    }
}