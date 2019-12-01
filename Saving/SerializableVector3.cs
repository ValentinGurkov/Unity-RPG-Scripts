using UnityEngine;

namespace RPG.Saving {
    /// <summary>
    /// Serializable variant of Unity's Vector3 for use with the BinaryFormatter.
    /// </summary>
    [System.Serializable]
    public class SerializableVector3 {
        private float x, y, z;

        public SerializableVector3(Vector3 vector) {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector() {
            return new Vector3(x, y, z);
        }
    }
}
