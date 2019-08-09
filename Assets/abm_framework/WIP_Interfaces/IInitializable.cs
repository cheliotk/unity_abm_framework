namespace ABM
{
    namespace Core
    {
        public interface IInitializable<T>
        {
            void Init(T param);
        }
    }
}