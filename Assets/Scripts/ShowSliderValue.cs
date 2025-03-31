using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

public class ShowSliderDayValue : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro textMesh = null;

    [SerializeField]
    private int maxDay = 7;
    public GameObject sliderPanel;

    private bool isDragging = false;  // 👈 添加拖动状态标记

    private void Awake()
    {
        Debug.Log("[Debug] Awake 被调用");

        // 自动尝试查找 TextMeshPro 组件
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshPro>();
            Debug.Log("[Debug] 自动获取 TextMeshPro：" + (textMesh != null ? "成功" : "失败"));
        }

        // ✅ 添加监听拖动开始和结束
        var slider = sliderPanel.GetComponentInChildren<PinchSlider>();
        if (slider == null)
        {
            Debug.LogError("[Debug] 没有找到 PinchSlider！");
        }
        else
        {
            Debug.Log("[Debug] 找到了 PinchSlider：" + slider.gameObject.name);
        }

        if (slider != null)
        {
            slider.OnInteractionStarted.AddListener((e) =>
            {
                isDragging = true;
                Debug.Log("[Debug] 拖动开始，isDragging = true");
            });

            slider.OnInteractionEnded.AddListener((e) =>
            {
                isDragging = false;
                Debug.Log("[Debug] 拖动结束，isDragging = false");
            });

            Debug.Log("[Debug] 已成功注册拖动监听器");
        }

    }

    public void OnSliderUpdated(SliderEventData eventData)
    {
        if (textMesh == null)
        {
            Debug.LogError("[Debug] textMesh 未设置！");
            return;
        }

        if (eventData == null)
        {
            Debug.LogError("[Debug] eventData 为空！");
            return;
        }

        float rawValue = eventData.NewValue;  // 通常是 0 到 1
        int dayIndex = Mathf.FloorToInt(rawValue * (maxDay - 1));

        Debug.Log($"[Debug] 滑条原始值：{rawValue}, 计算出的 dayIndex: {dayIndex}");

        textMesh.text = $"Day {dayIndex + 1}";
    }
    public void SetMaxDay(int newMaxDay)
    {
        maxDay = newMaxDay;
        Debug.Log($"[Debug] 已更新 maxDay 为：{maxDay}");
    }

    public void ToggleSliderPanel()
    {
        // 如果滑动中正在发生，暂时不要隐藏
        if (isDragging)
        {
            Debug.LogWarning("正在拖动滑块，已阻止 sliderPanel 显隐切换！");
            return;
        }

        sliderPanel.SetActive(!sliderPanel.activeSelf);

    }
}
