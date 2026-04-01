using UnityEngine;

namespace Pooling
{
    public sealed class PoolMember : MonoBehaviour
    {
        public PrefabPool OwnerPool { get; private set; }
        public bool IsInPool { get; private set; }

        public void SetOwner(PrefabPool ownerPool)
        {
            OwnerPool = ownerPool;
        }

        public void MarkTaken()
        {
            IsInPool = false;
        }

        public void MarkReturned()
        {
            IsInPool = true;
        }

        public void ReturnToPool()
        {
            if (OwnerPool == null)
            {
                Destroy(gameObject);
                return;
            }

            if (IsInPool)
            {
                return;
            }

            OwnerPool.Release(gameObject);
        }
    }
}