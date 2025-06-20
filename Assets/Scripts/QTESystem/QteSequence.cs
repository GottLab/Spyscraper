using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QteSequence", menuName = "Scriptable Objects/QteSequence")]
public class QteSequence : ScriptableObject
{   
    
    [Serializable]
    public struct QteSequenceElement
    {   
        public float time;
    }
    
    public QteSequenceElement[] sequence = new QteSequenceElement[0];
}
