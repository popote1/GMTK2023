using UnityEngine;

namespace script
{
    public interface IDestructible
    {
        public abstract void TakeDamage(int damage);
        public abstract void Destroy();
        public abstract bool IsAlive();
    }
}