namespace ABMU
{
    namespace Core
    {
        /// <summary>
        /// Interface fo the Steppable class. Classes implementing this interface can use steppers and execute methods every frame.
        /// </summary>
        public interface ISteppable
        {
            /// <summary>
            /// Default step method. Executes all Steppers registered by the implementing class within a specific stepperQueue slot (EARLY, NORMAL, LATE) 
            /// </summary>
            /// <param name="_stepperQueuePrompt">The stepperQueue value slot to filter steppers by</param>
            void Step(Stepper.StepperQueueOrder _stepperQueuePrompt);

            /// <summary>
            /// Default step method. Executes all Steppers registered by the implementing class within a specific stepper priority range (0-1000, lower is earlier) 
            /// </summary>
            /// <param name="priorityS">The lower bound for priority range (inclusive)</param>
            /// <param name="priorityE">The upper bound for priority range (exclusive)</param>
            void Step(int priorityS, int priorityE);

            /// <summary>
            /// Default step method. Executes all Steppers registered by the implementing class
            /// </summary>
            void Step();
        }
    }
}