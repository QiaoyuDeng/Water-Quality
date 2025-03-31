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

    // 把所有菜单放到数组中，方便批量处理
    private GameObject[] allDropdowns;
    private string currentFarmSize = "5ML";
    private string currentScenario = "LightRainfall";
    private string currentTargetName = "IrrigationChannel";

    public DataDisplay idatadisplay; // 拖入 Inspector 中

    //控制dashboard显示
    public GameObject dataBoard;
    public TMPro.TMP_Text titleText;
    public TMPro.TMP_Text descriptionText;
    public TMPro.TMP_Text unitText;
    public GameObject barChartObject;  // 在 Inspector 中拖入你的图表
    private BarChart barChart;         // 真正用的组件

    //public GameObject[] tooltips;
    //public AudioClip[] audioClips;
    //public AudioSource audioSource;

    void Start()
    {
        // 初始化下拉菜单列表
        allDropdowns = new GameObject[] { farmsizeDropMenu, scenaryDropMenu, periodDropMenu };
        csvReader.ReadCSV();
    }

    public void StartIntro()
    {
        nearMenu.SetActive(true);

        //StartCoroutine(PlayMenuIntro());
    }

    // 点击按钮时触发：显示对应的菜单，隐藏其他
    public void ToggleDropdown(GameObject targetDropdown)
    {
        foreach (GameObject dropdown in allDropdowns)
        {
            if (dropdown != null)
            {
                // 只显示目标菜单，其他隐藏
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
        UpdateDataColumn(); // 联动更新
    }

    private void UpdateDataColumn()
    {
        string col = $"{currentFarmSize}_{currentScenario}_{currentTargetName}"; // 改了名字！
        idatadisplay.columnName = col;
        //Debug.Log("🟢 [UpdateBarChartForWeek] 函数开始执行");
        idatadisplay.RebindSliderEvent(); // 👈 加这行
        idatadisplay.RefreshDisplay();
        idatadisplay.SetBarChart(barChartObject.GetComponent<BarChart>());
        idatadisplay.UpdateBarChartForWeek();
        Debug.Log("✅ 当前列名更新为: " + col);
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

    //这个只更显dashboard上的固定内容，不更新数据。
    public void ShowDashboard(string farmSize, string scenario, string targetName, string title, string description, string unit)
    {    

        // 设置当前列名组合
        currentFarmSize = farmSize;
        currentScenario = scenario;
        currentTargetName = targetName;

        barChart = barChartObject.GetComponent<BarChart>();

        // 更新数据列
        UpdateDataColumn();

        // 更新标题/描述
        if (titleText != null) titleText.text = title;
        if (descriptionText != null) descriptionText.text = description;
        if (unitText != null) unitText.text = unit;

        Debug.Log("✅ 显示 Dashboard: " + title);
        // 面板
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

    //    Debug.Log("✅ Near Menu 介绍完毕！");
    //}
}
