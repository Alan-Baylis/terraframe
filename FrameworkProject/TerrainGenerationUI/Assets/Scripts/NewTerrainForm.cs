using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class NewTerrainForm : MonoBehaviour {

    public delegate void OkAction(string terrainName);

    public InputField InputField;
    public Button OkButton;
    public Button CancelButton;

    private OkAction okAction;

	// Use this for initialization
	void Start () {
        OkButton.onClick.AddListener(() => onOk());
        CancelButton.onClick.AddListener(() => onCancel());
    }

    public void ShowForm(OkAction okAction)
    {
        clean();
        this.okAction = okAction;
        gameObject.SetActive(true);
    }

    private void clean()
    {
        InputField.GetComponent<InputField>().text = "";
    }

    private void onOk()
    {
        okAction.Invoke(InputField.GetComponent<InputField>().text);
        gameObject.SetActive(false);
    }

    private void onCancel()
    {
        gameObject.SetActive(false);
    }
}
