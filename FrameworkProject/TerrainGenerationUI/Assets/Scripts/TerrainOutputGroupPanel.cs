using UnityEngine;
using System.Collections;
using TerrainGeneration_PluginBase;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class TerrainOutputGroupPanel : MonoBehaviour {

    public GameObject TerrainOutputDropdown;
    public GameObject ContentPanel;

    private GameObject currentLayerOutput;
    private Dictionary<TerrainOutput, GameObject> terrainOutputObjects = new Dictionary<TerrainOutput, GameObject>();

    internal void Init(TerrainOutput terrainOutput)
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        List<GameObject> layerOutputs = new List<GameObject>();

        foreach (TerrainOutput terrainOutput_ in ((TerrainOutputGroup)terrainOutput).ChildOutputs)
        {
            createTerrainOutput(terrainOutput_, options, layerOutputs);
        }

        TerrainOutputDropdown.GetComponent<Dropdown>().ClearOptions();
        TerrainOutputDropdown.GetComponent<Dropdown>().AddOptions(options);

        TerrainOutputDropdown.GetComponent<Dropdown>().onValueChanged.AddListener((idx) => selectTab(idx, layerOutputs));

        if (options.Count > 0)
        {
            selectTab(0, layerOutputs);
        }
    }

    private void createTerrainOutput(TerrainOutput terrainOutput, List<Dropdown.OptionData> options, List<GameObject> layerOutputs)
    {

        options.Add(new Dropdown.OptionData(terrainOutput.Title));
        GameObject layerOutput = null;

        UnityEngine.Object terrainOutputPrefab;

        switch (terrainOutput.Type)
        {
            case OutputType.Group:
                terrainOutputPrefab = Resources.Load("Prefabs/TerrainOutputGroupPanel", typeof(GameObject));
                layerOutput = (GameObject)Instantiate(terrainOutputPrefab);
                layerOutput.GetComponent<TerrainOutputGroupPanel>().Init(terrainOutput);
                break;
            case OutputType.Image:
                terrainOutputPrefab = Resources.Load("Prefabs/TerrainOutputImagePanel", typeof(GameObject));
                layerOutput = (GameObject)Instantiate(terrainOutputPrefab);
                layerOutput.GetComponent<TerrainOutputImagePanel>().Init(terrainOutput);
                break;
            case OutputType.Mesh:
                terrainOutputPrefab = Resources.Load("Prefabs/TerrainOutputMeshPanel", typeof(GameObject));
                layerOutput = (GameObject)Instantiate(terrainOutputPrefab);
                layerOutput.GetComponent<TerrainOutputMeshPanel>().Init(terrainOutput);
                break;
            case OutputType.Values:
                terrainOutputPrefab = Resources.Load("Prefabs/TerrainOutputValuesPanel", typeof(GameObject));
                layerOutput = (GameObject)Instantiate(terrainOutputPrefab);
                layerOutput.GetComponent<TerrainOutputValuesPanel>().Init(terrainOutput);
                break;
        }

        terrainOutputObjects.Add(terrainOutput, layerOutput);
        layerOutputs.Add(layerOutput);
    }

    private void selectTab(int idx, List<GameObject> layerOutputs)
    {
        selectTab(layerOutputs[idx]);
    }

    private void selectTab(GameObject layerOutput)
    {
        if (currentLayerOutput != null)
            currentLayerOutput.transform.SetParent(null, false);

        layerOutput.transform.SetParent(ContentPanel.transform, false);
        currentLayerOutput = layerOutput;
    }

    internal void DestroyChildren()
    {
        foreach (TerrainOutput terrainOutput in terrainOutputObjects.Keys)
        {
            GameObject terrainOutputObject = terrainOutputObjects[terrainOutput];

            if (terrainOutput.Type.Equals(OutputType.Group))
            {
                terrainOutputObject.GetComponent<TerrainOutputGroupPanel>().DestroyChildren();
            }

            terrainOutputObject.transform.parent = null;
            Destroy(terrainOutputObject);
        }
    }
}
