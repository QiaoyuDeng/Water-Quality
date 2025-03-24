using UnityEngine;
using System.Collections;

public class MenuIntroController : MonoBehaviour
{
    public GameObject nearMenu;

    [Header("Dropdown Menus")]
    public GameObject farmsizeDropMenu;
    public GameObject scenaryDropMenu;
    public GameObject periodDropMenu;

    // 把所有菜单放到数组中，方便批量处理
    private GameObject[] allDropdowns;

    //public GameObject[] tooltips;
    //public AudioClip[] audioClips;
    //public AudioSource audioSource;

    void Start()
    {
        // 初始化下拉菜单列表
        allDropdowns = new GameObject[] { farmsizeDropMenu, scenaryDropMenu, periodDropMenu };
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
