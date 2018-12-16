using UnityEngine;
using System.Collections;
using System;
using TerrainGeneration_PluginBase;
using UnityEngine.UI;
using System.Collections.Generic;

public class TerrainInputFormPanel : MonoBehaviour {

    private Dictionary<FormField, GameObject> formFieldObjects = new Dictionary<FormField, GameObject>();  

    internal void Init(TerrainInput terrainInput)
    {
        foreach(FormField formField in ((TerrainInputForm)terrainInput).FormFields)
        {
            createFormField(formField);
        }
    }

    private void createFormField(FormField formField)
    {
        GameObject layerInput = null;
        UnityEngine.Object terrainInputPrefab;
        switch (formField.Type)
        {
            case FormFieldType.Check:
                terrainInputPrefab = Resources.Load("Prefabs/TerrainInputFieldCheck", typeof(GameObject));
                layerInput = (GameObject)Instantiate(terrainInputPrefab);
                layerInput.GetComponentInChildren<Text>().text = formField.Title;
                layerInput.GetComponentInChildren<Toggle>().isOn = ((FormFieldCheck)formField).Value;
                break;
            case FormFieldType.Integer:
                terrainInputPrefab = Resources.Load("Prefabs/TerrainInputFieldInteger", typeof(GameObject));
                layerInput = (GameObject)Instantiate(terrainInputPrefab);
                layerInput.GetComponentsInChildren<Text>()[0].text = formField.Title;
                layerInput.GetComponentsInChildren<Text>()[1].text = "" + ((FormFieldInteger)formField).Value;
                layerInput.GetComponentsInChildren<Text>()[2].text = "" + ((FormFieldInteger)formField).Value;
                layerInput.GetComponentInChildren<InputField>().text = "" + ((FormFieldInteger)formField).Value;
                break;
            case FormFieldType.Number:
                terrainInputPrefab = Resources.Load("Prefabs/TerrainInputFieldNumber", typeof(GameObject));
                layerInput = (GameObject)Instantiate(terrainInputPrefab);
                layerInput.GetComponentsInChildren<Text>()[0].text = formField.Title;
                layerInput.GetComponentsInChildren<Text>()[1].text = "" + ((FormFieldNumber)formField).Value;
                layerInput.GetComponentsInChildren<Text>()[2].text = "" + ((FormFieldNumber)formField).Value;
                layerInput.GetComponentInChildren<InputField>().text = "" + ((FormFieldNumber)formField).Value;
                break;
            case FormFieldType.Options:
                terrainInputPrefab = Resources.Load("Prefabs/TerrainInputFieldOptions", typeof(GameObject));
                layerInput = (GameObject)Instantiate(terrainInputPrefab);

                List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
                foreach(string optionStr in ((FormFieldOptions)formField).Options)
                {
                    options.Add(new Dropdown.OptionData(optionStr));
                }

                layerInput.GetComponentInChildren<Dropdown>().options.Clear();
                layerInput.GetComponentInChildren<Dropdown>().AddOptions(options);
                layerInput.GetComponentInChildren<Dropdown>().value = ((FormFieldOptions)formField).Value;
                layerInput.GetComponentInChildren<Text>().text = formField.Title;
                break;
            case FormFieldType.Text:
                terrainInputPrefab = Resources.Load("Prefabs/TerrainInputFieldText", typeof(GameObject));
                layerInput = (GameObject)Instantiate(terrainInputPrefab);
                layerInput.GetComponentsInChildren<Text>()[0].text = formField.Title;
                layerInput.GetComponentsInChildren<Text>()[1].text = "" + ((FormFieldText)formField).Value;
                layerInput.GetComponentsInChildren<Text>()[2].text = "" + ((FormFieldText)formField).Value;
                layerInput.GetComponentInChildren<InputField>().text = "" + ((FormFieldText)formField).Value;
                break;
        }

        formFieldObjects.Add(formField, layerInput);
        layerInput.transform.SetParent(gameObject.transform, false);
    }

    internal void FillInputValues()
    {
        foreach(FormField formField in formFieldObjects.Keys)
        {
            GameObject layerInput = formFieldObjects[formField];
            switch (formField.Type)
            {
                case FormFieldType.Check:
                    ((FormFieldCheck)formField).Value = layerInput.GetComponentInChildren<Toggle>().isOn;
                    break;
                case FormFieldType.Integer:
                    ((FormFieldInteger)formField).Value = Int32.Parse(layerInput.GetComponentInChildren<InputField>().text);
                    break;
                case FormFieldType.Number:
                    ((FormFieldNumber)formField).Value = Double.Parse(layerInput.GetComponentInChildren<InputField>().text);
                    break;
                case FormFieldType.Options:
                    ((FormFieldOptions)formField).Value = layerInput.GetComponentInChildren<Dropdown>().value;
                    break;
                case FormFieldType.Text:
                    ((FormFieldText)formField).Value = layerInput.GetComponentInChildren<InputField>().text;
                    break;
            }
        }
    }
}
