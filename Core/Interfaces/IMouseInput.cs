using UnityEngine;

namespace Core
{
    public interface IMouseInput
    {
        bool IsHoldingMouseButton { get; }
        Vector2 MousePosition { get; }
    }
}