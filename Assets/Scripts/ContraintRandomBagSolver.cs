/*
 * Created By Umut Zenger
 * https://github.com/ZengerU/SlotGame
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class ContraintRandomBagSolver
{
    private int _size;
    private ResultConfig _config;
    private List<int[]> _placementPoints = new List<int[]>();

    /// <summary>
    /// Creates a result generator.
    /// </summary>
    /// <param name="size">Size of the result to be generated. Values other than 100 have not been tested and are likely to fail.</param>
    public ContraintRandomBagSolver(int size)
    {
        _size = size;
        _config = Resources.Load<ResultConfig>($"ResultConfig");
    }

    /// <summary>
    /// Creates a result object array with size assigned at object generation.
    /// </summary>
    /// <returns>The generated result object array.</returns>
    public ResultObject[] GenerateNewSolution()
    {
        GeneratePlacementPoints();
        ResultObject[] newResults = new ResultObject[_size];
        Array.Clear(newResults, 0, newResults.Length);
        foreach (ResultObject resultObject in _config.ResultObjects)
        {
            for (int i = 0; i < resultObject.percentage; i++)
            {
                PlaceResult(new ResultObject(resultObject), i, ref newResults);
            }
        }

        return newResults;
    }

    /// <summary>
    /// Places the single result into the results array. Uses cuckoo replacement. While not deterministic, for this problem it works in an acceptable amount of time.
    /// Approximate run time for 100 iterations based on unit testing : .003 seconds.
    /// </summary>
    /// <param name="result">Result to place.</param>
    /// <param name="place">Order of the result in its own hierarchy. For example a 13 percentage result can have order from 0 up to 12.</param>
    /// <param name="newResults">Array to place into.</param>
    private void PlaceResult(ResultObject result, int place, ref ResultObject[] newResults)
    {
        if (result.resultNumber > _placementPoints.Count - 1)
        {
            Debug.LogError($"1-{result.resultNumber} in {_placementPoints.Count}");
        }

        if (place > _placementPoints[result.resultNumber].Length - 1)
        {
            Debug.LogError($"2-{place} in {_placementPoints[result.resultNumber].Length}");
        }

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

    /// <summary>
    /// Helper function generating placement points for each different result object that is later used for generating result object arrays.
    /// </summary>
    private void GeneratePlacementPoints()
    {
        foreach (ResultObject resultObject in _config.ResultObjects)
        {
            float frequency = _size / (float) resultObject.percentage;
            int[] points = new int[resultObject.percentage];
            for (int i = 0; i < resultObject.percentage; i++)
            {
                points[i] = (int) (frequency * i);
            }

            _placementPoints.Add(points);
        }
    }

    /// <summary>
    /// Helper function that, given the index a result was taken out of, returns its gap number it should fit in.
    /// </summary>
    /// <param name="res">The object that was popped out of array.</param>
    /// <param name="oldPlacement">The object's old placement.</param>
    /// <returns></returns>
    private int GetResultPlacement(ResultObject res, int oldPlacement)
    {
        int[] limits = _placementPoints[res.resultNumber];
        int place = -1;
        foreach (int limit in limits)
        {
            if (limit <= oldPlacement)
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

    public ResultConfig GetConfig()
    {
        return _config;
    }

    public List<int[]> GetPlacementPoints()
    {
        return _placementPoints;
    }
}