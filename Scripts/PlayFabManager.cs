using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using UnityEngine.UI;

public class PlayFabManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_Text messageText;
    public JsonManager jsonManager;

    private void Awake()
    {
        jsonManager = FindObjectOfType<JsonManager>();
    }

    public void RegisterButton()
    {
        messageText.text = "";

        if (passwordInput.text.Length < 6 || passwordInput.text.Length > 100)
        {
            messageText.color = Color.red;
            messageText.text = "The password must be a minimum of (6)six characters.";
            return;
        }

        var request = new RegisterPlayFabUserRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
            Username = userNameInput.text,
            DisplayName = userNameInput.text,
            RequireBothUsernameAndEmail = true
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    public void LoginButton()
    {
        messageText.text = "";

        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    public void ResetPasswordButton()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = emailInput.text,
            TitleId = PlayFabSettings.TitleId
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }

    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        messageText.color = Color.green;
        messageText.text = "The password reset link was sent to " + emailInput.text + " as an e-mail.";
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        messageText.color = Color.green;
        messageText.text = "Registration successful! Please log in.";

        if (jsonManager != null)
        {
            jsonManager.playerName = userNameInput.text;
            jsonManager.playerMoney = 100;
            jsonManager.playerAvatarUrl = "https://img.freepik.com/free-vector/cute-astronaut-peace-moon-with-rocket-cartoon-vector-icon-illustration-science-technology-icon_138676-5030.jpg?size=338&ext=jpg&ga=GA1.1.2008272138.1721606400&semt=ais_user";
            jsonManager.transitionSpeed = 0.5f;
            jsonManager.zoomSpeed_FPS = 0.2f;
            jsonManager.zoomSpeed_TPS = 0.5f;
            jsonManager.biasSpeed = 0.5f;
            jsonManager.targetEngagementShootingSpeed_FPS = 40f;
            jsonManager.targetEngagementShootingSpeed_TPS = 40f;
            jsonManager.freeCrosshairSpeed_FPS = 20f;
            jsonManager.freeCrosshairSpeed_TPS = 20f;
            jsonManager.focusSpotSpeed = 0.5f;
            jsonManager.FPS_TPS_SwitchKey = "V";
            jsonManager.engagementButton = 1;
            jsonManager.fireButton = 0;
            jsonManager.soundValue = 0.5f;
            jsonManager.purchasedIdItems.Add(1);
            jsonManager.usedItem = 1;

            jsonManager.CreateJson();

            // JSON dosyasını PlayFab'a yükleniyor.
            string json = JsonUtility.ToJson(jsonManager);
            var updateUserDataRequest = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    { "PlayerData", json }
                }
            };
            PlayFabClientAPI.UpdateUserData(updateUserDataRequest, OnDataSendSuccess, OnError);
        }
        else
        {
            Debug.LogError("JsonManager atanmamıştır.");
        }
    }

    void OnLoginSuccess(LoginResult result)
    {
        messageText.color = Color.green;
        messageText.text = "Entry Successful!";

        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = result.PlayFabId
        }, OnDataReceived, OnError);

        Invoke("LoadScene", 2f);
    }

    void LoadScene()
    {
        SceneManager.LoadScene("HomeScreen");
    }

    void OnDataReceived(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("PlayerData"))
        {
            string json = result.Data["PlayerData"].Value;
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            // JSON dosyasını oluşturma ve kaydetme
            string path = Path.Combine(Application.persistentDataPath, "PlayerData.json");
            File.WriteAllText(path, json);

            // JsonManager içeriğini güncelle
            if (jsonManager != null)
            {
                jsonManager.LoadFromJson(json);
            }

            Debug.Log("JSON indirildi ve kaydedildi: " + path);
        }
        else
        {
            Debug.LogError("Oyuncu verileri bulunamadı.");
        }
    }

    // Veriler(JSON) PlayFab 'e başarılı şekilde gönderildiği yazdırılıyor.
    void OnDataSendSuccess(UpdateUserDataResult result)
    {
        Debug.Log("Veriler PlayFab'a başarıyla gönderildi!");
    }

    // PlayFab kaynaklı hataları yazdırıyor.
    void OnError(PlayFabError error)
    {
        Debug.Log("Hata: " + error.ErrorMessage);
        Debug.LogError(error.GenerateErrorReport());

        messageText.color = Color.red;
        messageText.text = "Error: " + error.ErrorMessage;
    }
}