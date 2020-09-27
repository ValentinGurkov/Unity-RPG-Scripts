using System;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public abstract class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private readonly List<TKey> keyData = new List<TKey>();

        [SerializeField, HideInInspector] private readonly List<TValue> valueData = new List<TValue>();

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            for (var i = 0; i < keyData.Count && i < valueData.Count; i++)
            {
                this[keyData[i]] = valueData[i];
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            keyData.Clear();
            valueData.Clear();

            foreach (KeyValuePair<TKey, TValue> item in this)
            {
                keyData.Add(item.Key);
                valueData.Add(item.Value);
            }
        }
    }

    [Serializable]
    public class StringScriptableObjectDictionary : SerializableDictionary<string, ScriptableObject> { }
}