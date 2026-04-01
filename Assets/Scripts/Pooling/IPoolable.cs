namespace Pooling
{
    public interface IPoolable
    {
        void OnTakenFromPool();
        void OnReturnedToPool();
    }
}