using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    private const string ResultKey = "results";
    private Dictionary<ElementType, SlotElement> _typeElementMap = new Dictionary<ElementType, SlotElement>();

    private ResultQueue _futureResults = new ResultQueue();
    private ResultObject _nextResult;
    [SerializeField] private Transform leftSlotParent, middleSlotParent, rightSlotParent;


    private void Awake()
    {
        LoadElements();
        LoadOrCreateFutureResults();
    }

    private void LoadElements()
    {
        SlotElement[] elements = Resources.LoadAll<SlotElement>("Elements");
        foreach (SlotElement element in elements)
        {
            _typeElementMap.Add(element.type, element);
        }
    }

    private void LoadOrCreateFutureResults()
    {
        string storedResults = PlayerPrefs.GetString(ResultKey, string.Empty);
        if (storedResults == string.Empty)
        {
            CreateNewResults();
        }
        else
        {
            AssignResults(storedResults);
        }
    }

    private void AssignResults(string storedResults)
    {
        foreach (char result in storedResults)
        {
            _futureResults.Enqueue(ResultLookup[int.Parse(result.ToString())]);
        }
    }

    private void CreateNewResults()
    {
        ResultObject[] newResults = new ResultObject[100];
        foreach (ResultObject resultObject in ResultLookup)
        {
            int frequency = 100 / resultObject.Percentage;
            for (int i = 0; i < resultObject.Percentage; i++)
            {
                List<int> possibleNumbers = new List<int>();
                for (int j = 0; j < frequency; j++)
                {
                    if (newResults[i * frequency + j] == null)
                    {
                        possibleNumbers.Add(j);
                    }   
                }
                if (possibleNumbers.Count == 0)
                {
                    Debug.LogException(new Exception(message:"Found no possible placement for result object!"));
                }

                newResults[possibleNumbers.GetRandomElementFromList()] = new ResultObject(resultObject);
            }
        }

        foreach (ResultObject o in newResults)
        {
            _futureResults.Enqueue(o);
        }
        SaveFutureResults();
    }

    public void Spin()
    {
        _nextResult = _futureResults.Dequeue();
        SaveFutureResults();
    }

    private void SaveFutureResults()
    {
        PlayerPrefs.SetString(ResultKey, _futureResults.ToString());
        PlayerPrefs.Save();
    }

    private static readonly List<ResultObject> ResultLookup = new List<ResultObject>()
    {
        new ResultObject()
        {
            Elements = new[] {ElementType.A, ElementType.Wild, ElementType.Bonus}, ResultNumber = 0
        },
        new ResultObject()
        {
            Elements = new[] {ElementType.Wild, ElementType.Wild, ElementType.Seven}, ResultNumber = 1
        },
        new ResultObject()
        {
            Elements = new[] {ElementType.Jackpot, ElementType.Jackpot, ElementType.A}, ResultNumber = 2
        },
        new ResultObject()
        {
            Elements = new[] {ElementType.Wild, ElementType.Bonus, ElementType.A}, ResultNumber = 3
        },
        new ResultObject()
        {
            Elements = new[] {ElementType.Bonus, ElementType.A, ElementType.Jackpot}, ResultNumber = 4
        },
        new ResultObject()
        {
            Elements = new[] {ElementType.A, ElementType.A, ElementType.A}, Type = 1, ResultNumber = 5, Percentage = 9
        },
        new ResultObject()
        {
            Elements = new[] {ElementType.Bonus, ElementType.Bonus, ElementType.Bonus}, Type = 2, ResultNumber = 6,
            Percentage = 8
        },
        new ResultObject()
        {
            Elements = new[] {ElementType.Seven, ElementType.Seven, ElementType.Seven}, Type = 3, ResultNumber = 7,
            Percentage = 7
        },
        new ResultObject()
        {
            Elements = new[] {ElementType.Wild, ElementType.Wild, ElementType.Wild}, Type = 4, ResultNumber = 8,
            Percentage = 6
        },
        new ResultObject()
        {
            Elements = new[] {ElementType.Jackpot, ElementType.Jackpot, ElementType.Jackpot}, Type = 5,
            ResultNumber = 9, Percentage = 5
        },
    };
}