using UnityEngine;
using System.Collections;
using System;
using TerrainGeneration_PluginBase;
using UnityEngine.UI;

public class TerrainOutputValuesPanel : MonoBehaviour {

    internal void Init(TerrainOutput terrainOutput)
    {
        for(int i = 0; i < ((TerrainOutputValues)terrainOutput).ValueTitles.Count; i++)
        {
            UnityEngine.Object terrainOutputPrefab = Resources.Load("Prefabs/TerrainOutputValuePanel", typeof(GameObject));
            GameObject layerOutput = (GameObject)Instantiate(terrainOutputPrefab);
            layerOutput.GetComponentsInChildren<Text>()[0].text = ((TerrainOutputValues)terrainOutput).ValueTitles[i];
            layerOutput.GetComponentsInChildren<Text>()[1].text = ((TerrainOutputValues)terrainOutput).Values[i];

            layerOutput.transform.SetParent(gameObject.transform);
        }
    }
}
