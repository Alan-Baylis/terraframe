using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TerrainGeneration_Core;
using TerrainGeneration_PluginBase;
using System;

public class LayerConfigurationsPanel : MonoBehaviour {

    public GameObject LayerTypesPanel;
    public GameObject PluginsContainer;

    private GameObject layerButton;
    private TerrainPipelineLayer terrainPipelineLayer { get { return layerButton.GetComponent<LayerButton>().TerrainPipelineLayer; } }
    private ProjectPanel projectPanel;
    private List<Dropdown> layerTypeDropdowns = new List<Dropdown>();
    private LayerType lastTerrainPipelineLayerType;
    private Dictionary<GameObject, TerrainPluginInfo> pluginOptionsMap = new Dictionary<GameObject, TerrainPluginInfo>();

    public void InitConfigurations(GameObject layerButton, ProjectPanel projectPanel)
    {
        this.layerButton = layerButton;
        this.projectPanel = projectPanel;

        fillTerrainTypes();
        fillPluginList();
    }

    private void fillTerrainTypes()
    {

        UnityEngine.Object layerTypeDropdownPrefab = Resources.Load("Prefabs/LayerTypeDropdown", typeof(GameObject));
        Dropdown layerTypeDropdown;

        int i = 0;
        foreach (LayerType terrainLayerType in terrainPipelineLayer.LayerTypes)
        {
            if (i < layerTypeDropdowns.Count)
            {
                layerTypeDropdown = layerTypeDropdowns[i];
                layerTypeDropdown.onValueChanged.RemoveAllListeners();
            }
            else
            {
                layerTypeDropdown = ((GameObject)Instantiate(layerTypeDropdownPrefab)).GetComponent<Dropdown>();
                layerTypeDropdown.transform.SetParent(LayerTypesPanel.transform);
                layerTypeDropdowns.Add(layerTypeDropdown);
            }

            byte previousTypeCode = (byte)terrainLayerType;
            List<byte> typeCodes = fillLayerTypeDropdown(layerTypeDropdown, terrainLayerType, false);

            layerTypeDropdown.onValueChanged.AddListener((idx) => LayerTypeChanged(idx, typeCodes, previousTypeCode));

            lastTerrainPipelineLayerType = terrainLayerType;

            i++;
        }

        if (i < Enum.GetValues(typeof(LayerType)).Length)
        {
            if (i < layerTypeDropdowns.Count)
            {
                layerTypeDropdown = layerTypeDropdowns[i];
                layerTypeDropdown.onValueChanged.RemoveAllListeners();
            }
            else
            {
                layerTypeDropdown = ((GameObject)Instantiate(layerTypeDropdownPrefab)).GetComponent<Dropdown>();
                layerTypeDropdown.transform.SetParent(LayerTypesPanel.transform);
                layerTypeDropdowns.Add(layerTypeDropdown);
            }

            List<byte> typeCodes = fillLayerTypeDropdown(layerTypeDropdown, LayerType.HeightMap, true);

            layerTypeDropdown.onValueChanged.AddListener((idx) => LayerTypeChanged(idx, typeCodes, 0));
        }
    }

    private List<byte> fillLayerTypeDropdown(Dropdown layerTypeDropdown, LayerType selectedTerrainPipelineLayerType, bool filterAll)
    {
        List<byte> typeCodes = new List<byte>();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        int selectedIdx = 0;
        int i = 0;

        if (filterAll)
        {
            options.Add(new Dropdown.OptionData("Add type..."));
            typeCodes.Add(0);
        }

        foreach(LayerType terrainLayerType in Enum.GetValues(typeof(LayerType)))
        {
            if (!terrainPipelineLayer.LayerTypes.Contains(terrainLayerType) || (!filterAll && selectedTerrainPipelineLayerType.Equals(terrainLayerType)))
            {
                if (selectedTerrainPipelineLayerType.Equals(terrainLayerType))
                {
                    selectedIdx = i;
                }
                options.Add(new Dropdown.OptionData(TerrainProjectManager.LayerNames[terrainLayerType]));
                typeCodes.Add((byte)terrainLayerType);
                i++;
            }
        }

        layerTypeDropdown.options.Clear();
        layerTypeDropdown.AddOptions(options);
        layerTypeDropdown.value = selectedIdx;

        return typeCodes;
    }

    public void fillPluginList()
    {
        foreach(GameObject pluginOption in pluginOptionsMap.Keys){
            Destroy(pluginOption);
        }
        pluginOptionsMap.Clear();
        Dictionary<TerrainPipelineLayer, List<TerrainPluginInfo>> availablePlugins = TerrainProject.AvailableTerrainPlugins;
        GameObject selectedPlugin = null;

        foreach (TerrainPipelineLayer terrainPipelineLayer in availablePlugins.Keys)
        {
            foreach(TerrainPluginInfo terrainPluginInfo in availablePlugins[terrainPipelineLayer])
            {
                GameObject layerOption = createPluginOption(terrainPluginInfo);

                layerOption.transform.SetParent(PluginsContainer.transform);
                layerOption.GetComponent<PluginOption>().LayerConfigurationsPanel = this;
                pluginOptionsMap.Add(layerOption, terrainPluginInfo);

                if (this.terrainPipelineLayer.PluginId.Equals(terrainPluginInfo.Id))
                {
                    selectedPlugin = layerOption;
                }
            }
        }

        if (selectedPlugin != null)
            selectedPlugin.GetComponent<PluginOption>().Select();
    }

    public void PlginClicked(GameObject clickedObject)
    {
        foreach (GameObject pluginOption in pluginOptionsMap.Keys)
        {
            if (pluginOption.GetComponent<PluginOption>().Selected && pluginOption == clickedObject)
                clickedObject = null;

            pluginOption.GetComponent<PluginOption>().DeSelect();
        }

        if(clickedObject != null)
            clickedObject.GetComponent<PluginOption>().Select();

        ChangePlugin();
    }

    private GameObject createPluginOption(TerrainPluginInfo terrainPluginInfo)
    {
        UnityEngine.Object layerOptionPrefab = Resources.Load("Prefabs/PluginOption", typeof(GameObject));
        UnityEngine.Object terrainIconPrefab = Resources.Load("Prefabs/TerrainButtonIcon", typeof(GameObject));

        GameObject layerOption = ((GameObject)Instantiate(layerOptionPrefab));
        PluginOption pluginOption = layerOption.GetComponent<PluginOption>();

        pluginOption.Title.text = terrainPluginInfo.Name;
        pluginOption.Author.text = "Author: " + terrainPluginInfo.Author;
        pluginOption.Description.text = "Description: " + terrainPluginInfo.Description;

        foreach (LayerType terrainPipelineLayerType in TerrainProjectManager.InactiveTerrainLayerIcons.Keys)
        {
            // Out
            if (terrainPluginInfo.Out.Contains(terrainPipelineLayerType))
            {
                GameObject terrainIcon = (GameObject)Instantiate(terrainIconPrefab);
                terrainIcon.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(TerrainProjectManager.InactiveTerrainLayerIcons[terrainPipelineLayerType]);
                terrainIcon.transform.SetParent(pluginOption.Out.transform);
            }

            // In
            if (terrainPluginInfo.In.Contains(terrainPipelineLayerType))
            {
                GameObject terrainIcon = (GameObject)Instantiate(terrainIconPrefab);
                terrainIcon.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(TerrainProjectManager.InactiveTerrainLayerIcons[terrainPipelineLayerType]);
                terrainIcon.transform.SetParent(pluginOption.In.transform);
            }

            // Not
            if (terrainPluginInfo.Not.Contains(terrainPipelineLayerType))
            {
                GameObject terrainIcon = (GameObject)Instantiate(terrainIconPrefab);
                terrainIcon.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(TerrainProjectManager.InactiveTerrainLayerIcons[terrainPipelineLayerType]);
                terrainIcon.transform.SetParent(pluginOption.Not.transform);
            }
        }

        checkRestrictions(layerOption, terrainPluginInfo, projectPanel.configurationTerrainPipelineLayer, this.terrainPipelineLayer);
        return layerOption;
    }

    public static void checkRestrictions(GameObject pluginOption, 
        TerrainPluginInfo terrainPluginInfo, 
        List<TerrainPipelineLayer> configurationTerrainPipelineLayer, 
        TerrainPipelineLayer currentPipelineLayer)
    {
        PluginOption pluginOptionScript = pluginOption.GetComponent<PluginOption>();
        if (currentPipelineLayer.LayerTypes.Count != terrainPluginInfo.Out.Count)
        {
            pluginOptionScript.InvalidateOut();
        }
        else
        {
            foreach(LayerType terrainPipelineLayerType in terrainPluginInfo.Out)
            {
                if (!currentPipelineLayer.LayerTypes.Contains(terrainPipelineLayerType))
                {
                    pluginOptionScript.InvalidateOut();
                    break;
                }
            }
        }

        List<LayerType> previousPipelineLayerTypes = new List<LayerType>();
        foreach( TerrainPipelineLayer terrainPipelineLayer in configurationTerrainPipelineLayer)
        {
            if (terrainPipelineLayer.LayerCode.Equals(currentPipelineLayer.LayerCode))
            {
                break;
            }

            previousPipelineLayerTypes.AddRange(terrainPipelineLayer.LayerTypes);
        }

        foreach(LayerType terrainPipelineLayerType in terrainPluginInfo.In)
        {
            if (!previousPipelineLayerTypes.Contains(terrainPipelineLayerType))
            {
                pluginOptionScript.InvalidateIn();
                break;
            }
        }

        foreach (LayerType terrainPipelineLayerType in terrainPluginInfo.Not)
        {
            if (previousPipelineLayerTypes.Contains(terrainPipelineLayerType))
            {
                pluginOptionScript.InvalidateNot();
                break;
            }
        }
    }

    public static bool ArePluginRestrictionsMet(TerrainPluginInfo terrainPluginInfo, List<TerrainPipelineLayer> configurationTerrainPipelineLayer, TerrainPipelineLayer currentTerrainPipelineLayer)
    {
        UnityEngine.Object layerOptionPrefab = Resources.Load("Prefabs/PluginOption", typeof(GameObject));

        GameObject layerOption = ((GameObject)Instantiate(layerOptionPrefab));

        checkRestrictions(layerOption, terrainPluginInfo, configurationTerrainPipelineLayer, currentTerrainPipelineLayer);

        if(layerOption.GetComponent<PluginOption>().Valid)
            return true;
        return false;
    }

    public void RemoveLayerTypeClick()
    {
        if (terrainPipelineLayer.LayerTypes.Count == 1)
            return;
        if (layerTypeDropdowns.Count > 0)
        {
            Dropdown lastDropdown = layerTypeDropdowns[layerTypeDropdowns.Count - 1];
            layerTypeDropdowns.RemoveAt(layerTypeDropdowns.Count - 1);

            Destroy(lastDropdown.gameObject);
        }

        terrainPipelineLayer.LayerCode = (byte)(terrainPipelineLayer.LayerCode & (~(byte)lastTerrainPipelineLayerType));

        fillTerrainTypes();
        fillPluginList();
        layerButton = projectPanel.RefreshAvailableLayers(layerButton);
    }

    public void MoveLayerUpClick()
    {
        layerButton = projectPanel.MoveLayerUp(layerButton);
    }

    public void MoveLayerDownClick()
    {
        layerButton = projectPanel.MoveLayerDown(layerButton);
    }

    public void LayerTypeChanged(int idx, List<byte> typeCodes, byte previousTypeCode)
    {
        if(!typeCodes[idx].Equals(previousTypeCode) && !previousTypeCode.Equals(0))
            terrainPipelineLayer.LayerCode = (byte)(terrainPipelineLayer.LayerCode & (~previousTypeCode));
        terrainPipelineLayer.LayerCode = (byte)(terrainPipelineLayer.LayerCode | typeCodes[idx]);
        fillTerrainTypes();
        fillPluginList();
        layerButton = projectPanel.RefreshAvailableLayers(layerButton);
    }

    public void ChangePlugin()
    {
        GameObject selectedPlugin = null;

        foreach (GameObject pluginOption in pluginOptionsMap.Keys)
        {
            if (pluginOption.GetComponent<PluginOption>().Selected)
                selectedPlugin = pluginOption;
        }

        if (selectedPlugin != null)
            terrainPipelineLayer.PluginId = pluginOptionsMap[selectedPlugin].Id;
        else
            terrainPipelineLayer.PluginId = 0;
    }
}
