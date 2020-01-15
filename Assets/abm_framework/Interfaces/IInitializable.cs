namespace ABMU
{
    namespace Core
    {
        /// <summary>
        /// Interface to the Initializable class. Classes implementing this interface can be initialized.
        /// </summary>
        public interface IInitializable
        {
            /// <summary>
            /// The default initialization method to call for setting up the implementing class. Can (and likely will) be overriden in the implementing class.
            /// </summary>
            void Init();
        }
    }
}