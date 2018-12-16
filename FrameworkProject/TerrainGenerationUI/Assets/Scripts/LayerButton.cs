using UnityEngine;
using TerrainGeneration_Core;
using TerrainGeneration_PluginBase;
using System;
using UnityEngine.UI;

public class LayerButton : MonoBehaviour {

    private TerrainPipelineLayer terrainPipelineLayer;
    public TerrainPipelineLayer TerrainPipelineLayer
    {
        get
        {
            return terrainPipelineLayer;
        }

        set
        {
            if (terrainPipelineLayer != null)
            {
                if (value.HasValidPlugin && (ProjectPanel == null || !ProjectPanel.ConfigurationMode))
                {
                    gameObject.GetComponentInChildren<Toggle>().gameObject.SetActive(true);
                    gameObject.GetComponentInChildren<Toggle>().isOn = false;
                }
                else
                {
                    gameObject.GetComponentInChildren<Toggle>().gameObject.SetActive(false);
                }
            }
            terrainPipelineLayer = value;
        }
    }

    private ProjectPanel projectPanel;
    public ProjectPanel ProjectPanel {
        get
        {
            return projectPanel;
        }
        set
        {
            projectPanel = value;
            TerrainPipelineLayer = terrainPipelineLayer;
        }
    }
    private bool dragging = false;

    public void Click()
    {
        if(!dragging)
            ProjectPanel.SelectPipelineLayer(gameObject);
    }
}
