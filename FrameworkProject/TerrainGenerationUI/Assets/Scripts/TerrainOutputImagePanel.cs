using UnityEngine;
using System.Collections;
using System;
using TerrainGeneration_PluginBase;
using UnityEngine.UI;
using System.IO;

public class TerrainOutputImagePanel : MonoBehaviour {

    public GameObject Image;

    internal void Init(TerrainOutput terrainOutput)
    {
        if (File.Exists(((TerrainOutputImage)terrainOutput).ImagePath))
        {
            byte[] fileData = File.ReadAllBytes(((TerrainOutputImage)terrainOutput).ImagePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);

            Image.GetComponent<RectTransform>().sizeDelta = new Vector2(tex.width, tex.height);

            Image.GetComponent<RawImage>().texture = tex;
        }
    }
}
