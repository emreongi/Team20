using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class Success : MonoBehaviour
{
    [SerializeField] private TMP_Text successName;
    [SerializeField] private TMP_Text successDescription;
    [SerializeField] private Image successImage;

    public void EditSuccessTraits(string successName, string successDescription, Sprite successImage)
    {
        this.successName.text = successName;
        this.successDescription.text = successDescription;
        this.successImage.sprite = successImage;
    }
}
