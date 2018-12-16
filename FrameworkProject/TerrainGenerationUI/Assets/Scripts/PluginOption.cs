using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PluginOption : MonoBehaviour {

    public Text Title;
    public Text Author;
    public Text Description;

    public GameObject Out;
    public GameObject In;
    public GameObject Not;
    public GameObject Restrictions;


    public LayerConfigurationsPanel LayerConfigurationsPanel;
    public bool Selected = false;
    public bool Valid = true;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Select()
    {
        gameObject.GetComponent<Image>().color = new Color(0,1,0,0.2f);
        Selected = true;
    }

    public void DeSelect()
    {
        gameObject.GetComponent<Image>().color = Color.white;
        Selected = false;
    }

    public void Click()
    {
        LayerConfigurationsPanel.PlginClicked(gameObject);
    }

    public void InvalidateOut()
    {
        Restrictions.GetComponent<Image>().color = new Color(1, 0, 0, 0.2f);
        Out.transform.parent.GetComponent<Image>().color = Color.red;
        Valid = false;
    }

    public void InvalidateIn()
    {
        Restrictions.GetComponent<Image>().color = new Color(1, 0, 0, 0.2f);
        In.transform.parent.GetComponent<Image>().color = Color.red;
        Valid = false;
    }

    public void InvalidateNot()
    {
        Restrictions.GetComponent<Image>().color = new Color(1, 0, 0, 0.2f);
        Not.transform.parent.GetComponent<Image>().color = Color.red;
        Valid = false;
    }
}
