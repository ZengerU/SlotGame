/*
 * Created By Umut Zenger
 * https://github.com/ZengerU/SlotGame
 */

using System;
using System.Collections.Generic;


/// <summary>
/// Type of elements on the slot machine.
/// </summary>
public enum ElementType
{
    A,
    Bonus,
    Seven,
    Wild,
    Jackpot
}

/// <summary>
/// A single result object notating the specific elements it needs, how often it occurs, and what its reward type is.
/// </summary>
[Serializable]
public class ResultObject
{
    public List<ElementType> elements = new List<ElementType>()
    {
        ElementType.A,
        ElementType.A,
        ElementType.A
    };
    public int rewardType;
    public int percentage = 13;
    public int resultNumber;

    public ResultObject()
    {
    }
    public ResultObject(ResultObject other)
    {
        elements = other.elements;
        rewardType = other.rewardType;
        percentage = other.percentage;
        resultNumber = other.resultNumber;
    }
}

/// <summary>
/// Queue made to hold result objects that are to be used.
/// </summary>
public class ResultQueue : Queue<ResultObject>
{
    public override string ToString()
    {
        string result = string.Empty;
        foreach (ResultObject resultObject in this)
        {
            result += resultObject.resultNumber.ToString();
        }

        return result;
    }
}
