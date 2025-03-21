using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;

public class SceneGuide : MonoBehaviour
{
    private bool hasStarted = false;
    [System.Serializable]
    public class ObjectInfo
    {
        public GameObject targetObject;  // 目标物体
        public string objectName;        // 物体名称
        public Material highlightMaterial; // 高亮材质
        public string description; // 物体介绍
    }

    public ObjectInfo[] objectsToIntroduce; // 需要介绍的物体列表
    public TextMeshProUGUI nameText;        // UI 显示物体名称
    public TextMeshProUGUI descriptionText; // UI 显示物体介绍
    public float highlightDuration = 10f;   // 高亮持续时间
    private Material[] originalMaterials;   // 存储物体的原始材质

    public GameObject startBoard;
    public GameObject backgroundBoard;
    public GameObject focus;

    // 保存物体的原始位置
    //private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();

    void Start() 
    {
        if (backgroundBoard != null) { 
            backgroundBoard.SetActive(false);
        }
    }
    public void StartGuide()
    {
        hasStarted = true; 
        startBoard.SetActive(false);

        if (backgroundBoard != null)
        {
            backgroundBoard.SetActive(true);
        }


        BeginSceneGuide();
        
    }

    void BeginSceneGuide()
    {
        //Debug.Log("物体列表大小: " + objectsToIntroduce.Length); // 检查数组大小
        originalMaterials = new Material[objectsToIntroduce.Length]; // 确保数组大小匹配

        //// 存储物体的原始位置
        //foreach (var obj in objectsToIntroduce)
        //{
        //    originalPositions[obj.targetObject] = obj.targetObject.transform.position;
        //}

        //下面这个是异步函数，会依赖前面的初始化
        StartCoroutine(IntroduceObjects());
    }

    IEnumerator IntroduceObjects()
    {
        for (int i = 0; i < objectsToIntroduce.Length; i++)
        {
            ObjectInfo obj = objectsToIntroduce[i];

            Renderer objRenderer = obj.targetObject.GetComponent<Renderer>();
            Transform objTransform = obj.targetObject.transform;

            if (objRenderer != null)
            {
                Debug.Log(obj.objectName + " 的 Renderer 组件获取成功！");
                // 记录原始材质
                originalMaterials[i] = objRenderer.material;
                Debug.Log(obj.objectName + " 的原始材质: " + originalMaterials[i].name);

                // 设置高亮材质
                objRenderer.material = obj.highlightMaterial;
                Debug.Log(obj.objectName + " 已设置为高亮材质: " + obj.highlightMaterial.name);
            }


            // 仅显示物体名称
            nameText.transform.position = obj.targetObject.transform.position + new Vector3(0, 0.2f, 0);
            Debug.Log("物体名称位置：" + nameText.transform.position);
            nameText.text = "<b>" + obj.objectName + "</b>";
            nameText.fontSize = 5f; // 设置字体大小为 5
            nameText.color = Color.white; // 设置字体颜色为红色

            descriptionText.transform.position = obj.targetObject.transform.position + new Vector3(0, 0.12f, 0);
            descriptionText.text = "<b>" + obj.description + "</b>";  // 显示物体描述
            descriptionText.fontSize = 4f;
            descriptionText.color = Color.black;

            // ✅ 调整背景板，使其在 `nameText` 和 `descriptionText` 后面
            if (backgroundBoard != null)
            {
                backgroundBoard.transform.position = nameText.transform.position + new Vector3(0, 0.16f, 0); // ✅ 让背景板稍微靠后
                //backgroundBoard.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // ✅ 调整大小，使其适配文本
            }

            // 计算 `Text` 指向 `focus` 的方向
            Vector3 toFocus = (focus.transform.position - nameText.transform.position).normalized;

            // 计算 `Text` 当前的朝向
            Vector3 textForward = nameText.transform.forward;

            // 判断 `Text` 是否需要旋转 180°
            bool isFacingAway = Vector3.Dot(textForward, toFocus) > 0;

            if (isFacingAway)
            {
                // ✅ 让 `Text` 旋转 180°，确保正面朝向 `focus`
                nameText.transform.rotation = Quaternion.Euler(0, nameText.transform.rotation.eulerAngles.y + 180, 0);
                descriptionText.transform.rotation = Quaternion.Euler(0, descriptionText.transform.rotation.eulerAngles.y + 180, 0);
                backgroundBoard.transform.rotation = Quaternion.Euler(0, backgroundBoard.transform.rotation.eulerAngles.y + 180, 0);
            }



            // 等待 2 秒后介绍下一个物体
            yield return new WaitForSeconds(5f);

            // 取消高亮，恢复原始材质
            if (objRenderer != null)
            {
                objRenderer.material = originalMaterials[i];
            }

        }

        EndIntroduction();
    }

    // 介绍结束后清空文本
    void EndIntroduction()
    {
        nameText.text = "";
        descriptionText.text = "";

        if (backgroundBoard != null)
        {
            backgroundBoard.SetActive(false); // ✅ 结束介绍时隐藏背景板
        }

        Debug.Log("场景介绍结束");
    }
}