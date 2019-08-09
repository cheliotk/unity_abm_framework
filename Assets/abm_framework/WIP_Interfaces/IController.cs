namespace ABM
{
    namespace Core
    {
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;
        public interface IController: IInitializable<object>
        {
            List<IAgent> agents{ get; }
            void RegisterAgent(IAgent a);
            void DeregisterAgent(IAgent a);
        }
    }
}