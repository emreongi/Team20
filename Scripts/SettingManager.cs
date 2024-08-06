using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Collections.Generic;

[RequireComponent(typeof(JsonManager))]

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_InputField playerEmailInput;
    [SerializeField] private TMP_InputField playerAvatarURLInput;

    [SerializeField] private TMP_InputField transitionSpeedInput;
    [SerializeField] private TMP_InputField zoomSpeed_FPSInput;
    [SerializeField] private TMP_InputField zoomSpeed_TPSInput;
    [SerializeField] private TMP_InputField biasSpeedInput;
    [SerializeField] private TMP_InputField targetEngagementShootingSpeed_FPSInput;
    [SerializeField] private TMP_InputField targetEngagementShootingSpeed_TPSInput;
    [SerializeField] private TMP_InputField focusSpotSpeedInput;

    [SerializeField] private Slider soundValue;

    [SerializeField] private Image avatarImage;

    private string jsonFilePath;
    private PlayerData playerData;
    private JsonManager jsonManager;

    void Start()
    {
        jsonFilePath = Path.Combine(Application.dataPath, "Resources/PlayerData.json");
        jsonManager = GetComponent<JsonManager>();
        SettingPanelOpen();
    }

    public void SettingPanelOpen()
    {
        try
        {
            playerData = jsonManager.ReadJson();
            if (playerData != null)
            {
                LoadDataToUI();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error reading JSON: " + ex.Message);
        }

        PlayFabPlayerCollectData();
    }

    public void SettingPanelClose()
    {
        SaveDataFromUI();
        UpdateJsonData();
    }

    private void LoadDataToUI()
    {
        playerNameInput.text = playerData.playerName;
        playerAvatarURLInput.text = playerData.playerAvatarUrl;
        transitionSpeedInput.text = playerData.transitionSpeed.ToString();
        zoomSpeed_FPSInput.text = playerData.zoomSpeed_FPS.ToString();
        zoomSpeed_TPSInput.text = playerData.zoomSpeed_TPS.ToString();
        biasSpeedInput.text = playerData.biasSpeed.ToString();
        targetEngagementShootingSpeed_FPSInput.text = playerData.targetEngagementShootingSpeed_FPS.ToString();
        targetEngagementShootingSpeed_TPSInput.text = playerData.targetEngagementShootingSpeed_TPS.ToString();
        focusSpotSpeedInput.text = playerData.focusSpotSpeed.ToString();
        soundValue.value = playerData.soundValue;
    }

    private void SaveDataFromUI()
    {
        playerData.playerName = playerNameInput.text;
        playerData.playerAvatarUrl = playerAvatarURLInput.text;
        playerData.transitionSpeed = TryParseFloat(transitionSpeedInput.text);
        playerData.zoomSpeed_FPS = TryParseFloat(zoomSpeed_FPSInput.text);
        playerData.zoomSpeed_TPS = TryParseFloat(zoomSpeed_TPSInput.text);
        playerData.biasSpeed = TryParseFloat(biasSpeedInput.text);
        playerData.targetEngagementShootingSpeed_FPS = TryParseFloat(targetEngagementShootingSpeed_FPSInput.text);
        playerData.targetEngagementShootingSpeed_TPS = TryParseFloat(targetEngagementShootingSpeed_TPSInput.text);
        playerData.focusSpotSpeed = TryParseFloat(focusSpotSpeedInput.text);
        playerData.soundValue = soundValue.value;
    }

    private void UpdateJsonData()
    {
        try
        {
            string updatedJson = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(jsonFilePath, updatedJson);
            SaveJsonToPlayFab(updatedJson);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error writing JSON: " + ex.Message);
        }
    }

    public void SaveJsonToPlayFab(string updatedJson)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "PlayerData", updatedJson }
            }
        };
        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("JSON data successfully saved to PlayFab."),
            OnError
        );
    }

    void PlayFabPlayerCollectData()
    {
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoSuccess, OnError);
    }

    void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        if (result.AccountInfo != null)
        {
            playerNameInput.text = result.AccountInfo.TitleInfo.DisplayName;
            playerEmailInput.text = result.AccountInfo.PrivateInfo.Email;
            Debug.Log(result.AccountInfo.TitleInfo.AvatarUrl.ToString());
            playerAvatarURLInput.text = result.AccountInfo.TitleInfo.AvatarUrl.ToString();
            StartCoroutine(LoadAvatarImage(playerAvatarURLInput.text));
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
            avatarImage.sprite = avatarSprite;
        }
        else
        {
            Debug.LogError("Failed to load avatar image: " + request.error);
        }
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
    }

    private float TryParseFloat(string input)
    {
        if (float.TryParse(input, out float result))
        {
            return result;
        }
        else
        {
            Debug.LogWarning("Input is not a valid float: " + input);
            return 0f;
        }
    }
}