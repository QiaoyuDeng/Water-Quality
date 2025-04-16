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
    public PinchSlider daySlider;
    public PlayNarrationManager narrationManager;
    public TextMeshProUGUI dayLabel;
    public GameObject sliderGroup;

    private string[] rainfallScenarios = { "LightRainfall", "ModerateRainfall", "HeavyRainfall" };
    private int stepsPerScenario = 7;
    private int currentScenarioIndex = 0;
    private int currentDay = -1;
    private bool isUpdatingSlider = false;
    private bool isAutoPlaying = false;

    public bool introFinished = false;

    private IEnumerator Start()
    {
        csvReader.ReadCSV();

        smallFarm.SetPumpBackReuseRate(-5);
        mediumFarm.SetPumpBackReuseRate(-10);
        largeFarm.SetPumpBackReuseRate(-20);

        if (daySlider != null)
        {
            daySlider.OnValueUpdated.AddListener((SliderEventData data) =>
            {
                OnSliderChanged(data.NewValue);
            });
        }

        yield return new WaitUntil(() => introFinished);

        if (sliderGroup != null)
        {
            sliderGroup.SetActive(true);
        }

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

            yield return StartCoroutine(PlayFromDay(0));
        }

        isAutoPlaying = false;
    }

    public void OnSliderChanged(float value)
    {
        if (isAutoPlaying || isUpdatingSlider) return;

        float snapped = Mathf.Round(value * 6) / 6f;
        int selectedDay = Mathf.Clamp(Mathf.RoundToInt(snapped * 6), 0, 6);

        if (selectedDay != currentDay)
        {
            isUpdatingSlider = true;
            daySlider.SliderValue = snapped;

            currentDay = selectedDay;
            UpdateSliderFromPlayback(currentDay);
            StopAllCoroutines();
            isAutoPlaying = false;
            StartCoroutine(PlayFromDay(currentDay));
            isUpdatingSlider = false;
        }
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
            yield return new WaitUntil(() =>
                smallFarm.isReadyForNext &&
                mediumFarm.isReadyForNext &&
                largeFarm.isReadyForNext);

            currentDay = day;
            UpdateSliderFromPlayback(day);

            smallFarm.SetReuseValues(smallVolume[day], smallOverflow[day]);
            mediumFarm.SetReuseValues(mediumVolume[day], mediumOverflow[day]);
            largeFarm.SetReuseValues(largeVolume[day], largeOverflow[day]);

            smallFarm.currentScenarioId = currentScenarioIndex;
            mediumFarm.currentScenarioId = currentScenarioIndex;
            largeFarm.currentScenarioId = currentScenarioIndex;

            smallFarm.currentDay = day;
            mediumFarm.currentDay = day;
            largeFarm.currentDay = day;

            Debug.Log($"Scenario: {rain}, Day {day + 1} | OverflowPlux => Small: {smallOverflow[day]}, Medium: {mediumOverflow[day]}, Large: {largeOverflow[day]}");

            StartCoroutine(narrationManager.PlayNarrationAndWait(currentScenarioIndex, day, 0));

            StartCoroutine(smallFarm.AnimateScenarioSilent());
            StartCoroutine(mediumFarm.AnimateScenarioSilent());
            StartCoroutine(largeFarm.AnimateScenarioSilent());

            //yield return new WaitForSeconds(2f);
            yield return new WaitUntil(() =>
                smallFarm.isReadyForNext &&
                mediumFarm.isReadyForNext &&
                largeFarm.isReadyForNext &&
                !narrationManager.audioSource.isPlaying
            );
        }
    }



    private void UpdateSliderFromPlayback(int day)
    {
        if (daySlider != null)
        {
            daySlider.SliderValue = day / 6f;
        }
        if (dayLabel != null)
        {
            dayLabel.text = $"Day {day + 1}";
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
