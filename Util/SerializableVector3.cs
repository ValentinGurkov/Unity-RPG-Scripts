using UnityEngine;

namespace Util
{
    /// <summary>
    /// Serializable variant of Unity's Vector3 for use with the BinaryFormatter.
    /// </summary>
    [System.Serializable]
    public class SerializableVector3
    {
        private float _x, _y, _z;

        public SerializableVector3(Vector3 vector)
        {
            _x = vector.x;
            _y = vector.y;
            _z = vector.z;
        }

        public Vector3 ToVector()
        {
            return new Vector3(_x, _y, _z);
        }
    }
}