using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ProfileGeneral : MonoBehaviour
{
    public Image playerAvatar;
    public TMP_Text playerName;
    public TMP_Text playerMoney;

    private JsonManager jsonManager;
    private PlayerData playerData;

    void Start()
    {
        jsonManager = GetComponent<JsonManager>();

        GetProfileData();
    }

    void GetProfileData()
    {
        playerData = jsonManager.ReadJson();
        if (playerData != null)
        {
            playerName.text = playerData.playerName;
            playerMoney.text = playerData.playerMoney.ToString();
            StartCoroutine(LoadAvatarImage(playerData.playerAvatarUrl));
        }
    }

    private IEnumerator LoadAvatarImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite avatarSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            playerAvatar.sprite = avatarSprite;
        }
        else
        {
            Debug.Log("Avatar görseli yüklenemedi: " + request.error);
        }
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
