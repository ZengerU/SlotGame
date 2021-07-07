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

    [SerializeField] private ResultConfig config;
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
            _futureResults.Enqueue(config.ResultObjects[int.Parse(result.ToString())]);
        }
    }

    private void CreateNewResults()
    {
        ResultObject[] newResults = new ResultObject[100];
        foreach (ResultObject resultObject in config.ResultObjects)
        {
            int frequency = 100 / resultObject.percentage;
            for (int i = 0; i < resultObject.percentage; i++)
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
}