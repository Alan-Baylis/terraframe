using UnityEngine;
using TerrainGeneration_Core;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TerrainGeneration_PluginBase;

public class TerrainProjectManager : MonoBehaviour {

    public NewTerrainForm NewTerrainForm;
    public ProjectPanel ProjectPanel;
    public GameObject WaitPassive;

    public GameObject DeleteButton;
    public GameObject ConfirmDeleteButton;
    public GameObject CancelDeleteButton;

    private List<GameObject> projectButtons = new List<GameObject>();
    private Dictionary<GameObject, TerrainButton> projectButtonsDict = new Dictionary<GameObject, TerrainButton>();
    private TerrainProject terrainProjectToOpen;

    public static Dictionary<LayerType, string> InactiveTerrainLayerIcons = new Dictionary<LayerType, string>()
    {
        { LayerType.HeightMap, "Graphics/TerrainIcons/heightmap_white"},
        { LayerType.WaterBodies, "Graphics/TerrainIcons/water_white"},
        { LayerType.Vegetation, "Graphics/TerrainIcons/vegetation_white"},
        { LayerType.Roads, "Graphics/TerrainIcons/roads_white"},
        { LayerType.Urban, "Graphics/TerrainIcons/urban_white"}
    };

    public static Dictionary<LayerType, string> ActiveTerrainLayerIcons = new Dictionary<LayerType, string>()
    {
        { LayerType.HeightMap, "Graphics/TerrainIcons/heightmap_color"},
        { LayerType.WaterBodies, "Graphics/TerrainIcons/water_color"},
        { LayerType.Vegetation, "Graphics/TerrainIcons/vegetation_color"},
        { LayerType.Roads, "Graphics/TerrainIcons/roads_color"},
        { LayerType.Urban, "Graphics/TerrainIcons/urban_color"}
    };

    public static Dictionary<LayerType, string> LayerNames = new Dictionary<LayerType, string>()
    {
        { LayerType.HeightMap, "Heightmap"},
        { LayerType.WaterBodies, "Water"},
        { LayerType.Vegetation, "Vegetation"},
        { LayerType.Roads, "Roads"},
        { LayerType.Urban, "Cities"}
    };

    public void ShowAvailableProjects()
    {
        clearProjects();
        fillProjects();
        gameObject.SetActive(true);
    }

    private void clearProjects()
    {
        foreach(GameObject projectButton in projectButtons)
        {
            Destroy(projectButton);
        }

        projectButtons.Clear();
        projectButtonsDict.Clear();

    }

    private void fillProjects()
    {
        Object terrainButtonPrefab;
        GameObject terrainButton;

        foreach (TerrainProject terrainProject in TerrainProject.TerrainProjects)
        {
            System.Console.WriteLine(terrainProject.Info.Name);
            terrainButtonPrefab = Resources.Load("Prefabs/TerrainButton", typeof(GameObject));
            terrainButton = (GameObject)Instantiate(terrainButtonPrefab);
            terrainButton.GetComponentInChildren<Text>().text = terrainProject.Info.Name;
            //terrainProject.Info.
            foreach(LayerType tplt in System.Enum.GetValues(typeof(LayerType)))
            {
                if (terrainProject.Info.ConfiguredLayers.Contains(tplt))
                {
                    Object terrainIconPrefab = Resources.Load("Prefabs/TerrainButtonIcon", typeof(GameObject));
                    GameObject terrainIcon = (GameObject)Instantiate(terrainIconPrefab);

                    if (terrainProject.Info.ProcessedLayers.Contains(tplt))
                    {
                        terrainIcon.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(ActiveTerrainLayerIcons[tplt]);
                    }
                    else
                    {
                        terrainIcon.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(InactiveTerrainLayerIcons[tplt]);
                    }

                    terrainIcon.transform.SetParent(terrainButton.GetComponentsInChildren<RectTransform>()[2].transform);
                }
            }

            if(terrainProject.Info.ThumbnailPath != null && terrainProject.Info.ThumbnailPath.Length > 0)
            {
                try
                {
                    byte[] fileData = File.ReadAllBytes(terrainProject.Info.ThumbnailPath);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(fileData);

                    terrainButton.GetComponentInChildren<RawImage>().texture = tex;
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine("Something went wrong with thumbnail");
                }
            }
            else
            {
                terrainButton.GetComponentInChildren<RawImage>().gameObject.SetActive(false);
            }
            terrainButton.transform.SetParent(gameObject.transform);
            projectButtons.Add(terrainButton);
            projectButtonsDict.Add(terrainButton, new TerrainButton(this, terrainButton, terrainProject));
        }

        terrainButtonPrefab = Resources.Load("Prefabs/TerrainButton", typeof(GameObject));
        terrainButton = (GameObject)Instantiate(terrainButtonPrefab);
        terrainButton.GetComponentInChildren<Text>().text = "Create new...";


        //terrainProject.Info.
        foreach (LayerType tplt in System.Enum.GetValues(typeof(LayerType)))
        {
            Object terrainIconPrefab = Resources.Load("Prefabs/TerrainButtonIcon", typeof(GameObject));
            GameObject terrainIcon = (GameObject)Instantiate(terrainIconPrefab);
            terrainIcon.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(InactiveTerrainLayerIcons[tplt]);

            terrainIcon.transform.SetParent(terrainButton.GetComponentsInChildren<RectTransform>()[2].transform);
        }

        Texture2D plus = Resources.Load<Texture2D>("Graphics/plus");

        /*Texture2D croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                (int)sprite.textureRect.y,
                                                (int)sprite.textureRect.width,
                                                (int)sprite.textureRect.height);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();*/

        try
        {
            terrainButton.GetComponentInChildren<RawImage>().texture = plus;
        }
        catch (System.Exception ex)
        {
            terrainButton.GetComponentInChildren<RawImage>().gameObject.SetActive(false);
            System.Console.WriteLine("Something went wrong with thumbnail");
        }

        terrainButton.transform.SetParent(gameObject.transform);
        terrainButton.GetComponent<Button>().onClick.AddListener(() => createNewTerrain());
        projectButtons.Add(terrainButton);
        terrainButton.GetComponentInChildren<Toggle>().gameObject.SetActive(false);
    }

    private void createNewTerrain()
    {
        NewTerrainForm.ShowForm(createNewTerrain);
    }

    private void createNewTerrain(string terrainName)
    {
        System.Console.WriteLine("Create new Terrain: " + terrainName);
        if(terrainName != null && terrainName.Length > 0)
        {
            WaitPassive.SetActive(true);
            TerrainProject terrainProject = new TerrainProject(terrainName, "");
            terrainProject.Load();
            terrainProject.Save();

            openProject(terrainProject);
        }

    }

    private void openProject(TerrainProject terrainProject)
    {
        terrainProjectToOpen = terrainProject;
        WaitPassive.SetActive(true);
        Invoke("continueOpenProject", 0.1f);
    }

    private void continueOpenProject()
    {
        ProjectPanel.ShowProject(terrainProjectToOpen);
        gameObject.SetActive(false);
    }

    private class TerrainButton
    {
        public TerrainProject TerrainProject;
        public Toggle toggle;
        public bool IgnoreClick;

        public TerrainButton(TerrainProjectManager terrainProjectManager, GameObject terrainButton, TerrainProject terrainProject)
        {
            TerrainProject = terrainProject;
            terrainButton.GetComponent<Button>().onClick.AddListener(() => terrainClicked(terrainProjectManager, terrainProject));
            toggle = terrainButton.GetComponentInChildren<Toggle>();
            toggle.gameObject.SetActive(false);
        }

        private void terrainClicked(TerrainProjectManager terrainProjectManager, TerrainProject terrainProject)
        {
            if (IgnoreClick)
                return;
            System.Console.WriteLine("Clicked: " + terrainProject.Info.Name);
            terrainProjectManager.openProject(terrainProject);
        }
    }

    public void OnDeletePressed()
    {
        GameObject projectButton_ = null;
        foreach(GameObject projectButton in projectButtons)
        {
            projectButton_ = projectButton;
            if (projectButtonsDict.ContainsKey(projectButton))
            {
                projectButtonsDict[projectButton].toggle.gameObject.SetActive(true);
                projectButtonsDict[projectButton].toggle.isOn = false;
                projectButtonsDict[projectButton].IgnoreClick = true;
            }
        }

        if(projectButton_ != null)
        {
            projectButtons.Remove(projectButton_);
            Destroy(projectButton_);
        }

        DeleteButton.SetActive(false);
        ConfirmDeleteButton.SetActive(true);
        CancelDeleteButton.SetActive(true);
}

    public void OnDeleteCancelPressed()
    {
        ShowAvailableProjects();

        DeleteButton.SetActive(true);
        ConfirmDeleteButton.SetActive(false);
        CancelDeleteButton.SetActive(false);
    }

    public void OnDeleteConfirmPressed()
    {
        foreach (GameObject projectButton in projectButtons)
        {
            if (projectButtonsDict[projectButton].toggle.isOn)
            {
                TerrainProject.DeleteProject(projectButtonsDict[projectButton].TerrainProject);
            }
        }
        ShowAvailableProjects();

        DeleteButton.SetActive(true);
        ConfirmDeleteButton.SetActive(false);
        CancelDeleteButton.SetActive(false);
    }
}