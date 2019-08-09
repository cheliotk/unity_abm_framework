namespace ABM
{
    namespace Core
    {
        using System.Collections;
        using System.Collections.Generic;
        // using UnityEngine;
        public interface IAgent: ISteppable, IInitializable<object>
        {
            List<Stepper> steppers{get; set;}
        }
    }
}