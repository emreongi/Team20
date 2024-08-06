using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class CharacterSelection : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] private List<GameObject> playerPrefabs = new List<GameObject>();
    public List<GameObject> playerAvatars = new List<GameObject>();
    private int selectedAvatarIndex = 0;
    private int avatarCount;

    [SerializeField] private TMP_Text playerName;
    private const byte AvatarChangeEventCode = 1;

    void Start()
    {
        avatarCount = playerPrefabs.Count;

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("playerAvatar"))
        {
            selectedAvatarIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"];
        }
        else
        {
            SetAvatar(0);
        }

        CreateAvatars();
        if (playerAvatars.Count > 0 && selectedAvatarIndex < playerAvatars.Count)
        {
            playerAvatars[selectedAvatarIndex].SetActive(true);
        }

        if (playerName != null)
        {
            playerName.text = PhotonNetwork.NickName;
        }
        else
        {
            Debug.LogError("Player Name TMP_Text is not assigned.");
        }

        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                AvatarSelectBack();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                AvatarSelectForward();
            }
        }
    }

    private void CreateAvatars()
    {
        Quaternion rotation = Quaternion.Euler(0, 180, 0);
        foreach (GameObject playerPrefab in playerPrefabs)
        {
            GameObject avatar = Instantiate(playerPrefab, transform.position, rotation);
            avatar.transform.SetParent(transform);
            avatar.SetActive(false);
            playerAvatars.Add(avatar);
        }
    }

    public void AvatarSelectForward()
    {
        if (playerAvatars.Count == 0) return;
        playerAvatars[selectedAvatarIndex].SetActive(false);
        selectedAvatarIndex = (selectedAvatarIndex + 1) % avatarCount;
        SetAvatar(selectedAvatarIndex);
        playerAvatars[selectedAvatarIndex].SetActive(true);
        RaiseAvatarChangeEvent(selectedAvatarIndex);
    }

    public void AvatarSelectBack()
    {
        if (playerAvatars.Count == 0) return;
        playerAvatars[selectedAvatarIndex].SetActive(false);
        selectedAvatarIndex = (selectedAvatarIndex - 1 + avatarCount) % avatarCount;
        SetAvatar(selectedAvatarIndex);
        playerAvatars[selectedAvatarIndex].SetActive(true);
        RaiseAvatarChangeEvent(selectedAvatarIndex);
    }

    private void SetAvatar(int index)
    {
        selectedAvatarIndex = index;
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "playerAvatar", selectedAvatarIndex }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

    private void RaiseAvatarChangeEvent(int index)
    {
        object[] content = new object[] { PhotonNetwork.LocalPlayer.ActorNumber, index };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(AvatarChangeEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == AvatarChangeEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int actorNumber = (int)data[0];
            int avatarIndex = (int)data[1];

            foreach (GameObject platform in GameObject.FindGameObjectsWithTag("PlayerPlatform"))
            {
                CharacterSelection characterSelection = platform.GetComponent<CharacterSelection>();
                if (characterSelection != null && characterSelection.photonView.Owner.ActorNumber == actorNumber)
                {
                    characterSelection.UpdateAvatarSelection(avatarIndex);
                }
            }
        }
    }

    public override void OnJoinedRoom()
    {
        UpdatePlayerAvatars();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("playerAvatar"))
        {
            UpdatePlayerAvatars();
        }
    }

    private void UpdatePlayerAvatars()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("playerAvatar"))
            {
                int avatarIndex = (int)player.CustomProperties["playerAvatar"];
                foreach (GameObject platform in GameObject.FindGameObjectsWithTag("PlayerPlatform"))
                {
                    CharacterSelection characterSelection = platform.GetComponent<CharacterSelection>();
                    if (characterSelection != null && characterSelection.photonView.Owner.ActorNumber == player.ActorNumber)
                    {
                        characterSelection.UpdateAvatarSelection(avatarIndex);
                        characterSelection.UpdatePlayerName(player.NickName);
                    }
                }
            }
        }
    }

    public void UpdateAvatarSelection(int avatarIndex)
    {
        for (int i = 0; i < playerAvatars.Count; i++)
        {
            playerAvatars[i].SetActive(i == avatarIndex);
        }
    }

    public void InitializeForPlayer(Player player)
    {
        if (player.CustomProperties.ContainsKey("playerAvatar"))
        {
            int avatarIndex = (int)player.CustomProperties["playerAvatar"];
            UpdateAvatarSelection(avatarIndex);
        }
        else
        {
            SetAvatar(0);
        }

        UpdatePlayerName(player.NickName);
    }

    public void UpdatePlayerName(string name)
    {
        if (playerName != null)
        {
            playerName.text = name;
        }
        else
        {
            Debug.LogError("Player Name TMP_Text is not assigned.");
        }
    }
}