using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


[CreateAssetMenu(fileName = "ResultConfig", menuName = "ScriptableObjects/Result Config", order = 1)]
public class ResultConfig : ScriptableObject
{
    [ValidateInput("EnforceSize", "All result objects must have exactly 3 elements. Percentages must add up to 100.")]
    public List<ResultObject> ResultObjects = new List<ResultObject>();
    
    
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
