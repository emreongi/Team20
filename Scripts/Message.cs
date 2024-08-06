using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    public TMP_Text messageText;

    void Start()
    {
        GetComponent<RectTransform>().SetAsFirstSibling();
    }
}