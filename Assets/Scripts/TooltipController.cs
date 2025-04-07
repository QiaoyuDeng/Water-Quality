using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;


public class TooltipController : MonoBehaviour
{
    public GameObject[] tooltips; // Store all Tooltip objects
    public AudioClip[] audioClips; // Store audio files
    public AudioSource audioSource; // Audio player
    private bool isRunning = false;
    public GameObject startBoard;
    public GameObject dialogPrefab;
    public MenuIntroController MenuIntroController;
    public MRTKSceneTransition sceneTransition;
    //private float displayTime = 5f;

    private void Start()
    {
        
        foreach (GameObject tooltip in tooltips)
        {
            tooltip.SetActive(false);
        }

        // Make sure AudioSource exists
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

    }

    // This method is bound to the Start button
    public void StartTooltipSequence() 
    {
        startBoard.SetActive(false);
        if (!isRunning) // Prevent multiple starts
        {
            isRunning = true;
            StartCoroutine(ShowTooltipsSequentially());
        }
    }

    private IEnumerator ShowTooltipsSequentially()
    {
        // Ensure all tooltips are initially hidden
        foreach (GameObject tooltip in tooltips)
        {
            tooltip.SetActive(false);
        }

        // Display tooltips one by one
        for (int i = 0; i < tooltips.Length; i++)
        {
            tooltips[i].SetActive(true); // Show current tooltip

            // Play corresponding audio (if exists)
            if (audioClips.Length > i && audioClips[i] != null)
            {
                audioSource.clip = audioClips[i];
                audioSource.Play();
            }

            // After testing, this wait time can be changed to the length of the audio
            //yield return new WaitForSeconds(audioSource.clip.length); // Wait for audio to finish
            yield return new WaitForSeconds(1f);
            tooltips[i].SetActive(false); // Hide current tooltip
        }

        Debug.Log("Preparing to open dialog...");
        // After all tooltips are shown, display dialog

        Dialog myDialog = Dialog.Open(
            dialogPrefab
        );


        //if (myDialog != null)
        //{
        //    myDialog.OnClosed += result =>
        //    {
        //        switch (result.Result)
        //        {
        //            case DialogButtonType.Yes:
        //                StartCoroutine(ShowTooltipsSequentially()); // Replay
        //                break;
        //            case DialogButtonType.No:
        //                Debug.Log("MenuIntroController");
        //                MenuIntroController.StartIntro();
        //                break;
        //        }
        //    };
        //}

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
                        //MenuIntroController.StartIntro(); //菜单
                        sceneTransition.BeginTransition();
                        break;
                }
            };
        }

    }
}
