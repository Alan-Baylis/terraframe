using UnityEngine;
using System.Collections;
using System;
using TerrainGeneration_Core;
using TerrainGeneration_PluginBase;
using System.Collections.Generic;
using UnityEngine.UI;

public class TerrainInputPanel : MonoBehaviour {

    public GameObject TabPanel;
    public GameObject ContentPanel;

    private GameObject layerButton;
    private TerrainPipelineLayer terrainPipelineLayer { get { return layerButton.GetComponent<LayerButton>().TerrainPipelineLayer; } }
    private GameObject currentLayer;
    private bool firstLayout = true;
    private Dictionary<TerrainInput, GameObject> terrainInputObjects = new Dictionary<TerrainInput, GameObject>();
    private Dictionary<TerrainOutput, GameObject> terrainOutputObjects = new Dictionary<TerrainOutput, GameObject>();
    private List<GameObject> tabButtons = new List<GameObject>();
    private GameObject curTabButton;

    public void Init(GameObject layerButton, List<TerrainInput> terrainInputs, List<TerrainOutput> terrainOutputs)
    {
        this.layerButton = layerButton;

        if ((terrainInputs == null || terrainInputs.Count == 0) && (terrainOutputs == null || terrainOutputs.Count == 0))
        {
            UnityEngine.Object messagePrefab = Resources.Load("Prefabs/EmptyInputMessage", typeof(GameObject));
            GameObject message = (GameObject)Instantiate(messagePrefab);
            message.transform.SetParent(ContentPanel.transform, false);
        }
        else
        {
            if (terrainOutputs != null)
            {
                foreach (TerrainOutput terrainOutput in terrainOutputs)
                {
                    createTerrainOutput(terrainOutput);
                }
            }

            if (terrainInputs != null)
            {
                foreach (TerrainInput terrainInput in terrainInputs)
                {
                    createTerrainInput(terrainInput);
                }
            }
        }

        if (tabButtons != null && curTabButton != null)
        {
            foreach (GameObject button_ in tabButtons)
            {
                if (button_ == curTabButton)
                {
                    button_.GetComponent<Image>().sprite = Resources.Load<Sprite>("Graphics/TabButton");
                }
                else
                {
                    button_.GetComponent<Image>().sprite = Resources.Load<Sprite>("Graphics/TabButton_inactive");
                }
            }
        }
    }

    private void createTerrainInput(TerrainInput terrainInput)
    {
        UnityEngine.Object tabButtonPrefab = Resources.Load("Prefabs/TabButton", typeof(GameObject));
        GameObject tabButton = (GameObject)Instantiate(tabButtonPrefab);
        tabButton.transform.SetParent(TabPanel.transform, false);

        System.Console.WriteLine("Input is " + terrainInput.Title);
        tabButton.GetComponentInChildren<Text>().text = terrainInput.Title;

        tabButtons.Add(tabButton);

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

        tabButton.GetComponent<Button>().onClick.AddListener(() => selectTab(layerInput, tabButton));
        terrainInputObjects.Add(terrainInput, layerInput);

        if (firstLayout)
        {
            firstLayout = false;
            selectTab(layerInput, tabButton);
        }
    }

    private void createTerrainOutput(TerrainOutput terrainOutput)
    {
        if (terrainOutput.Type.Equals(OutputType.Object))
            return;
        UnityEngine.Object tabButtonPrefab = Resources.Load("Prefabs/TabButtonOutput", typeof(GameObject));
        GameObject tabButton = (GameObject)Instantiate(tabButtonPrefab);
        tabButton.transform.SetParent(TabPanel.transform, false);

        System.Console.WriteLine("Output is " + terrainOutput.Title);
        tabButton.GetComponentInChildren<Text>().text = terrainOutput.Title;

        tabButtons.Add(tabButton);

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

        tabButton.GetComponent<Button>().onClick.AddListener(() => selectTab(layerOutput, tabButton));
        terrainOutputObjects.Add(terrainOutput, layerOutput);

        if (firstLayout)
        {
            firstLayout = false;
            selectTab(layerOutput, tabButton);
        }
    }

    private void selectTab(GameObject layerInOut, GameObject button)
    {
        curTabButton = button;
        if (tabButtons != null)
        {
            foreach (GameObject button_ in tabButtons)
            {
                if (button_ == button)
                {
                    button_.GetComponent<Image>().sprite = Resources.Load<Sprite>("Graphics/TabButton");
                }
                else
                {
                    button_.GetComponent<Image>().sprite = Resources.Load<Sprite>("Graphics/TabButton_inactive");
                }
            }
        }

        if (currentLayer != null)
            currentLayer.transform.SetParent(null, false);

        layerInOut.transform.SetParent(ContentPanel.transform, false);
        currentLayer = layerInOut;
    }

    internal void FillInputValues()
    {
        foreach(TerrainInput terrainInput in terrainInputObjects.Keys)
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
        foreach(TerrainInput terrainInput in terrainInputObjects.Keys)
        {
            GameObject terrainInputObject = terrainInputObjects[terrainInput];

            if (terrainInput.Type.Equals(InputType.Group))
            {
                terrainInputObject.GetComponent<TerrainInputGroupPanel>().DestroyChildren();
            }

            terrainInputObject.transform.parent = null;
            Destroy(terrainInputObject);
        }

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
