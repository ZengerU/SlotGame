/*
 * Created By Umut Zenger
 * https://github.com/ZengerU/SlotGame
 */
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "ResultConfig", menuName = "ScriptableObjects/Result Config", order = 1)]
public class ResultConfig : ScriptableObject
{
   
    [Tooltip("List of possible results slot machine can end up on.")]
    [ValidateInput("EnforceSize", "All result objects must have exactly 3 elements. Percentages must add up to 100.")]
    [OnValueChanged("SetNumbers")]
    public List<ResultObject> ResultObjects = new List<ResultObject>();


    /// <summary>
    /// Sets the result numbers according to indices.
    /// </summary>
    [Button("Validate Result Numbers")]
    private void SetNumbers()
    {
        for (int i = 0;  i < ResultObjects.Count; i++)
        {
            ResultObjects[i].resultNumber = i;
        }
    }
    /// <summary>
    /// Enforces 3 elements on each result, a total of 100 on result percentage summation.
    /// </summary>
    /// <returns></returns>
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