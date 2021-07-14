/*
 * Created By Umut Zenger
 * https://github.com/ZengerU/SlotGame
 */

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Controls the game flow.
/// </summary>
public class GameController : MonoBehaviour
{
    private const string ResultKey = "results";

    private ResultQueue _futureResults = new ResultQueue();
    private ResultObject _nextResult;
    private int _spinningCount, _preStopSpinning;
    private ContraintRandomBagSolver _contraintRandomBagSolver;
    private ResultConfig _config;

    [SerializeField] private SlotController leftController, middleController, rightController;

    [SerializeField] private Button spinButton, stopButton;


    [SerializeField] private float bigPrizeEmissionAmount,
        mediumPrizeEmissionAmount,
        smallPrizeEmissionAmount,
        leftMiddleSlotStopTime = .1f;

    [SerializeField] private ParticleSystem prizeParticleSystem;
    private ParticleSystem.EmissionModule _prizeEmitter;

    #region Setup Methods

    private void Awake()
    {
        _contraintRandomBagSolver = new ContraintRandomBagSolver(100);
        _config = _contraintRandomBagSolver.GetConfig();
        LoadOrGenerateResults();
        _prizeEmitter = prizeParticleSystem.emission;
    }

    /// <summary>
    /// 100 results are generated at once, load from registry or generate new results.
    /// </summary>
    private void LoadOrGenerateResults()
    {
        string storedResults = PlayerPrefs.GetString(ResultKey, string.Empty);
        if (storedResults == string.Empty)
        {
            GenerateResults();
        }
        else
        {
            LoadResults(storedResults);
        }
    }

    /// <summary>
    /// Results have already been generated, populate future results queue.
    /// </summary>
    /// <param name="storedResults">Generated future results.</param>
    private void LoadResults(string storedResults)
    {
        foreach (char result in storedResults)
        {
            _futureResults.Enqueue(_config.ResultObjects[int.Parse(result.ToString())]);
        }
    }

    /// <summary>
    /// Generates a new result set of 100 and saves it to registry.
    /// </summary>
    private void GenerateResults()
    {
        _futureResults.Clear();
        foreach (ResultObject o in _contraintRandomBagSolver.GenerateNewSolution())
        {
            _futureResults.Enqueue(o);
        }

        SaveFutureResults();
    }

    #endregion

    #region Spin Slot Methods

    /// <summary>
    /// Dequeues the next result form future results, spins the wheels based on result configuration.
    /// </summary>
    public void Spin()
    {
        _spinningCount = 3;
        _preStopSpinning = 3;
        _nextResult = _futureResults.Dequeue();
        StartCoroutine(leftController.Spin(0, 4));
        StartCoroutine(middleController.Spin(0.1f, 4.1f));
        StartCoroutine(rightController.Spin(0.2f, 4.5f));
#if UNITY_EDITOR
        print($"{_nextResult.elements[0]} - {_nextResult.elements[1]} - {_nextResult.elements[2]}");
#endif
        SaveFutureResults();
    }

    public void StopSpin()
    {
        StartCoroutine(leftController.StopSpin(.5f, leftMiddleSlotStopTime, _nextResult.elements[0]));
        StartCoroutine(middleController.StopSpin(1f, leftMiddleSlotStopTime, _nextResult.elements[1]));
        StartCoroutine(rightController.StopSpin(1.5f, _nextResult.rewardType > 0 ? Random.value + 2 : Random.value + 1,
            _nextResult.elements[2]));
    }


    /// <summary>
    /// Called after each spinner stops, enables particles and spin button if the current active spinner count is 0.
    /// </summary>
    public void SpinnerStopped()
    {
        _spinningCount--;
        if (_spinningCount > 0) return;
        if (_nextResult.rewardType == 0)
        {
            spinButton.interactable = true;
            return;
        }

        if (_nextResult.rewardType >= 4)
        {
            _prizeEmitter.rateOverTime = bigPrizeEmissionAmount;
        }
        else if (_nextResult.rewardType >= 2)
        {
            _prizeEmitter.rateOverTime = mediumPrizeEmissionAmount;
        }
        else
        {
            _prizeEmitter.rateOverTime = smallPrizeEmissionAmount;
        }

        prizeParticleSystem.Play();
        ;
        DOVirtual.DelayedCall(
            prizeParticleSystem.main.duration + ((float) prizeParticleSystem.main.startLifetimeMultiplier),
            () => { spinButton.interactable = true; });
    }

    public void SpinnerReachedAllocatedTime()
    {
        _preStopSpinning--;
        stopButton.interactable = _preStopSpinning == 0;
    }

    #endregion

    private void SaveFutureResults()
    {
        if (_futureResults.Count == 0)
        {
            GenerateResults();
        }

        PlayerPrefs.SetString(ResultKey, _futureResults.ToString());
        PlayerPrefs.Save();
    }
}