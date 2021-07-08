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
    private List<int[]> _placementPoints = new List<int[]>();
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
        GeneratePlacementPoints();
        ResultObject[] newResults = new ResultObject[100];
        Array.Clear(newResults, 0, newResults.Length);
        foreach (ResultObject resultObject in config.ResultObjects)
        {
            for (int i = 0; i < resultObject.percentage; i++)
            {
                PlaceResult(new ResultObject(resultObject), i, ref newResults);
            }
        }
        foreach (ResultObject o in newResults)
        {
            _futureResults.Enqueue(o);
        }
        SaveFutureResults();
    }

    private void PlaceResult(ResultObject result, int place, ref ResultObject[] newResults)
    {
        print($"{result.resultNumber}-{place}");
        int min = _placementPoints[result.resultNumber][place];
        int max = place == result.percentage - 1 ? 100 : _placementPoints[result.resultNumber][place + 1];
        List<int> possibleNumbers = new List<int>();
        for (int i = min; i < max; i++)
        {
            if (newResults[i] == null)
            {
                possibleNumbers.Add(i);
            }
        }
        if (possibleNumbers.Count == 0)
        {
            int rand = Random.Range(min, max);
            ResultObject changing = newResults[rand];
            newResults[rand] = result;
            PlaceResult(changing, GetResultPlacement(changing, rand),ref newResults);
        }
        else
        {
            newResults[possibleNumbers.GetRandomElementFromList()] = new ResultObject(result);    
        }
    }

    private int GetResultPlacement(ResultObject res, int oldPlacement)
    {
        int[] limits = _placementPoints[res.resultNumber];
        int place = 0;
        foreach (int limit in limits)
        {
            if (limit < oldPlacement)
            {
                place++;
            }
            else
            {
                break;
            }
        }
        return place;
    }
    private void GeneratePlacementPoints()
    {
        foreach (ResultObject resultObject in config.ResultObjects)
        {
            float frequency = 100f / resultObject.percentage;
            int[] points = new int[resultObject.percentage];
            for (int i = 0; i < resultObject.percentage; i++)
            {
                points[i] = (int)(frequency * i) ;
            }
            _placementPoints.Add(points);
        }
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