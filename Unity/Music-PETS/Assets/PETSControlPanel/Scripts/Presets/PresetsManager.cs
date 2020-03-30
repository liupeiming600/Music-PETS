using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HololensPETS
{
    public abstract class PresetsManager : MonoBehaviour
    {
        public PresetListScriptableObject presetList;
        public string presetsPath;

        public Dropdown presetsDropdown;
        public Button loadPresetButton, deletePresetButton;

        public InputField newPresetNameInputField;
        public Button savePresetButton;

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
            savePresetButton.onClick.AddListener( SaveButtonClickedHandler );
            deletePresetButton.onClick.AddListener(DeleteButtonClickedHandler);
            loadPresetButton.onClick.AddListener( LoadButtonClickedHandler );
        }

        protected virtual void Update() {}

        protected virtual void OnEnable()
        {
            RefreshPresetDropdown();
        }

        protected virtual void OnDisable() {}

        private PresetScriptableObject GetPreset( int index )
        {
            if( MathUtils.IsInRange( index, 0, presetList.presets.Count ) )
            {
                return presetList.presets[index];
            }

            return null;
        }

        private void SavePreset( PresetScriptableObject preset, string presetFileName = null )
        {
            if( presetFileName == null )
            {
                presetFileName = preset.presetName;
                if( string.IsNullOrEmpty( presetFileName ) )
                {
                    presetFileName = "New Preset";
                }
            }

            string assetPath = presetsPath;
            if ( presetsPath[presetsPath.Length - 1] != '/' )
            {
                assetPath += "/";
            }
            assetPath += presetFileName + ".asset";

#if UNITY_EDITOR
            AssetDatabase.CreateAsset( preset, assetPath );
#endif

            presetList.presets.Add( preset );   //preset追加

#if UNITY_EDITOR
            EditorUtility.SetDirty(presetList);
#endif
        }

        private void SaveButtonClickedHandler()
        {
            PresetScriptableObject preset = GetPresetScriptableObjectInstance();
            preset.presetName = newPresetNameInputField.text;

            PopulatePresetObjectForSaving( preset );

            SavePreset( preset );

            RefreshPresetDropdown();
        }

        private void DeleteButtonClickedHandler()
        {
            int selectedPreset = presetsDropdown.value;
            presetList.presets.Remove(GetPreset(selectedPreset));

            RefreshPresetDropdown();
        }

        private void LoadButtonClickedHandler()
        {
            int selectedPreset = presetsDropdown.value;
            PresetScriptableObject presetToLoad = GetPreset( selectedPreset );
            LoadPreset( presetToLoad );
        }

        private void RefreshPresetDropdown()
        {
            //Debug.Log("Refresh: " + presetList.presets.Count);
            presetsDropdown.ClearOptions();
            
            List<string> options = new List<string>();
            for ( int i = 0; i < presetList.presets.Count; i++ )
            {
                options.Add( presetList.presets[i].presetName );
            }
            presetsDropdown.AddOptions( options );
        }

        protected abstract PresetScriptableObject GetPresetScriptableObjectInstance();

        protected abstract void PopulatePresetObjectForSaving( PresetScriptableObject preset );
        protected abstract void LoadPreset( PresetScriptableObject preset );
    }
}
