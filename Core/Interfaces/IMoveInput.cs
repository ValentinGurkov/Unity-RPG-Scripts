using UnityEngine;

namespace Core
{
    public interface IMoveInput
    {
        Vector3 MoveDirection { get; }
    }
}