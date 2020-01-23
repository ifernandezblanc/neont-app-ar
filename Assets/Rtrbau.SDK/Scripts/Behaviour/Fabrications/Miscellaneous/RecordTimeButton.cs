﻿/*==============================================================================
Author: Iñigo Fernandez del Amo - 2019
Email: inigofernandezdelamo@outlook.com
License: This code has been developed for research and demonstration purposes.

Copyright (c) 2019 Iñigo Fernandez del Amo. All Rights Reserved.
Copyright (c) 2019 Cranfield University. All Rights Reserved.
Copyright (c) 2019 Babcock International Group. All Rights Reserved.

All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.

Date: 18/11/2019
==============================================================================*/

/// <summary>
/// Describe script purpose
/// Add links when code has been inspired
/// </summary>
#region NAMESPACES
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#endregion NAMESPACES

namespace Rtrbau
{ 
    public class RecordTimeButton : MonoBehaviour, IVisualisable
    {
        #region INITIALISATION_VARIABLES
        public Action<DateTimeOffset> timeRecord;
        #endregion INITIALISATION_VARIABLES

        #region CLASS_VARIABLES
        public DateTimeOffset attributeValueDateTime;
        #endregion CLASS_VARIABLES

        #region GAMEOBJECT_PREFABS
        public TextMeshPro recordText;
        public TextMeshPro clickingText;
        #endregion GAMEOBJECT_PREFABS

        #region CLASS_EVENTS
        private bool buttonCreated;
        #endregion CLASS_EVENTS

        #region MONOBEHAVIOUR_METHODS
        void Start()
        {
            if (recordText == null || clickingText == null)
            {
                throw new ArgumentException("RecordTimeButton::Start: Script requires some prefabs to work.");
            }
            else
            {
                attributeValueDateTime = default(DateTimeOffset);
                recordText.text = "Focus to input date: yyyy-MM-dd HH:mm";
                buttonCreated = true;
            }
        }

        void OnDestroy() { DestroyIt(); }
        #endregion MONOBEHAVIOUR_METHODS

        #region IVISUALISABLE_METHODS
        public void LocateIt()
        {
            /// Fabrication location is managed by <see cref="IRecordable"/>.
        }

        public void ActivateIt()
        {
            /// Fabrication activation is managed by <see cref="IRecordable"/>.
        }

        public void DestroyIt()
        {
            Destroy(this.gameObject);
        }

        public void ModifyMaterial(Material material)
        {
            /// Fabrication material modification is managed by <see cref="IRecordable"/>.
        }
        #endregion IVISUALISABLE_METHODS

        #region CLASS_METHODS
        #region PRIVATE
        #endregion PRIVATE

        #region PUBLIC
        /// <summary>
        /// Initialise <see cref="RecordTimeButton"/> using the Action that triggers the button.
        /// </summary>
        /// <param name="recordTime"></param>
        public void Initialise(Action<DateTimeOffset> recordTime)
        {
            timeRecord = recordTime;
        }


        /// <summary>
        /// Returns user recorded <see cref="DateTimeOffset"/> by text input.
        /// </summary>
        /// <returns>Returns <see cref="attributeValueDateTime"/> if parsed correctly, otherwise returns <see cref="default"/>.</returns>
        public void RecordDateTime()
        {
            if (buttonCreated == true)
            {
                if (recordText.text != null)
                {
                    if (DateTimeOffset.TryParse(recordText.text, out attributeValueDateTime))
                    {
                        timeRecord.Invoke(attributeValueDateTime);
                    }
                    else { recordText.text = "Input date as: yyyy-MM-dd HH:mm"; }
                }
                else { recordText.text = "Input date as: yyyy-MM-dd HH:mm"; }
            }
            else { }

            // Deactivate loading plate
            this.gameObject.GetComponentInParent<IElementable>().DeactivateLoadingPlate();
            // Provide instructions for user to open keyboard
            clickingText.text = "Look up to open keyboard";
        }

        /// <summary>
        /// Calls RtrbauKeyboard to open system keyboard
        /// </summary>
        public void OpenKeyboard()
        {
            // Activate Rtrbau keyboard
            RtrbauKeyboard.instance.OpenRtrbauKeyboard(TouchScreenKeyboardType.Default, recordText);
            // Activate loading plate
            this.gameObject.GetComponentInParent<IElementable>().ActivateLoadingPlate();
            // Provide instruction for user to click button
            clickingText.text = "Click to record";
        }
        #endregion PUBLIC
        #endregion CLASS_METHODS
    }
}

