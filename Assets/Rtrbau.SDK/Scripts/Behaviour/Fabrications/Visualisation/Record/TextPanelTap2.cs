﻿/*==============================================================================
Author: Iñigo Fernandez del Amo - 2019
Email: inigofernandezdelamo@outlook.com
License: This code has been developed for research and demonstration purposes.

Copyright (c) 2019 Iñigo Fernandez del Amo. All Rights Reserved.
Copyright (c) 2019 Cranfield University. All Rights Reserved.
Copyright (c) 2019 Babcock International Group. All Rights Reserved.

All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.

Date: 19/11/2019
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
using Microsoft.MixedReality.Toolkit.Utilities;
#endregion NAMESPACES

namespace Rtrbau
{
    /// <summary>
    /// Describe script purpose
    /// Add links when code has been inspired
    /// </summary>
    public class TextPanelTap2 : MonoBehaviour, IFabricationable, IVisualisable, IRecordable
    {
        #region INITIALISATION_VARIABLES
        public AssetVisualiser visualiser;
        public RtrbauFabrication data;
        public Transform element;
        public Transform scale;
        #endregion INITIALISATION_VARIABLES

        #region CLASS_VARIABLES
        public Dictionary<string, GameObject> selectableRecords;
        public string selectedRecord;
        #endregion CLASS_VARIABLES

        #region FACETS_VARIABLES
        #endregion FACETS_VARIABLES

        #region GAMEOBJECT_PREFABS
        public TextMeshPro fabricationText;
        public MeshRenderer fabricationSeenPanel;
        public MeshRenderer fabricationReportedPanel;
        public Material fabricationSeenMaterial;
        public Material fabricationNonReportedMaterial;
        public Material fabricationReportedMaterial;
        public Transform recordSelectButtons;
        public GameObject recordSelectButton;
        #endregion GAMEOBJECT_PREFABS

        #region CLASS_EVENTS
        private bool fabricationCreated;
        private bool recordSelectButtonsActive;
        private bool recordSelected;
        #endregion CLASS_EVENTS

        #region MONOBEHVAIOUR_METHODS
        void Start()
        {
            if (fabricationText == null || fabricationSeenPanel == null || fabricationReportedPanel == null || fabricationSeenMaterial == null || fabricationNonReportedMaterial == null || fabricationReportedMaterial == null || recordSelectButtons == null || recordSelectButton == null)
            {
                throw new ArgumentException("DefaultNominate::Start: Script requires some prefabs to work.");
            }
        }

        void Update() { }

        void OnEnable() { }

        void OnDisable() { }

        void OnDestroy() { DestroyIt(); }
        #endregion MONOBEHVAIOUR_METHODS

        #region IFABRICATIONABLE_METHODS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetVisualiser"></param>
        /// <param name="fabrication"></param>
        /// <param name="elementParent"></param>
        /// <param name="fabricationParent"></param>
        public void Initialise(AssetVisualiser assetVisualiser, RtrbauFabrication fabrication, Transform elementParent, Transform fabricationParent)
        {
            visualiser = assetVisualiser;
            data = fabrication;
            element = elementParent;
            scale = fabricationParent;
            selectableRecords = new Dictionary<string, GameObject>();
            selectedRecord = null;
            fabricationCreated = false;
            recordSelectButtonsActive = false;
            recordSelected = false;
            Scale();
            InferFromText();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Scale()
        {
            float sX = this.transform.localScale.x / scale.transform.localScale.x;
            float sY = this.transform.localScale.y / scale.transform.localScale.y;
            float sZ = this.transform.localScale.z / scale.transform.localScale.z;

            this.transform.localScale = new Vector3(sX, sY, sZ);
        }

        /// <summary>
        /// 
        /// </summary>
        public void InferFromText()
        {
            DataFacet textfacet6 = DataFormats.TextPanelTap2.formatFacets[0];
            RtrbauAttribute attribute;

            // Check data received meets fabrication requirements
            if (data.fabricationData.TryGetValue(textfacet6, out attribute))
            {
                // Set button name to relationship name
                fabricationText.text = Parser.ParseNamingOntologyFormat(attribute.attributeName.Name());
                // Create record select button for 'true' boolean value and assign to selectable records
                selectableRecords.Add("true", CreateRecordSelectButton("true"));
                // Create record select button for 'false' boolean value and assign to selectable records
                selectableRecords.Add("false", CreateRecordSelectButton("false"));
                // Check record select buttons 
                // Set fabrication as created
                fabricationCreated = true;
            }
            else
            {
                throw new ArgumentException(data.fabricationName.ToString() + "::InferFromText: cannot implement attribute received.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnNextVisualisation()
        {
            // Check if a record has been selected
            if (recordSelected == true)
            {
                DataFacet textfacet6 = DataFormats.TextPanelTap2.formatFacets[0];
                RtrbauAttribute attribute;

                // Check data received meets fabrication requirements
                if (data.fabricationData.TryGetValue(textfacet6, out attribute))
                {
                    // Update attributeValue assigned automatically when fabrication created (InferFromText)
                    // It could not be done before for attributes values re-initialisation after fabrications creation
                    // This assigns to RtrbauElement from ElementReport through RtrbauFabrication
                    attribute.attributeValue = selectedRecord;
                    // Change button colour for user confirmation
                    fabricationReportedPanel.material = fabricationReportedMaterial;
                    // Check if all attribute values have been recorded
                    // If true, then ElementReport will input reported element into report
                    // If true, then ElementReport will change colour to reported
                    element.gameObject.GetComponent<ElementReport>().CheckAttributesReported();
                    // Deactivate record select buttons
                    DeactivateRecords();
                }
                else { }
            }
            else { }
        }
        #endregion IFABRICATIONABLE_METHODS

        #region IVISUALISABLE_METHODS
        public void LocateIt()
        {
            /// Fabrication location is managed by <see cref="ElementConsult"/>.
        }

        public void ActivateIt()
        {
            /// Fabrication activation is managed by <see cref="ElementConsult"/>.
        }

        public void DestroyIt()
        {
            Destroy(this.gameObject);
        }

        public void ModifyMaterial(Material material)
        {
            // Update button panel material
            fabricationSeenPanel.material = material;
            // Update record selects buttons panel material
            foreach (KeyValuePair<string, GameObject> recordSelectButton in selectableRecords)
            {
                recordSelectButton.Value.GetComponent<RecordSelectButton>().SeenMaterial(material);
            }
        }
        #endregion IVISUALISABLE_METHODS

        #region IRECORDABLE_METHODS
        /// <summary>
        /// Activates nominate buttons when attribute name button is <see cref="OnFocus"/>.
        /// It also triggers deactivation of other nominate buttons fabrications.
        /// </summary>
        public void ActivateRecords()
        {
            if (fabricationCreated == true)
            {
                // Call ElementReport to deactivate buttons from other record fabrications
                element.GetComponent<ElementReport>().DeactivateRecords(this.gameObject);
                // Call ElementReport to deactivate buttons from other nominate fabrications
                element.GetComponent<ElementReport>().DeactivateNominates(null);

                if (recordSelectButtonsActive == false && recordSelected == false)
                {
                    // Activate all buttons
                    foreach (KeyValuePair<string, GameObject> selectButton in selectableRecords)
                    {
                        ActivateRecordSelectButton(selectButton.Value);
                    }
                    // Check record select buttons as active
                    recordSelectButtonsActive = true;
                }
                else if (recordSelectButtonsActive == false && recordSelected == true)
                {
                    GameObject selectButton;
                    // Activate only nominated individual button
                    if (selectableRecords.TryGetValue(selectedRecord, out selectButton))
                    {
                        ActivateRecordSelectButton(selectButton);
                    }
                    // Check record select buttons as active
                    recordSelectButtonsActive = true;
                }
                else { }
            }
            else { }
        }

        /// <summary>
        /// Deactivates nominate buttons.
        /// It is also called by <see cref="ElementReport"/> to deactivate nominate buttons when others are to become active.
        /// </summary>
        public void DeactivateRecords()
        {
            if (fabricationCreated == true)
            {
                if (recordSelectButtonsActive == true && recordSelected == false)
                {
                    // Deactivate all buttons
                    foreach (KeyValuePair<string, GameObject> selectButton in selectableRecords)
                    {
                        DeactivateRecordSelectButton(selectButton.Value);
                    }
                    // Check record select buttons as deactive
                    recordSelectButtonsActive = false;
                }
                else if (recordSelectButtonsActive == true && recordSelected == true)
                {
                    GameObject selectButton;
                    // Deactivate only nominated individual button
                    if (selectableRecords.TryGetValue(selectedRecord, out selectButton))
                    {
                        DeactivateRecordSelectButton(selectButton);
                    }
                    // Check record select buttons as deactive
                    recordSelectButtonsActive = false;
                }
                else { }
            }
        }
        #endregion IRECORDABLE_METHODS

        #region CLASS_METHODS
        #region PRIVATE
        GameObject CreateRecordSelectButton(string setValue)
        {
            // Instantiate record select button
            GameObject selectButton = Instantiate(recordSelectButton);
            // Initialise record select button with corresponding nominate function
            selectButton.GetComponent<RecordSelectButton>().Initialise(SelectRecord, setValue);
            // Scale buttons to possible change in fabrications scale
            ScaleRecordSelectButton(selectButton);
            // Set tile grid object collection as button parent
            DeactivateRecordSelectButton(selectButton);
            // Return button created and modified
            return selectButton;
        }

        void ScaleRecordSelectButton(GameObject button)
        {
            // UPG: if it does not work, then try scale.transform.localScale
            float sX = button.transform.localScale.x / this.transform.localScale.x;
            float sY = button.transform.localScale.y / this.transform.localScale.y;
            float sZ = button.transform.localScale.z / this.transform.localScale.z;

            button.transform.localScale = new Vector3(sX, sY, sZ);
        }

        void ActivateRecordSelectButton(GameObject button)
        {
            // Deactivate tile grid object collection
            recordSelectButtons.gameObject.GetComponent<TileGridObjectCollection>().enabled = false;
            // Set tile grid object collection as button parent
            button.transform.SetParent(recordSelectButtons, false);
            // Set game object button to active
            button.SetActive(true);
            // Activate tile grid object collection
            recordSelectButtons.gameObject.GetComponent<TileGridObjectCollection>().enabled = true;
        }

        void DeactivateRecordSelectButton(GameObject button)
        {
            // Deactivate tile grid object collection
            recordSelectButtons.gameObject.GetComponent<TileGridObjectCollection>().enabled = false;
            // Set fabrication root as button parent
            button.transform.SetParent(this.transform, false);
            // Set game object button to deactive
            button.SetActive(false);
            // Activate tile grid object collection
            recordSelectButtons.gameObject.GetComponent<TileGridObjectCollection>().enabled = true;
        }
        #endregion PRIVATE

        #region PUBLIC
        /// <summary>
        /// 
        /// </summary>
        /// <param name="individual"></param>
        public void SelectRecord(string setValue)
        {
            // If fabrication created but individual not nominated, then nominate this individual
            if (fabricationCreated == true && recordSelected == false)
            {
                // Deactivate other buttons and update selected button material
                foreach (KeyValuePair<string, GameObject> selectButton in selectableRecords)
                {
                    if (selectButton.Key == setValue) { selectButton.Value.GetComponent<RecordSelectButton>().ReportMaterial(fabricationReportedMaterial); }
                    else { DeactivateRecordSelectButton(selectButton.Value); }
                }
                // Update button material
                fabricationReportedPanel.material = fabricationReportedMaterial;
                // Assign set value as selected
                selectedRecord = setValue;
                // Check attribute selection record
                recordSelected = true;
            }
            else if (fabricationCreated == true && recordSelected == true)
            {
                // Activate other buttons and update individual button material
                foreach (KeyValuePair<string, GameObject> selectButton in selectableRecords)
                {
                    if (selectButton.Key == setValue) { selectButton.Value.GetComponent<RecordSelectButton>().ReportMaterial(fabricationNonReportedMaterial); }
                    else { ActivateRecordSelectButton(selectButton.Value); }
                }
                // Update button material
                fabricationReportedPanel.material = fabricationNonReportedMaterial;
                // Unassign set value as selected
                selectedRecord = null;
                // Uncheck attribute selection record
                recordSelected = false;
            }
            else { }
        }
        #endregion PUBLIC
        #endregion CLASS_METHODS
    }
}
