using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviourPunCallbacks
{
    public TMP_Text playerName;
    public Image backgroundImage;
    public Color highlightColor;
    public GameObject leftArrow;
    public GameObject rightArrow;

    private ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
    public Image playerAvatar;
    public Sprite[] avatars;

    private Player player;

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
    }

    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
        player = _player;
        UpdatePlayerItem(player);
    }

    public void ApplyLocalChanges()
    {
        backgroundImage.color = highlightColor;
        leftArrow.SetActive(true);
        rightArrow.SetActive(true);
    }

    public void OnClickLeftArrow()
    {
        int currentAvatarIndex = (int)playerProperties["playerAvatar"];
        int newAvatarIndex = (currentAvatarIndex == 0) ? avatars.Length - 1 : currentAvatarIndex - 1;
        playerProperties["playerAvatar"] = newAvatarIndex;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void OnClickRightArrow()
    {
        int currentAvatarIndex = (int)playerProperties["playerAvatar"];
        int newAvatarIndex = (currentAvatarIndex == avatars.Length - 1) ? 0 : currentAvatarIndex + 1;
        playerProperties["playerAvatar"] = newAvatarIndex;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (player != null && targetPlayer.UserId == player.UserId)
        {
            UpdatePlayerItem(targetPlayer);
        }
    }

    private void UpdatePlayerItem(Player player)
    {
        if (player.CustomProperties.ContainsKey("playerAvatar"))
        {
            int avatarIndex = (int)player.CustomProperties["playerAvatar"];
            playerAvatar.sprite = avatars[avatarIndex];
            playerProperties["playerAvatar"] = avatarIndex;
        }
        else
        {
            playerProperties["playerAvatar"] = 0;
        }
    }
}
