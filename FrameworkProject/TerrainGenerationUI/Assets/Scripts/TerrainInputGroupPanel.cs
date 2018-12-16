using UnityEngine;
using System.Collections.Generic;
using TerrainGeneration_PluginBase;
using UnityEngine.UI;
using System;

public class TerrainInputGroupPanel : MonoBehaviour {

    public GameObject TerrainInputDropdown;
    public GameObject ContentPanel;

    private GameObject currentLayerInput;
    private Dictionary<TerrainInput, GameObject> terrainInputObjects = new Dictionary<TerrainInput, GameObject>();

    internal void Init(TerrainInput terrainInput)
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        List<GameObject> layerInputs = new List<GameObject>();

        foreach(TerrainInput terrainInput_ in ((TerrainInputGroup)terrainInput).ChildInputs)
        {
            createTerrainInput(terrainInput_, options, layerInputs);
        }

        TerrainInputDropdown.GetComponent<Dropdown>().ClearOptions();
        TerrainInputDropdown.GetComponent<Dropdown>().AddOptions(options);

        TerrainInputDropdown.GetComponent<Dropdown>().onValueChanged.AddListener((idx) => selectTab(idx, layerInputs));

        if(options.Count > 0)
        {
            selectTab(0, layerInputs);
        }
    }

    private void createTerrainInput(TerrainInput terrainInput, List<Dropdown.OptionData> options, List<GameObject> layerInputs)
    {

        options.Add(new Dropdown.OptionData(terrainInput.Title));
        GameObject layerInput = null;

        switch (terrainInput.Type)
        {
            case InputType.Group:
                {
                    UnityEngine.Object terrainInputPrefab = Resources.Load("Prefabs/TerrainInputGroupPanel", typeof(GameObject));
                    layerInput = (GameObject)Instantiate(terrainInputPrefab);
                    layerInput.GetComponent<TerrainInputGroupPanel>().Init(terrainInput);
                }
                break;
            case InputType.Form:
                {
                    UnityEngine.Object terrainInputPrefab = Resources.Load("Prefabs/TerrainInputFormPanel", typeof(GameObject));
                    layerInput = (GameObject)Instantiate(terrainInputPrefab);
                    layerInput.GetComponent<TerrainInputFormPanel>().Init(terrainInput);
                }
                break;
            case InputType.Sketch:
                {
                    UnityEngine.Object terrainInputPrefab = Resources.Load("Prefabs/TerrainInputSketchPanel", typeof(GameObject));
                    layerInput = (GameObject)Instantiate(terrainInputPrefab);
                    layerInput.GetComponent<TerrainInputSketchPanel>().Init(terrainInput);
                }
                break;
        }

        terrainInputObjects.Add(terrainInput, layerInput);
        layerInputs.Add(layerInput);
    }

    private void selectTab(int idx, List<GameObject> layerInputs)
    {
        selectTab(layerInputs[idx]);
    }

    private void selectTab(GameObject layerInput)
    {
        if (currentLayerInput != null)
            currentLayerInput.transform.SetParent(null, false);

        layerInput.transform.SetParent(ContentPanel.transform, false);
        currentLayerInput = layerInput;
    }

    internal void FillInputValues()
    {
        foreach (TerrainInput terrainInput in terrainInputObjects.Keys)
        {
            switch (terrainInput.Type)
            {
                case InputType.Group:
                    terrainInputObjects[terrainInput].GetComponent<TerrainInputGroupPanel>().FillInputValues();
                    break;
                case InputType.Form:
                    terrainInputObjects[terrainInput].GetComponent<TerrainInputFormPanel>().FillInputValues();
                    break;
                case InputType.Sketch:
                    terrainInputObjects[terrainInput].GetComponent<TerrainInputSketchPanel>().FillInputValues();
                    break;
            }
        }
    }

    internal void DestroyChildren()
    {
        foreach (TerrainInput terrainInput in terrainInputObjects.Keys)
        {
            GameObject terrainInputObject = terrainInputObjects[terrainInput];

            if (terrainInput.Type.Equals(InputType.Group))
            {
                terrainInputObject.GetComponent<TerrainInputGroupPanel>().DestroyChildren();
            }

            terrainInputObject.transform.parent = null;
            Destroy(terrainInputObject);
        }
    }
}
