
using System.Collections.Generic;
using UnityEngine.Rendering;

public enum ElementType
{
    A,
    Bonus,
    Wild,
    Seven,
    Jackpot
}

public class ResultObject
{
    public ElementType[] Elements = new ElementType[3];
    public int Type = 0;
    public int Percentage = 13;
    public int ResultNumber;

    public ResultObject()
    {
        
    }
    public ResultObject(ResultObject other)
    {
        Elements = other.Elements;
        Type = other.Type;
        Percentage = other.Percentage;
        ResultNumber = other.ResultNumber;
    }
}

public class ResultQueue : Queue<ResultObject>
{
    public override string ToString()
    {
        string result = string.Empty;
        foreach (ResultObject resultObject in this)
        {
            result += resultObject.ResultNumber.ToString();
        }

        return result;
    }
}
