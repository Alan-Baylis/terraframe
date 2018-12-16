using UnityEngine;
using TerrainGeneration_Core;
using System.Collections.Generic;
using UnityEngine.UI;
using TerrainGeneration_PluginBase;
using System;

public class ProjectPanel : MonoBehaviour {

    private TerrainProject terrainProject;
    public GameObject layerButtonsPanel;
    public GameObject ContentPanel;
    public TerrainProjectManager terrainProjectManager;
    public GameObject MessageWindow;

    public GameObject BackButton;
    public GameObject ConfigurationButton;
    public GameObject SaveButton;
    public GameObject ProcessButton;
    public GameObject CancelConfigurationButton;
    public GameObject WaitPassive;

    private List<GameObject> layerButtons = new List<GameObject>();
    public List<TerrainPipelineLayer> configurationTerrainPipelineLayer = new List<TerrainPipelineLayer>();
    internal bool ConfigurationMode = false;
    private GameObject layerConfigurations;
    private GameObject layerInput;
    private bool processing = false;
    private bool previousProcessing = false;

    void Update()
    {
        if (processing)
        {
            previousProcessing = true;
        }
        else if(previousProcessing)
        {
            previousProcessing = false;
            refreshLayers(terrainProject.Configurations.Layers, false);
            Camera.main.GetComponent<MainScript>().TakeScreenShot();
            selectPipelineLayerInput(getPrimaryLayerButton());
        }
    }

    public void ShowProject(TerrainProject terrainProject)
    {
        this.terrainProject = terrainProject;
        terrainProject.Load();

        refreshLayers(terrainProject.Configurations.Layers, false);
        selectPipelineLayerInput(getPrimaryLayerButton());

        gameObject.SetActive(true);
    }

    private void clearLayers()
    {
        foreach(GameObject layerButton in layerButtons)
        {
            Destroy(layerButton);
        }

        layerButtons.Clear();
    }

    private void refreshLayers(List<TerrainPipelineLayer> terrainPipelineLayers, Boolean isConfiguration)
    {
        clearLayers();

        foreach(TerrainPipelineLayer terrainPipelineLayer in terrainPipelineLayers)
        {
            UnityEngine.Object layerButtonPrefab = Resources.Load("Prefabs/LayerButton", typeof(GameObject));
            UnityEngine.Object terrainIconPrefab = Resources.Load("Prefabs/TerrainButtonIcon", typeof(GameObject));
            GameObject layerButton = (GameObject)Instantiate(layerButtonPrefab);

            string layerName = "";

            Boolean isFirst = true;
            foreach (LayerType terrainPipelineLayerType in terrainPipelineLayer.LayerTypes)
            {
                if (!isFirst)
                    layerName += " + ";
                layerName += TerrainProjectManager.LayerNames[terrainPipelineLayerType];
                isFirst = false;

                GameObject terrainIcon = (GameObject)Instantiate(terrainIconPrefab);
                if (!isConfiguration && terrainProject != null && terrainProject.TerrainOutputs != null && terrainProject.TerrainOutputs.ContainsKey(terrainPipelineLayer) &&
                    terrainProject.TerrainOutputs[terrainPipelineLayer].Count > 0)
                {
                    terrainIcon.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(TerrainProjectManager.ActiveTerrainLayerIcons[terrainPipelineLayerType]);
                }
                else
                {
                    terrainIcon.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(TerrainProjectManager.InactiveTerrainLayerIcons[terrainPipelineLayerType]);
                }
                terrainIcon.transform.SetParent(layerButton.GetComponentsInChildren<RectTransform>()[6].transform);

            }
            layerButton.GetComponentInChildren<Text>().text = layerName;
            layerButton.GetComponent<LayerButton>().TerrainPipelineLayer = terrainPipelineLayer;
            layerButton.GetComponent<LayerButton>().ProjectPanel = this;

            if (!(!isConfiguration && terrainPipelineLayer.PluginId == 0))
                layerButton.transform.SetParent(layerButtonsPanel.transform);

            layerButtons.Add(layerButton);
        }
    }

    public void SaveButtonPressed()
    {
        WaitPassive.SetActive(true);
        Invoke("continueSaveButtonPressed", 0.1f);
    }

    private void continueSaveButtonPressed()
    {
        if (ConfigurationMode)
        {
            if (checkLayerRestrictions())
            {
                terrainProject.Configurations.Layers = configurationTerrainPipelineLayer;
                terrainProject.Save();
                terrainProject.Load();
                CancelConfigurationButtonPressed();
            }
        }
        else
        {
            if (layerInput != null)
            {
                layerInput.GetComponent<TerrainInputPanel>().FillInputValues();
            }

            terrainProject.Save();
            terrainProject.Load();
            WaitPassive.SetActive(false);
        }
    }

    public bool checkLayerRestrictions()
    {
        Console.WriteLine("Checking restrictions");

        Dictionary<Int32, TerrainPluginInfo> availablePluginIds = new Dictionary<Int32, TerrainPluginInfo>();

        Dictionary<TerrainPipelineLayer, List<TerrainPluginInfo>> availablePlugins = TerrainProject.AvailableTerrainPlugins;
        foreach (TerrainPipelineLayer terrainPipelineLayer in availablePlugins.Keys)
        {
            foreach (TerrainPluginInfo terrainPluginInfo in availablePlugins[terrainPipelineLayer])
            {
                availablePluginIds.Add(terrainPluginInfo.Id, terrainPluginInfo);
            }
        }

        bool arePluginsSelected = false;
        foreach(TerrainPipelineLayer terrainPipelinLayer in configurationTerrainPipelineLayer)
        {
            if (availablePluginIds.ContainsKey(terrainPipelinLayer.PluginId))
            {
                arePluginsSelected = true;
            }
            else
            {
                terrainPipelinLayer.PluginId = 0;
            }
        }

        Console.WriteLine("Are plugins selected: " + arePluginsSelected);

        if (arePluginsSelected)
        {
            Console.WriteLine("A");
            bool endOfPipeline = false;
            foreach (TerrainPipelineLayer terrainPipelinLayer in configurationTerrainPipelineLayer)
            {
                if (!availablePluginIds.ContainsKey(terrainPipelinLayer.PluginId))
                {
                    endOfPipeline = true;
                }
                else
                {
                    if (endOfPipeline)
                    {
                        WaitPassive.SetActive(false);
                        MessageWindow.GetComponent<MessageWindow>().ShowMessage("Configured layers should be moved to the beginning of the pipeline.");
                        return false;
                    }
                }
            }

            Console.WriteLine("B");
            foreach (TerrainPipelineLayer terrainPipelinLayer in configurationTerrainPipelineLayer)
            {
                if (availablePluginIds.ContainsKey(terrainPipelinLayer.PluginId))
                {
                    if (!LayerConfigurationsPanel.ArePluginRestrictionsMet(availablePluginIds[terrainPipelinLayer.PluginId], configurationTerrainPipelineLayer, terrainPipelinLayer))
                    {
                        WaitPassive.SetActive(false);
                        MessageWindow.GetComponent<MessageWindow>().ShowMessage("Some plugin restrictions aren't met.");
                        return false;
                    }

                }
            }
        }

        return true;
    }

    private TerrainGenerationTask terrainGenerationTask;
    public void ProcessButtonPressed()
    {
        TerrainGenerationTask terrainGenerationTask = new TerrainGenerationTask();
        terrainGenerationTask.TaskFinished += TerrainGenerationTask_TaskFinished;

        bool previousHasSomethingProcessed = true;
        bool hasSomethingToProcess = false;

        foreach (GameObject layerButton in layerButtons){
            if (layerButton.GetComponent<LayerButton>().TerrainPipelineLayer.HasValidPlugin)
            {
                if (layerButton.GetComponentInChildren<Toggle>().isOn)
                {
                    if (previousHasSomethingProcessed)
                    {
                        terrainGenerationTask.AddLayerToProcess(layerButton.GetComponent<LayerButton>().TerrainPipelineLayer);
                        hasSomethingToProcess = true;
                    }
                    else
                    {
                        MessageWindow.GetComponent<MessageWindow>().ShowMessage("Previous layers should be already processed.");
                        return;
                    }
                }
                else
                {
                    if (!terrainProject.TerrainOutputs.ContainsKey(layerButton.GetComponent<LayerButton>().TerrainPipelineLayer) || hasSomethingToProcess)
                        previousHasSomethingProcessed = false;
                }
            }else
            {
                previousHasSomethingProcessed = false;
            }
        }

        if (!hasSomethingToProcess)
        {
            MessageWindow.GetComponent<MessageWindow>().ShowMessage("Select some layers to proceed.");
            return;
        }

        this.terrainGenerationTask = terrainGenerationTask;
        WaitPassive.SetActive(true);
        Invoke("continueProcessing", 0.1f);
    }

    private void continueProcessing()
    {
        processing = true;
        previousProcessing = true;
        if (layerInput != null)
        {
            layerInput.GetComponent<TerrainInputPanel>().FillInputValues();
        }
        terrainProject.Save();
        terrainProject.StartTerrainGenerationTask(terrainGenerationTask);
    }

    private void TerrainGenerationTask_TaskFinished(object sender, EventArgs e)
    {
        processing = false;
    }

    public void ConfigureButtonPressed()
    {
        enterConfigurationMode();
    }

    public void BackButtonPressed()
    {
        terrainProjectManager.ShowAvailableProjects();
        gameObject.SetActive(false);
    }

    public void CancelConfigurationButtonPressed()
    {
        if (layerConfigurations != null)
            Destroy(layerConfigurations);

        BackButton.SetActive(true);
        ConfigurationButton.SetActive(true);
        ProcessButton.SetActive(true);
        CancelConfigurationButton.SetActive(false);

        ConfigurationMode = false;

        refreshLayers(terrainProject.Configurations.Layers, false);

        selectPipelineLayerInput(getPrimaryLayerButton());
    }

    private GameObject getPrimaryLayerButton()
    {
        GameObject currentLayerButton = layerButtons[0];
        int i = 0;
        foreach (TerrainPipelineLayer terrainPipelineLayer in terrainProject.Configurations.Layers)
        {
            if (terrainProject.TerrainOutputs.ContainsKey(terrainPipelineLayer) && terrainProject.TerrainOutputs[terrainPipelineLayer] != null && terrainProject.TerrainOutputs[terrainPipelineLayer].Count > 0)
                currentLayerButton = layerButtons[i];
            i++;
        }

        return currentLayerButton;
    }

    private void enterConfigurationMode()
    {
        ConfigurationMode = true;

        BackButton.SetActive(false);
        ConfigurationButton.SetActive(false);
        ProcessButton.SetActive(false);
        CancelConfigurationButton.SetActive(true);

        configurationTerrainPipelineLayer = terrainProject.Configurations.Layers;
        refreshLayers(configurationTerrainPipelineLayer, true);

        selectPipelineLayerConfigurations(layerButtons[0]);
}

    public void SelectPipelineLayer(GameObject layerButton)
    {
        if (ConfigurationMode)
        {
            selectPipelineLayerConfigurations(layerButton);
        }
        else
        {
            selectPipelineLayerInput(layerButton);
        }
    }

    private GameObject layerButtonToContinue;
    private void selectPipelineLayerInput(GameObject layerButton)
    {

        if (layerButtons != null)
        {
            foreach (GameObject layerButton_ in layerButtons)
            {
                if (layerButton_ == layerButton)
                {
                    layerButton_.GetComponent<Image>().sprite = Resources.Load<Sprite>("Graphics/LayerButton_new");
                }
                else
                {
                    layerButton_.GetComponent<Image>().sprite = Resources.Load<Sprite>("Graphics/LayerButton_inactive_new");
                }
            }
        }

        layerButtonToContinue = layerButton;
        WaitPassive.SetActive(true);
        Invoke("continueSelectPipelineLayerInput", 0.1f);
    }

    private void continueSelectPipelineLayerInput()
    { 
        if (layerInput != null)
        {
            layerInput.GetComponent<TerrainInputPanel>().FillInputValues();
            layerInput.GetComponent<TerrainInputPanel>().DestroyChildren();
            Destroy(layerInput);
        }

        UnityEngine.Object terrainInputPrefab = Resources.Load("Prefabs/TerrainInputPanel", typeof(GameObject));
        layerInput = (GameObject)Instantiate(terrainInputPrefab);
        layerInput.transform.SetParent(ContentPanel.transform, false);

        List<TerrainInput> terrainInputs = null;
        List<TerrainOutput> terrainOutputs = null;
        TerrainPipelineLayer terrainPipelineLayer = layerButtonToContinue.GetComponent<LayerButton>().TerrainPipelineLayer;

        if(terrainProject.TerrainInputs.ContainsKey(terrainPipelineLayer))
        {
            terrainInputs = terrainProject.TerrainInputs[terrainPipelineLayer];
        }

        if (terrainProject.TerrainOutputs.ContainsKey(terrainPipelineLayer))
        {
            terrainOutputs = terrainProject.TerrainOutputs[terrainPipelineLayer];
        }

        layerInput.GetComponent<TerrainInputPanel>().Init(layerButtonToContinue, terrainInputs, terrainOutputs);
        WaitPassive.SetActive(false);
    }

    private void selectPipelineLayerConfigurations(GameObject layerButton)
    {
        if (layerButtons != null)
        {
            foreach (GameObject layerButton_ in layerButtons)
            {
                if (layerButton_ == layerButton)
                {
                    layerButton_.GetComponent<Image>().sprite = Resources.Load<Sprite>("Graphics/LayerButton_new");
                }
                else
                {
                    layerButton_.GetComponent<Image>().sprite = Resources.Load<Sprite>("Graphics/LayerButton_inactive_new");
                }
            }
        }

        if (layerConfigurations != null)
            Destroy(layerConfigurations);
        UnityEngine.Object layerConfigurationsPrefab = Resources.Load("Prefabs/LayerConfigurationsPanel", typeof(GameObject));
        layerConfigurations = (GameObject)Instantiate(layerConfigurationsPrefab);
        layerConfigurations.transform.SetParent(ContentPanel.transform);
        layerConfigurations.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        layerConfigurations.GetComponent<LayerConfigurationsPanel>().InitConfigurations(layerButton, this);
    }

    GameObject movingButton;

    private void startMoveButton(GameObject pipelineLayerButtonToMove)
    {
        movingButton = pipelineLayerButtonToMove;
    }

    public GameObject MoveLayerUp(GameObject layerButton)
    {
        int idx = configurationTerrainPipelineLayer.IndexOf(layerButton.GetComponent<LayerButton>().TerrainPipelineLayer);
        if (idx > 0)
        {
            configurationTerrainPipelineLayer.Insert(idx-1, layerButton.GetComponent<LayerButton>().TerrainPipelineLayer);
            configurationTerrainPipelineLayer.RemoveAt(idx+1);
            refreshLayers(configurationTerrainPipelineLayer, true);
            SelectPipelineLayer(layerButtons[idx - 1]);
            return layerButtons[idx - 1];
        }
        return layerButtons[idx];
    }

    public GameObject MoveLayerDown(GameObject layerButton)
    {
        int idx = configurationTerrainPipelineLayer.IndexOf(layerButton.GetComponent<LayerButton>().TerrainPipelineLayer);
        if (idx < configurationTerrainPipelineLayer.Count-1)
        {
            configurationTerrainPipelineLayer.Insert(idx + 2, layerButton.GetComponent<LayerButton>().TerrainPipelineLayer);
            configurationTerrainPipelineLayer.RemoveAt(idx);
            refreshLayers(configurationTerrainPipelineLayer, true);
            SelectPipelineLayer(layerButtons[idx + 1]);
            return layerButtons[idx + 1];
        }
        return layerButtons[idx];
    }

    public GameObject RefreshAvailableLayers(GameObject layerButton)
    {
        int idx = configurationTerrainPipelineLayer.IndexOf(layerButton.GetComponent<LayerButton>().TerrainPipelineLayer);
        TerrainPipelineLayer currentTerrainPipelineLayer = configurationTerrainPipelineLayer[idx];

        List<TerrainPipelineLayer> layerTypesToRemove = new List<TerrainPipelineLayer>();

        List<LayerType> terrainPipelineLayerTypes = new List<LayerType>();
        foreach(LayerType terrainPipelineLayerType in Enum.GetValues(typeof(LayerType)))
        {
            terrainPipelineLayerTypes.Add(terrainPipelineLayerType);
        }

        int i = 0;
        foreach(TerrainPipelineLayer terrainPipelineLayer in configurationTerrainPipelineLayer)
        {
            if (i != idx)
            {
                foreach (LayerType terrainPipelineLayerType in currentTerrainPipelineLayer.LayerTypes)
                {
                    if (terrainPipelineLayer.LayerTypes.Contains(terrainPipelineLayerType))
                    {
                        terrainPipelineLayer.LayerCode = (byte)(terrainPipelineLayer.LayerCode & (~(byte)terrainPipelineLayerType));
                    }
                }

                if (terrainPipelineLayer.LayerTypes.Count == 0)
                {
                    layerTypesToRemove.Add(terrainPipelineLayer);
                }
            }
            terrainPipelineLayerTypes.RemoveAll(tt =>  terrainPipelineLayer.LayerTypes.Contains(tt));
            i++;
        }

        foreach(TerrainPipelineLayer terrainPipelineLayer in layerTypesToRemove)
        {
            configurationTerrainPipelineLayer.Remove(terrainPipelineLayer);
        }

        foreach(LayerType terrainPipelineLayer in terrainPipelineLayerTypes)
        {
            TerrainPipelineLayer newTerrainPipelineLayer = new TerrainPipelineLayer();
            newTerrainPipelineLayer.LayerCode = (byte)terrainPipelineLayer;

            configurationTerrainPipelineLayer.Add(newTerrainPipelineLayer);
        }

        refreshLayers(configurationTerrainPipelineLayer, true);
        idx = configurationTerrainPipelineLayer.IndexOf(currentTerrainPipelineLayer);

        return layerButtons[idx];
    }

    public void SetTerrainThumbnail(int[][] thumbnailData)
    {
        terrainProject.ThumbnailData = thumbnailData;
    }
}
