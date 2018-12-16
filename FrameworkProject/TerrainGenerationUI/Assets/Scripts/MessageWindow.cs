using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessageWindow : MonoBehaviour {

    public void OnOk()
    {
        gameObject.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        gameObject.GetComponentInChildren<Text>().text = message;
        gameObject.SetActive(true);
    }
}
