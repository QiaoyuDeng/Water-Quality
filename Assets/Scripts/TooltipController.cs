using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;


public class TooltipController : MonoBehaviour
{
    public GameObject[] tooltips; // 存放所有 Tooltip 对象
    public AudioClip[] audioClips; // 存放音频文件
    public AudioSource audioSource; // 音频播放器
    private bool isRunning = false;
    public GameObject startBoard;
    public GameObject dialogPrefab;
    public MenuIntroController MenuIntroController;

    private void Start()
    {
        
        foreach (GameObject tooltip in tooltips)
        {
            tooltip.SetActive(false);
        }

        // 确保 AudioSource 存在
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

    }

    public void StartTooltipSequence() // 这个方法绑定到 Start 按钮
    {
        startBoard.SetActive(false);
        if (!isRunning) // 避免重复启动
        {
            isRunning = true;
            StartCoroutine(ShowTooltipsSequentially());
        }
    }

    private IEnumerator ShowTooltipsSequentially()
    {
        // 确保所有的 Tooltips 初始时隐藏
        foreach (GameObject tooltip in tooltips)
        {
            tooltip.SetActive(false);
        }

        // 依次显示 Tooltips
        for (int i = 0; i < tooltips.Length; i++)
        {
            tooltips[i].SetActive(true); // 显示当前 Tooltip

            // 播放对应的音频（如果存在）
            if (audioClips.Length > i && audioClips[i] != null)
            {
                audioSource.clip = audioClips[i];
                audioSource.Play();
            }

            yield return new WaitForSeconds(audioSource.clip.length); // 等待 5 秒
            //yield return new WaitForSeconds(displayTime);
            tooltips[i].SetActive(false); // 关闭当前 Tooltip
        }

        Debug.Log("准备打开对话框...");
        // 所有 Tooltip 播放完之后，显示对话框

        Dialog myDialog = Dialog.Open(
            dialogPrefab
        );


        if (myDialog != null)
        {
            myDialog.OnClosed += result =>
            {
                switch (result.Result)
                {
                    case DialogButtonType.Yes:
                        StartCoroutine(ShowTooltipsSequentially()); // Replay
                        break;
                    case DialogButtonType.No:
                        Debug.Log("MenuIntroController");
                        MenuIntroController.StartIntro();
                        break;
                }
            };
        }

    }
}
