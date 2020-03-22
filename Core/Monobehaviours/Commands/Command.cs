using UnityEngine;

namespace Core
{
    public abstract class Command : MonoBehaviour
    {
        public virtual void Execute() { }
    }
}