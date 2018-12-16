using UnityEngine;
using TerrainGeneration_PluginBase;
using UnityEngine.UI;

public class TerrainInputSketchPanel : MonoBehaviour {

    public GameObject DrawingPanel_;
    public GameObject ToolsPanel;
    private TerrainInput terrainInput;

    public void Init(TerrainInput terrainInput)
    {
        this.terrainInput = terrainInput;

        DrawingPanel_.GetComponent<DrawingPanel>().Init(((TerrainInputSketch)terrainInput).Width, ((TerrainInputSketch)terrainInput).Height, ((TerrainInputSketch)terrainInput).BaseColor, ((TerrainInputSketch)terrainInput).Value);
        foreach (SketchTool sketchTool in ((TerrainInputSketch)terrainInput).SketchTools)
        {
            createSketchTool(sketchTool);
        }

        if(((TerrainInputSketch)terrainInput).SketchTools.Count > 0)
        {
            System.Console.WriteLine("Setting Tool: " + ((TerrainInputSketch)terrainInput).SketchTools[0].Title);
            selectTool(((TerrainInputSketch)terrainInput).SketchTools[0]);
        }
    }

    private void createSketchTool(SketchTool sketchTool)
    {

        System.Console.WriteLine("Tool: " + sketchTool.Title);

        UnityEngine.Object tabButtonPrefab = Resources.Load("Prefabs/SketchToolButton", typeof(GameObject));
        GameObject tabButton = (GameObject)Instantiate(tabButtonPrefab);
        tabButton.transform.SetParent(ToolsPanel.transform, false);

        tabButton.GetComponentInChildren<Text>().text = sketchTool.Title;
        tabButton.GetComponent<Button>().onClick.AddListener(() => selectTool(sketchTool));

        Color color = Color.red;
        switch (sketchTool.Type)
        {
            case SketchToolType.Brush:
                color = DrawingPanel.stringToColor(((SketchToolBrush)sketchTool).Color);
                break;
            case SketchToolType.Circle:
                color = DrawingPanel.stringToColor(((SketchToolCircle)sketchTool).Color);
                break;
            case SketchToolType.Circunference:
                color = DrawingPanel.stringToColor(((SketchToolCircunference)sketchTool).Color);
                break;
            case SketchToolType.Eraser:
                tabButton.transform.GetChild(2).gameObject.SetActive(false);
                tabButton.transform.GetChild(3).gameObject.SetActive(true);
                color = DrawingPanel.stringToColor(((TerrainInputSketch)terrainInput).BaseColor);
                break;
        }


        tabButton.GetComponentsInChildren<Image>()[1].color = color;
    }

    private void selectTool(SketchTool sketchTool)
    {
        DrawingPanel_.GetComponent<DrawingPanel>().SetTool(sketchTool);
    }

    internal void FillInputValues()
    {
        ((TerrainInputSketch)terrainInput).Value = DrawingPanel_.GetComponent<DrawingPanel>().GetImageData();
    }
}
