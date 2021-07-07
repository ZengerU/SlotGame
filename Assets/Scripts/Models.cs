
using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine.Rendering;

public enum ElementType
{
    A,
    Bonus,
    Wild,
    Seven,
    Jackpot
}

[Serializable]
public class ResultObject
{
    public List<ElementType> elements = new List<ElementType>()
    {
        ElementType.A,
        ElementType.A,
        ElementType.A
    };
    public int type;
    public int percentage = 13;
    public int resultNumber;

    public ResultObject()
    {
    }
    public ResultObject(ResultObject other)
    {
        elements = other.elements;
        type = other.type;
        percentage = other.percentage;
        resultNumber = other.resultNumber;
    }
}

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
