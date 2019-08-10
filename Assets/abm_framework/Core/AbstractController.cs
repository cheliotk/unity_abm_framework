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

            public void RegisterAgent(AbstractAgent a){
                _agents.Add(a);
            }

            public void DeregisterAgent(AbstractAgent a){
                _agents.Remove(a);
            }

            public virtual void Init(){
                agents = new List<AbstractAgent>();
            }

            public virtual void Step(){
                foreach (AbstractAgent a in agents){
                    a.Step();
                }
            }

            void Start(){
                Init();
            }

            void Update(){
                Step();
            }
        }
    }
}