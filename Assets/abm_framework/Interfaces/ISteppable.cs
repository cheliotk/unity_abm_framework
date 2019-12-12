namespace ABM
{
    namespace Core
    {
        using System;
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;
        public interface ISteppable
        {
            void Step(ABM.Utilities.StepperQueueOrder stepperQueuePrompt);
            void Step(int priorityS, int priorityE);
            void Step();
        }
    }
}