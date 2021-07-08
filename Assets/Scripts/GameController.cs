using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    private const string ResultKey = "results";
    private Dictionary<ElementType, SlotElement> _typeElementMap = new Dictionary<ElementType, SlotElement>();

    [SerializeField] private ResultConfig config;
    private ResultQueue _futureResults = new ResultQueue();
    private ResultObject _nextResult;
    private List<int[]> _placementPoints = new List<int[]>();
    [SerializeField] private SlotController leftController, middleController, rightController;
    [SerializeField] private Button spinButton;
    [SerializeField] private ParticleSystem prizeParticleSystem;
    private ParticleSystem.EmissionModule _prizeEmitter;
    private int _spinningCount;


    private void Awake()
    {
        LoadElements();
        LoadOrCreateFutureResults();
        _prizeEmitter = prizeParticleSystem.emission;
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
            PlaceResult(changing, GetResultPlacement(changing, rand), ref newResults);
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
                points[i] = (int) (frequency * i);
            }

            _placementPoints.Add(points);
        }
    }

    public void Spin()
    {
        _spinningCount = 3;
        _nextResult = _futureResults.Dequeue();
        StartCoroutine(leftController.Spin(0, 4, Random.value / 10, _nextResult.elements[0]));
        StartCoroutine(middleController.Spin(0.1f, 4.1f, Random.value / 10, _nextResult.elements[1]));
        StartCoroutine(rightController.Spin(0.2f, 4.5f, _nextResult.type > 0 ? Random.value + 2 : Random.value + 1,
            _nextResult.elements[2]));
        print($"{_nextResult.elements[0]} - {_nextResult.elements[1]} - {_nextResult.elements[2]}");
        SaveFutureResults();
    }

    [SerializeField] private float bigPrizeEmission, mediumPrizeEmission, smallPrizeEmission;

    public void SpinnerStopped()
    {
        _spinningCount--;
        if (_spinningCount > 0) return;
        if (_nextResult.type == 0)
        {
            spinButton.interactable = true;
            return;
        }
        if (_nextResult.type >= 4)
        {
            _prizeEmitter.rateOverTime = bigPrizeEmission;
        }
        else if (_nextResult.type >= 2)
        {
            _prizeEmitter.rateOverTime = mediumPrizeEmission;
        }
        else
        {
            _prizeEmitter.rateOverTime = smallPrizeEmission;
        }
        prizeParticleSystem.Play();;
        DOVirtual.DelayedCall(prizeParticleSystem.main.duration + ((float)prizeParticleSystem.main.startLifetimeMultiplier), () =>
        {
            spinButton.interactable = true;
        });
    }

    private void SaveFutureResults()
    {
        if (_futureResults.Count == 0)
        {
            CreateNewResults();
        }

        PlayerPrefs.SetString(ResultKey, _futureResults.ToString());
        PlayerPrefs.Save();
    }
}