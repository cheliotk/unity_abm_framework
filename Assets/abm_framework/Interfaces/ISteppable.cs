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
            /// Default step method. Executes the corresponding method registered by the implementing class
            /// </summary>
            void Step();
        }
    }
}