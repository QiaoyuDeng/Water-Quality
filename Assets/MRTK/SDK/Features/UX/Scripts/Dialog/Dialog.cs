﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public abstract class Dialog : MonoBehaviour
    {
        /// <summary>
        /// The current state of the Dialog.
        /// </summary>
        public DialogState State { get; set; } = DialogState.Uninitialized;

        /// <summary>
        /// Called after user has clicked a button and the dialog has finished closing
        /// </summary>
        public Action<DialogResult> OnClosed;

        protected DialogResult result;
        /// <summary>
        /// Can be used to monitor result instead of events
        /// </summary>
        public DialogResult Result => result;

        protected void Launch(DialogResult newResult)
        {
            if (State != DialogState.Uninitialized)
            {
                return;
            }

            result = newResult;
            StartCoroutine(RunDialogOverTime());
        }

        /// <summary>
        /// Opens dialog, waits for input, then closes
        /// </summary>
        protected IEnumerator RunDialogOverTime()
        {
            // Create buttons and set up message
            GenerateButtons();
            SetTitleAndMessage();
            FinalizeLayout();

            // Open dialog
            State = DialogState.Opening;
            yield return StartCoroutine(OpenDialog());
            State = DialogState.WaitingForInput;
            // Wait for input
            while (State == DialogState.WaitingForInput)
            {
                UpdateDialog();
                yield return null;
            }
            // Close dialog
            State = DialogState.Closing;
            yield return StartCoroutine(CloseDialog());
            State = DialogState.Closed;
            // Callback
            OnClosed?.Invoke(result);
            // Wait a moment to give scripts a chance to respond
            yield return null;
            // Destroy ourselves
            Destroy(gameObject);
            yield break;
        }

        /// <summary>
        /// Opens the dialog - state must be set to WaitingForInput afterwards
        /// Overridden in inherited class.
        /// Perform any change here that you'd like to have when the dialog opened.
        /// </summary>
        protected virtual IEnumerator OpenDialog()
        {
            yield break;
        }

        /// <summary>
        /// Closes the dialog - state must be set to Closed afterwards
        /// </summary>
        protected virtual IEnumerator CloseDialog()
        {
            yield break;
        }

        /// <summary>
        /// Perform any updates (animation, tagalong, etc) here
        /// This will be called every frame while waiting for input
        /// </summary>
        protected virtual void UpdateDialog()
        {
            return;
        }

        /// <summary>
        /// Generates buttons - Must parent them under buttonParent!
        /// </summary>
        protected abstract void GenerateButtons();

        /// <summary>
        /// This is called after the buttons are generated and
        /// the title and message have been set.
        /// Perform here any operations that you'd like
        /// Lays out the buttons on the dialog
        /// E.g. using an ObjectCollection
        /// </summary>
        protected abstract void FinalizeLayout();

        /// <summary>
        /// Set the title and message using the result
        /// E.g. using TextMesh components 
        /// </summary>
        protected abstract void SetTitleAndMessage();

        /// <summary>
        /// Function to destroy the Dialog.
        /// </summary>
        public abstract void DismissDialog();

        ///////////////////////new definition////////////////////////
        /// <summary>
        /// Opens a dialog using only the prefab's internal settings (title, message, buttons, labels).
        /// </summary>
        /// <param name="dialogPrefab">Dialog prefab that already has all content configured.</param>
        public static Dialog Open(GameObject dialogPrefab)
        {
            GameObject dialogGO = GameObject.Instantiate(dialogPrefab);
            Dialog dialog = dialogGO.GetComponent<Dialog>();

            // ✅ 创建空的 DialogResult，让它保留原 prefab 的内容
            DialogResult result = new DialogResult
            {
                // 什么都不设置，让 DialogShell 用 prefab 内部设置的 title/description/button label
            };

            dialog.Launch(result);
            return dialog;
        }

        /// <summary>
        /// Instantiates a dialog and passes it a result
        /// </summary>
        /// <param name="dialogPrefab">Dialog prefab</param>
        /// <param name="result">DialogResult class object which contains information such as title and description text</param>
        public static Dialog Open(GameObject dialogPrefab, DialogResult result)
        {
            GameObject dialogGo = GameObject.Instantiate(dialogPrefab) as GameObject;
            Dialog dialog = dialogGo.GetComponent<Dialog>();

            dialog.Launch(result);
            return dialog;
        }

        /// <summary>
        /// Instantiates a dialog and passes a generated result
        /// </summary>
        /// <param name="dialogPrefab">Dialog prefab</param>
        /// <param name="buttons">button configuration type which is defined in DialogButtonType enum</param>
        /// <param name="title">Title text of the dialog</param>
        /// <param name="message">Description text of the dialog</param>
        /// <param name="variable">Object with additional variable</param>
        public static Dialog Open(GameObject dialogPrefab, DialogButtonType buttons, string title, string message, bool placeForNearInteraction, System.Object variable = null)
        {
            GameObject dialogGameObject = GameObject.Instantiate(dialogPrefab) as GameObject;

            if (placeForNearInteraction == true)
            {
                // For HoloLens 2, place the dialog at 45cm from the user for the near hand interactions.
                // Size is maintained by ConstantViewSize solver
                Follow followSolver = dialogGameObject.GetComponent<Follow>();
                followSolver.MinDistance = 0.3f;
                followSolver.MaxDistance = 0.9f;
                followSolver.DefaultDistance = 0.7f;
            }
            else
            {
                // For HoloLens 1 and other platforms, place the dialog for far interactions with gaze or pointers.
                // Size is maintained by ConstantViewSize solver
                Follow followSolver = dialogGameObject.GetComponent<Follow>();
                followSolver.MinDistance = 1.5f;
                followSolver.MaxDistance = 2.0f;
                followSolver.DefaultDistance = 1.8f;
            }

            Dialog dialog = dialogGameObject.GetComponent<Dialog>();

            DialogResult result = new DialogResult
            {
                Buttons = buttons,
                Title = title,
                Message = message,
                Variable = variable
            };

            dialog.Launch(result);
            return dialog;
        }
    }
}