using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlotElement", menuName = "ScriptableObjects/Slot Element", order = 0)]
public class SlotElement : ScriptableObject
{
    public Material material;
    public ElementType type;
    
}
