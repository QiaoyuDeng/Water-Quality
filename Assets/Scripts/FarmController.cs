using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    //public AudioSource audioSource;

    private string[] rainfallScenarios = { "LightRainfall", "ModerateRainfall", "HeavyRainfall" };
    private int stepsPerScenario = 7;

    public bool introFinished = false; // Used to control whether to play the animation

    public ScenarioOverlayController overlayController; // Reference to the overlay controller


    private IEnumerator Start()
    {
        csvReader.ReadCSV();

        // Fix the pump back reuse rate
        smallFarm.SetPumpBackReuseRate(-5);   // 5ML
        mediumFarm.SetPumpBackReuseRate(-10); // 10ML
        largeFarm.SetPumpBackReuseRate(-20);  // 20ML

        // Wait for the introduction phase to finish
        yield return new WaitUntil(() => introFinished);

        foreach (string rain in rainfallScenarios)
        {

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

                yield return StartCoroutine(overlayController.ShowScenarioText(rainLabel, clip));
            }
            // Set the rain scenario
            SetRainByScenario(rain); 

            string overflowColSmall = $"5ML_{rain}_OverflowPlux";
            string overflowColMedium = $"10ML_{rain}_OverflowPlux";
            string overflowColLarge = $"20ML_{rain}_OverflowPlux";

            string volumeColSmall = $"5ML_{rain}_StorageVolume";
            string volumeColMedium = $"10ML_{rain}_StorageVolume";
            string volumeColLarge = $"20ML_{rain}_StorageVolume";

            List<float> smallOverflow = csvReader.GetColumnValues(overflowColSmall);
            List<float> mediumOverflow = csvReader.GetColumnValues(overflowColMedium);
            List<float> largeOverflow = csvReader.GetColumnValues(overflowColLarge);

            List<float> smallVolume = csvReader.GetColumnValues(volumeColSmall);
            List<float> mediumVolume = csvReader.GetColumnValues(volumeColMedium);
            List<float> largeVolume = csvReader.GetColumnValues(volumeColLarge);

            for (int i = 0; i < stepsPerScenario; i++)
            {
                
                yield return new WaitUntil(() =>
                    smallFarm.isReadyForNext &&
                    mediumFarm.isReadyForNext &&
                    largeFarm.isReadyForNext);

                float so = GetSafeValue(smallOverflow, i, 0);
                float mo = GetSafeValue(mediumOverflow, i, 0);
                float lo = GetSafeValue(largeOverflow, i, 0);

                float sv = GetSafeValue(smallVolume, i, 0);
                float mv = GetSafeValue(mediumVolume, i, 0);
                float lv = GetSafeValue(largeVolume, i, 0);

                Debug.Log($"Scenario: {rain}, Day {i + 1} | OverflowPlux => Small: {so}, Medium: {mo}, Large: {lo}");

                smallFarm.SetReuseValues(sv, so);
                mediumFarm.SetReuseValues(mv, mo);
                largeFarm.SetReuseValues(lv, lo);

                // play animation
                smallFarm.StartCoroutine(smallFarm.AnimateScenario());
                mediumFarm.StartCoroutine(mediumFarm.AnimateScenario());
                largeFarm.StartCoroutine(largeFarm.AnimateScenario());

            }
        }
    }

    private float GetSafeValue(List<float> data, int index, float fallback)
    {
        return (data != null && index < data.Count) ? data[index] : fallback;
    }

    // Call this method from FarmIntroSequence
    public void MarkIntroAsFinished()
    {
        introFinished = true;
    }

    // Set the rainfall intensity
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

