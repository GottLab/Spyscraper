using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[Serializable]
public abstract class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{

    [Serializable]
    public struct SerializableTuple<A, B>
    {
        public A key;
        public B Value;
    }

    [SerializeField]
    private List<SerializableTuple<TKey, TValue>> values = new();


    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            SerializableTuple<TKey, TValue> tuple = new()
            {
                key = pair.Key,
                Value = pair.Value
            };
            values.Add(tuple);
        }
    }

    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();

        foreach (var pair in values)
        {
            if (!this.TryAdd(pair.key, pair.Value))
                this.TryAdd(GetNewKey(pair.key), pair.Value);
        }
    }

    public abstract TKey GetNewKey(TKey key);
}