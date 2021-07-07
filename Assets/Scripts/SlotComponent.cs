using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SlotComponent : MonoBehaviour
{
    private List<SlotElementComponent> _slotElements;

    private void Reset()
    {
        _slotElements = GetComponentsInChildren<SlotElementComponent>().ToList();
    }
}
