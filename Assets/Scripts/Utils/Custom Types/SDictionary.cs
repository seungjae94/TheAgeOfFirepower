using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mathlife.ProjectL.Utils
{
    [Serializable]
    public class SPair<K, V>
    {
        public SPair(K key, V value)
        {
            this.key = key;
            this.value = value;
        }

        public K key;
        public V value;

        // implicit conversion (KeyValuePair -> SPair)
        public static implicit operator SPair<K, V>(KeyValuePair<K, V> pair)
            => new SPair<K, V>(pair.Key, pair.Value);

        // implicit conversion (KeyValuePair -> SPair)
        public static implicit operator KeyValuePair<K, V>(SPair<K, V> pair)
            => new KeyValuePair<K, V>(pair.key, pair.value);
    }

    // Serializable Dictionary
    // @see https://github.com/EduardMalkhasyan/Serializable-Dictionary-Unity/blob/master/Assets/Plugins/SerializableDictionary/SerializableDictionary.cs

    [Serializable]
    public class SDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<SPair<K, V>> sPairs;

        public Type KeyType => typeof(K);

        public Type ValueType => typeof(V);

        public void OnBeforeSerialize()
        {
            foreach (KeyValuePair<K, V> kvPair in this)
            {
                // kvPair.Key is already included in sPairs
                if (sPairs.FirstOrDefault(sPair => Comparer.Equals(kvPair.Key, sPair.key))
                    is SPair<K, V> sPair)
                {
                    sPair.value = kvPair.Value;
                }
                // kvPair.Key is not included in sPairs
                else
                {
                    sPairs.Add(kvPair);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            foreach (SPair<K, V> sPair in sPairs)
            {
                if (ContainsKey(sPair.key))
                    continue;

                Add(sPair.key, sPair.value);
            }
        }

        public new V this[K key]
        {
            get { return base[key]; }
        }
    }
}