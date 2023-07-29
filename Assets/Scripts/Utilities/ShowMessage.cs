using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowMessage : Singleton<ShowMessage>
{
    public GameObject messageBox;
    public TextMeshProUGUI message;

    public void Show(string msg)
    {
        message.text = msg;
        StartCoroutine(ShowMsg());
    }

    private IEnumerator ShowMsg()
    {
        messageBox.SetActive(true);
        yield return new WaitForSeconds(3f);
        messageBox.SetActive(false);
    }
}
