namespace ABMU
{
    namespace Core
    {
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;

        /// <summary>
        /// Abstract agent class. To be used as a blueprint for specific simulation agents. A simulation-specific agent class should inherit from this class
        /// </summary>
        public class AbstractAgent : Steppable
        {
            /// <summary>
            /// Agent Awake method. Is called automatically by UnityEngine when the agent GameObject is instantiated and registers it with the controller
            /// </summary>
            public override void Awake() {
                base.Awake();
                controller.RegisterAgent(this);
            }
        }
    }
}