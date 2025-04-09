using UnityEngine;
using System.Collections;
using ChartAndGraph;

public class MenuIntroController : MonoBehaviour
{
    public GameObject nearMenu;

    [Header("Dropdown Menus")]
    public GameObject farmsizeDropMenu;
    public GameObject scenaryDropMenu;
    public GameObject periodDropMenu;
    public CSVReader csvReader;

    // Put all menus into an array for batch processing
    private GameObject[] allDropdowns;
    private string currentFarmSize = "5ML";
    private string currentScenario = "LightRainfall";
    private string currentTargetName = "IrrigationChannel";

    public DataDisplay idatadisplay;

    // Control the dashboard display
    public GameObject dataBoard;
    public TMPro.TMP_Text titleText;
    public TMPro.TMP_Text descriptionText;
    public TMPro.TMP_Text unitText;
    public GameObject barChartObject;  // Drag your chart into the Inspector
    private BarChart barChart;         // Actual components in use

    //public GameObject[] tooltips;
    //public AudioClip[] audioClips;
    //public AudioSource audioSource;

    void Start()
    {
        // Initialize the dropdown menu list
        allDropdowns = new GameObject[] { farmsizeDropMenu, scenaryDropMenu, periodDropMenu };
        csvReader.ReadCSV();
    }

    public void StartIntro()
    {
        nearMenu.SetActive(true);

        //StartCoroutine(PlayMenuIntro());
    }

    // Triggered when the button is clicked: Show the corresponding menu, hide others
    public void ToggleDropdown(GameObject targetDropdown)
    {
        foreach (GameObject dropdown in allDropdowns)
        {
            if (dropdown != null)
            {
                // Only show the target menu, hide others
                dropdown.SetActive(dropdown == targetDropdown && !dropdown.activeSelf);
            }
        }
    }
    public void OnFarmSizeSelected(string size)
    {
        currentFarmSize = size;
        UpdateDataColumn();
    }

    public void OnScenarioSelected(string scenario)
    {
        currentScenario = scenario;
        UpdateDataColumn();
    }

    public void OnTargetSelected(string target)
    {
        currentTargetName = target;
        UpdateDataColumn(); // renew the data column
    }

    private void UpdateDataColumn()
    {
        string col = $"{currentFarmSize}_{currentScenario}_{currentTargetName}"; 
        idatadisplay.columnName = col;
        idatadisplay.RebindSliderEvent(); 
        idatadisplay.RefreshDisplay();
        idatadisplay.SetBarChart(barChartObject.GetComponent<BarChart>());
        idatadisplay.UpdateBarChartForWeek();
        Debug.Log("Current column name updated to: " + col);
    }

    public string GetCurrentFarmSize()
    {
        return currentFarmSize;
    }

    public string GetCurrentScenario()
    {
        return currentScenario;
    }

    public string GetCurrentTarget()
    {
        return currentTargetName;
    }

    // This only updates the fixed content (chart) on the dashboard, not the data.
    public void ShowDashboard(string farmSize, string scenario, string targetName, string title, string description, string unit)
    {

        // Set the current column name combination
        currentFarmSize = farmSize;
        currentScenario = scenario;
        currentTargetName = targetName;

        barChart = barChartObject.GetComponent<BarChart>();

        // Update data columns
        UpdateDataColumn();

        // Update title/description
        if (titleText != null) titleText.text = title;
        if (descriptionText != null) descriptionText.text = description;
        if (unitText != null) unitText.text = unit;

        Debug.Log("Show Dashboard: " + title);
        // Panel
        if (dataBoard != null) dataBoard.SetActive(true);
    }


    //private IEnumerator PlayMenuIntro()
    //{
    //    foreach (GameObject go in tooltips)
    //    {
    //        go.SetActive(false);
    //    }

    //    for (int i = 0; i < tooltips.Length; i++)
    //    {
    //        tooltips[i].SetActive(true);

    //        if (i < audioClips.Length && audioClips[i] != null)
    //        {
    //            audioSource.clip = audioClips[i];
    //            audioSource.Play();
    //            yield return new WaitForSeconds(audioSource.clip.length + 0.5f);
    //        }

    //        tooltips[i].SetActive(false);
    //    }

    //    Debug.Log("✅ Near Menu Intro Completed！");
    //}
}
