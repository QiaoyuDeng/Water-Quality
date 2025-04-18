using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;

public class FarmController : MonoBehaviour
{
    public CSVReader csvReader;

    public FarmSimulator smallFarm;
    public FarmSimulator mediumFarm;
    public FarmSimulator largeFarm;

    [Header("Rain Particle Control")]
    public ParticleSystem rain;

    [Header("Rain Audio Clips")]
    public AudioClip lightRainAudio;
    public AudioClip moderateRainAudio;
    public AudioClip heavyRainAudio;

    [Header("Overlay & Control")]
    public ScenarioOverlayController overlayController;
    public PlayNarrationManager narrationManager;
    public TextMeshProUGUI dayLabel;
    public Interactable previousButton;
    public Interactable nextButton;
    public GameObject dayControlGroup;

    private string[] rainfallScenarios = { "LightRainfall", "ModerateRainfall", "HeavyRainfall" };
    private int stepsPerScenario = 7;
    private int currentScenarioIndex = 0;
    private int currentDay = -1;
    private bool isAutoPlaying = false;
    private Coroutine playbackCoroutine;

    public bool introFinished = false;

    private IEnumerator Start()
    {
        csvReader.ReadCSV();
        if (dayControlGroup != null)
        {
            dayControlGroup.SetActive(false);
        }

        smallFarm.SetPumpBackReuseRate(-5);
        mediumFarm.SetPumpBackReuseRate(-10);
        largeFarm.SetPumpBackReuseRate(-20);

        yield return new WaitUntil(() => introFinished);

        isAutoPlaying = true;

        for (int scenarioIndex = 0; scenarioIndex < rainfallScenarios.Length; scenarioIndex++)
        {
            currentScenarioIndex = scenarioIndex;
            string rain = rainfallScenarios[scenarioIndex];

            if (overlayController != null)
            {
                string rainLabel = rain switch
                {
                    "LightRainfall" => "Scenario 1: Light Rainfall",
                    "ModerateRainfall" => "Scenario 2: Moderate Rainfall",
                    "HeavyRainfall" => "Scenario 3: Heavy Rainfall",
                    _ => "Scenario: Unknown"
                };

                AudioClip clip = rain switch
                {
                    "LightRainfall" => lightRainAudio,
                    "ModerateRainfall" => moderateRainAudio,
                    "HeavyRainfall" => heavyRainAudio,
                    _ => null
                };

                Debug.Log($"🎬 Showing rain overlay: {rainLabel}, clip: {(clip != null ? clip.name : "null")}");
                yield return StartCoroutine(overlayController.ShowScenarioText(rainLabel, clip));
                Debug.Log("✅ Finished rain overlay");
            }

            Debug.Log("🌧️ Starting rain visual effect");
            SetRainByScenario(rain);

            if (dayControlGroup != null)
            {
                dayControlGroup.SetActive(true);
            }
            playbackCoroutine = StartCoroutine(PlayFromDay(0));
            yield return playbackCoroutine;
        }

        isAutoPlaying = false;
    }

    public void GoToPreviousDay()
    {
        if (currentDay > 0)
        {
            currentDay--;
            StopAllPlayback();
            playbackCoroutine = StartCoroutine(PlayFromDay(currentDay));
        }
    }

    public void GoToNextDay()
    {
        if (currentDay < stepsPerScenario - 1)
        {
            currentDay++;
            StopAllPlayback();
            playbackCoroutine = StartCoroutine(PlayFromDay(currentDay));
        }
    }

    private void StopAllPlayback()
    {
        if (playbackCoroutine != null)
        {
            StopCoroutine(playbackCoroutine);
        }

        narrationManager.StopNarration();
        smallFarm.StopAllAnimations();
        mediumFarm.StopAllAnimations();
        largeFarm.StopAllAnimations();
    }

    public IEnumerator PlayFromDay(int startDay)
    {
        string rain = rainfallScenarios[currentScenarioIndex];
        SetRainByScenario(rain);

        List<float> smallOverflow = csvReader.GetColumnValues($"5ML_{rain}_OverflowPlux");
        List<float> mediumOverflow = csvReader.GetColumnValues($"10ML_{rain}_OverflowPlux");
        List<float> largeOverflow = csvReader.GetColumnValues($"20ML_{rain}_OverflowPlux");

        List<float> smallVolume = csvReader.GetColumnValues($"5ML_{rain}_StorageVolume");
        List<float> mediumVolume = csvReader.GetColumnValues($"10ML_{rain}_StorageVolume");
        List<float> largeVolume = csvReader.GetColumnValues($"20ML_{rain}_StorageVolume");

        for (int day = startDay; day < stepsPerScenario; day++)
        {
            currentDay = day;
            UpdateDayLabel(day);
            UpdateDayButtonStates();

            yield return StartCoroutine(PlayDay(day, rain, smallVolume, mediumVolume, largeVolume, smallOverflow, mediumOverflow, largeOverflow));
        }

        // ✅ Only trigger transition if it's the last day
        if (currentScenarioIndex < rainfallScenarios.Length - 1 && currentDay >= stepsPerScenario - 1)
        {
            currentScenarioIndex++;
            currentDay = 0;

            string nextRain = rainfallScenarios[currentScenarioIndex];
            string rainLabel = nextRain switch
            {
                "LightRainfall" => "Scenario 1: Light Rainfall",
                "ModerateRainfall" => "Scenario 2: Moderate Rainfall",
                "HeavyRainfall" => "Scenario 3: Heavy Rainfall",
                _ => "Scenario: Unknown"
            };
            AudioClip clip = nextRain switch
            {
                "LightRainfall" => lightRainAudio,
                "ModerateRainfall" => moderateRainAudio,
                "HeavyRainfall" => heavyRainAudio,
                _ => null
            };

            if (overlayController != null)
            {
                yield return StartCoroutine(overlayController.ShowScenarioText(rainLabel, clip));
            }

            playbackCoroutine = StartCoroutine(PlayScenarioFromCurrentDay());
        }
    }

    private IEnumerator PlayScenarioFromCurrentDay()
    {
        yield return PlayFromDay(currentDay);
    }

    private IEnumerator PlayDay(int day, string rain, List<float> smallVolume, List<float> mediumVolume, List<float> largeVolume, List<float> smallOverflow, List<float> mediumOverflow, List<float> largeOverflow)
    {
        yield return new WaitUntil(() =>
            smallFarm.isReadyForNext &&
            mediumFarm.isReadyForNext &&
            largeFarm.isReadyForNext);

        smallFarm.SetReuseValues(smallVolume[day], smallOverflow[day]);
        mediumFarm.SetReuseValues(mediumVolume[day], mediumOverflow[day]);
        largeFarm.SetReuseValues(largeVolume[day], largeOverflow[day]);

        smallFarm.currentScenarioId = currentScenarioIndex;
        mediumFarm.currentScenarioId = currentScenarioIndex;
        largeFarm.currentScenarioId = currentScenarioIndex;

        smallFarm.currentDay = day;
        mediumFarm.currentDay = day;
        largeFarm.currentDay = day;

        Coroutine narrationCoroutine = StartCoroutine(narrationManager.PlayNarrationAndWait(currentScenarioIndex, day, 0));
        Coroutine smallAnim = StartCoroutine(smallFarm.AnimateScenarioSilent());
        Coroutine mediumAnim = StartCoroutine(mediumFarm.AnimateScenarioSilent());
        Coroutine largeAnim = StartCoroutine(largeFarm.AnimateScenarioSilent());

        yield return new WaitUntil(() =>
            smallFarm.isReadyForNext &&
            mediumFarm.isReadyForNext &&
            largeFarm.isReadyForNext &&
            !narrationManager.audioSource.isPlaying);
    }

    private void UpdateDayLabel(int day)
    {
        if (dayLabel != null)
        {
            dayLabel.text = $"Day {day + 1}";
        }
    }

    private void UpdateDayButtonStates()
    {
        if (previousButton != null)
        {
            previousButton.IsEnabled = currentDay > 0;
        }
        if (nextButton != null)
        {
            nextButton.IsEnabled = currentDay < stepsPerScenario - 1;
        }
    }

    public void MarkIntroAsFinished()
    {
        introFinished = true;
    }

    public void SetRainByScenario(string scenario)
    {
        if (rain == null)
        {
            Debug.LogWarning("❗ Rain particle system not assigned.");
            return;
        }

        var emission = rain.emission;
        var main = rain.main;

        if (scenario == "LightRainfall")
        {
            emission.rateOverTime = 10;
            main.startSize = 0.05f;
            Debug.Log("Light rainfall started.");
        }
        else if (scenario == "ModerateRainfall")
        {
            emission.rateOverTime = 100;
            main.startSize = 0.08f;
            Debug.Log("Moderate rainfall started.");
        }
        else if (scenario == "HeavyRainfall")
        {
            emission.rateOverTime = 200;
            main.startSize = 0.1f;
            Debug.Log("Heavy rainfall started.");
        }

        rain.Play();
    }
}
