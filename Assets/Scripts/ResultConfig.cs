using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


[CreateAssetMenu(fileName = "ResultConfig", menuName = "ScriptableObjects/Result Config", order = 1)]
public class ResultConfig : ScriptableObject
{
    [ValidateInput("EnforceSize", "All result objects must have exactly 3 elements. Percentages must add up to 100.")]
    [OnValueChanged("SetNumbers")]
    public List<ResultObject> ResultObjects = new List<ResultObject>();


    [Button("Validate Result Numbers")]
    private void SetNumbers()
    {
        for (int i = 0;  i < ResultObjects.Count; i++)
        {
            ResultObjects[i].resultNumber = i;
        }
    }
    private bool EnforceSize()
    {
        int num = 0;
        foreach (ResultObject resultObject in ResultObjects)
        {
            if (resultObject.elements.Count != 3)
            {
                return false;
            }

            num += resultObject.percentage;
        }

        return num == 100;
    }
}