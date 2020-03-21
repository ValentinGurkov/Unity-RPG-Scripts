using UnityEngine;

namespace RPG.Saving
{
    /// <summary>
    /// Serializable variant of Unity's Vector3 for use with the BinaryFormatter.
    /// </summary>
    [System.Serializable]
    public class SerializableVector3
    {
        private float m_X, m_Y, m_Z;

        public SerializableVector3(Vector3 vector)
        {
            m_X = vector.x;
            m_Y = vector.y;
            m_Z = vector.z;
        }

        public Vector3 ToVector()
        {
            return new Vector3(m_X, m_Y, m_Z);
        }
    }
}