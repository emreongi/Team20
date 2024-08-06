using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string playerAvatarUrl;
    public int playerMoney;
    public float transitionSpeed;
    public float zoomSpeed_FPS;
    public float zoomSpeed_TPS;
    public float biasSpeed;
    public float targetEngagementShootingSpeed_FPS;
    public float targetEngagementShootingSpeed_TPS;
    public float freeCrosshairSpeed_FPS;
    public float freeCrosshairSpeed_TPS;
    public float focusSpotSpeed;
    public string FPS_TPS_SwitchKey;
    public int engagementButton;
    public int fireButton;
    public float soundValue;
    public List<int> purchasedIdItems;
    public float usedItem;
}

public class JsonManager : MonoBehaviour
{
    [HideInInspector] public string playerName;
    [HideInInspector] public string playerAvatarUrl;
    [HideInInspector] public int playerMoney;
    [HideInInspector] public float transitionSpeed;
    [HideInInspector] public float zoomSpeed_FPS;
    [HideInInspector] public float zoomSpeed_TPS;
    [HideInInspector] public float biasSpeed;
    [HideInInspector] public float targetEngagementShootingSpeed_FPS;
    [HideInInspector] public float targetEngagementShootingSpeed_TPS;
    [HideInInspector] public float freeCrosshairSpeed_FPS;
    [HideInInspector] public float freeCrosshairSpeed_TPS;
    [HideInInspector] public float focusSpotSpeed;
    [HideInInspector] public string FPS_TPS_SwitchKey;
    [HideInInspector] public int engagementButton;
    [HideInInspector] public int fireButton;
    [HideInInspector] public float soundValue;
    [HideInInspector] public List<int> purchasedIdItems = new List<int>();
    [HideInInspector] public int usedItem;

    private string GetPath()
    {
        return Path.Combine(Application.persistentDataPath, "PlayerData.json");
    }

    public void CreateJson()
    {
        PlayerData playerData = new PlayerData()
        {
            playerName = playerName,
            playerAvatarUrl = playerAvatarUrl,
            playerMoney = playerMoney,
            transitionSpeed = transitionSpeed,
            zoomSpeed_FPS = zoomSpeed_FPS,
            zoomSpeed_TPS = zoomSpeed_TPS,
            biasSpeed = biasSpeed,
            targetEngagementShootingSpeed_FPS = targetEngagementShootingSpeed_FPS,
            targetEngagementShootingSpeed_TPS = targetEngagementShootingSpeed_TPS,
            freeCrosshairSpeed_FPS = freeCrosshairSpeed_FPS,
            freeCrosshairSpeed_TPS = freeCrosshairSpeed_TPS,
            focusSpotSpeed = focusSpotSpeed,
            FPS_TPS_SwitchKey = FPS_TPS_SwitchKey,
            engagementButton = engagementButton,
            fireButton = fireButton,
            soundValue = soundValue,
            purchasedIdItems = purchasedIdItems,
            usedItem = usedItem
        };

        string json = JsonUtility.ToJson(playerData, true);
        string path = GetPath();
        File.WriteAllText(path, json);
    }

    public PlayerData ReadJson()
    {
        string path = GetPath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
            return playerData;
        }
        else
        {
            Debug.LogError("Dosya bulunamadı: " + path);
            return null;
        }
    }

    public void UpdateJson(PlayerData updatedData)
    {
        string json = JsonUtility.ToJson(updatedData, true);
        string path = GetPath();
        File.WriteAllText(path, json);
    }

    public void LoadFromJson(string json)
    {
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
        playerName = playerData.playerName;
        playerAvatarUrl = playerData.playerAvatarUrl;
        playerMoney = playerData.playerMoney;
        transitionSpeed = playerData.transitionSpeed;
        zoomSpeed_FPS = playerData.zoomSpeed_FPS;
        zoomSpeed_TPS = playerData.zoomSpeed_TPS;
        biasSpeed = playerData.biasSpeed;
        targetEngagementShootingSpeed_FPS = playerData.targetEngagementShootingSpeed_FPS;
        targetEngagementShootingSpeed_TPS = playerData.targetEngagementShootingSpeed_TPS;
        freeCrosshairSpeed_FPS = playerData.freeCrosshairSpeed_FPS;
        freeCrosshairSpeed_TPS = playerData.freeCrosshairSpeed_TPS;
        focusSpotSpeed = playerData.focusSpotSpeed;
        FPS_TPS_SwitchKey = playerData.FPS_TPS_SwitchKey;
        engagementButton = playerData.engagementButton;
        fireButton = playerData.fireButton;
        soundValue = playerData.soundValue;
        purchasedIdItems = playerData.purchasedIdItems;
        usedItem = (int)playerData.usedItem;
    }
}