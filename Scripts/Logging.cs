using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Logging : MonoBehaviour
{
    public TMP_Text playerName;
    public TMP_Text index;
    public TMP_Text description;

    public void SetLoggingProperties(string playerName, int index, string description)
    {
        this.playerName.text = playerName;
        this.index.text = index.ToString();
        this.description.text = description;
    }

    void Start()
    {
        GetComponent<RectTransform>().SetAsFirstSibling();
    }
}
