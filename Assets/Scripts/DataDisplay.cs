using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using ChartAndGraph; // 加在顶部
using System.Collections.Generic; // Add this line


public class DataDisplay : MonoBehaviour
{
    public CSVReader csvReader;
    public string columnName = "5ML_LightRainfall_IrrigationChannel";
    public PinchSlider pinchSlider; // MRTK 的 Slider
    public TextMeshProUGUI valueDisplayText;
    //public GameObject barChartObject;  // 在 Inspector 中拖入你的图表
    private BarChart barChart;         // 真正用的组件


    void Start()
    {
        //Debug.Log($"🧠 DataDisplay Start() on {gameObject.name}");

        //if (valueDisplayText == null)
        //    Debug.LogError($"❌ {gameObject.name} 的 valueDisplayText 是 null！");
        //else
        //    Debug.Log($"✅ {gameObject.name} 正确绑定了 ValueText：{valueDisplayText.name}");



        //if (barChartObject == null)
        //{
        //    Debug.LogError("❌ barChartObject 没有被拖入！");
        //}

        //barChart = barChartObject.GetComponent<BarChart>();

        //if (barChart == null)
        //{
        //    Debug.LogError("❌ 无法从 barChartObject 获取 BarChart 组件！");
        //}
        //else
        //{
        //    Debug.Log("✅ 成功获取 BarChart 组件！");
        //}
        Debug.Log("CSV Loaded: " + csvReader.rowData.Count + " rows");

        //// 监听滑条变化
        //pinchSlider.OnValueUpdated.AddListener((SliderEventData data) =>
        //{
        //    int dayIndex = Mathf.FloorToInt(data.NewValue * 6); // 0-1 映射到 0-6
        //    string value = csvReader.GetValue(dayIndex, columnName);
        //    // 只打印，不再设置 UI 文本
        //    Debug.Log($"Day {dayIndex + 1} {columnName} = {value}");

        //    // 更新文本显示
        //    if (valueDisplayText != null)
        //    {
        //        valueDisplayText.text = $"{value}";
        //    }

        //});
        ////// 启动时也刷新一次
        //RefreshDisplay();
    }
    public void RebindSliderEvent()
    {
        Debug.Log($"🔥 调用 RebindSliderEvent 时，valueDisplayText = {valueDisplayText}");
        Debug.Log($"🧩 调用 RefreshDisplay() 的对象是：{gameObject.name}, text 组件为：{valueDisplayText}");
        var textRef = valueDisplayText;

        // 先移除所有旧的监听器
        pinchSlider.OnValueUpdated.RemoveAllListeners();

        // 重新绑定监听器，使用最新的 columnName
        pinchSlider.OnValueUpdated.AddListener((SliderEventData data) =>
        {
            int dayIndex = Mathf.FloorToInt(data.NewValue * 6);
            string value = csvReader.GetValue(dayIndex, columnName);
            Debug.Log($"🔁 [滑条更新] Day {dayIndex + 1} {columnName} = {value}");

            if (textRef != null)
                textRef.text = $"{value}";
        });

        Debug.Log("✅ 重新绑定滑条监听器！");
    }

    public void SetBarChart(BarChart chart)
    {
        barChart = chart;
        Debug.Log("✅ MenuIntroController 传入了 BarChart：" + barChart);
    }


    // ✅ 添加这个方法，供外部调用刷新显示
    public void RefreshDisplay()
    {
        if (csvReader.rowData.Count == 0 || string.IsNullOrEmpty(columnName))
        {
            Debug.LogWarning("无法刷新：数据尚未加载或列名为空");
            return;
        }

        Debug.Log($"🔄 [RefreshDisplay] 函数开始执行，当前列名: {columnName}");
        Debug.Log($"🧩 调用 RefreshDisplay() 的对象是：{gameObject.name}, text 组件为：{valueDisplayText}");

        int dayIndex = Mathf.FloorToInt(pinchSlider.SliderValue * 6);
        string value = csvReader.GetValue(dayIndex, columnName);
        Debug.Log($"[手动刷新] Day {dayIndex + 1} {columnName} = {value}");

        if (valueDisplayText != null)
        {
            valueDisplayText.text = $"{value}";
            Debug.Log($"✅ 显示更新: {valueDisplayText.text}");
        }
        else
        {
            Debug.Log("❌ valueDisplayText 是 null！");
        }

    }

    public void UpdateBarChartForWeek()
    {
        Debug.Log("🟢 [UpdateBarChartForWeek] 函数开始执行");

        if (barChart == null)
        {
            //barChart = barChartObject.GetComponent<BarChart>();
            Debug.Log("🔄 尝试运行时获取 barChart：" + barChart);
        }

        Debug.Log("📦 rowData 当前行数：" + csvReader.rowData.Count);
        List<float> values = csvReader.GetColumnValues(columnName); // 你刚刚加的新函数
        Debug.Log($"📊 {columnName} 对应的值为：{string.Join(", ", values)}");


        for (int i = 0; i < Mathf.Min(values.Count, 7); i++) // 只取前7天
        {
            string group = $"Day {i + 1}";

            //// 确保柱状图已经有这个 group
            //if (!barChart.DataSource.HasGroup(group))
            //{
            //    barChart.DataSource.AddGroup(group);
            //}

            // 设置或滑动到对应数值（平滑动画）

            Debug.Log(string.Join(group, values[i]));
            barChart.DataSource.SetValue(group, "All", values[i]);
        }

        Debug.Log("✅ 柱状图已更新！");
    }


}
