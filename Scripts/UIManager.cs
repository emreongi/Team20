using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(JsonManager))]
public class UIManager : MonoBehaviourPunCallbacks
{
    [Header("Lobby Panel")]
    public TMP_InputField roomNameInput;
    [SerializeField] private GameObject platformDef;

    [Header("Room List")]
    [SerializeField] private RoomItem roomItemPrefab;
    private List<RoomItem> roomItemsList = new List<RoomItem>();
    [SerializeField] private Transform content;

    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private List<Vector3> spawnPoints = new List<Vector3>();

    [Header("UIs")]
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject leaveButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button createButton;

    private JsonManager jsonManager;
    private PlayerData playerData;

    [SerializeField] private GameObject infopanel;
    [SerializeField] private GameObject Settingspanel;

    [SerializeField] private GameObject informationbutton;
    [SerializeField] private GameObject marketing;
    [SerializeField] private GameObject settings;

    private Button infobutton;
    private Button martketingbutton;
    private Button settingsButton;

    private void Awake()
    {
        infobutton= informationbutton.GetComponent<Button>();
        martketingbutton = marketing.GetComponent<Button>();
        settingsButton = settings.GetComponent<Button>();
    }
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        jsonManager = GetComponent<JsonManager>();
        playerData = jsonManager.ReadJson();
        PhotonNetwork.NickName = playerData.playerName;
    }

    public void OnClickCreateRoom()
    {
        if (roomNameInput.text.Length > 0)
        {
            PhotonNetwork.CreateRoom(roomNameInput.text, new RoomOptions { MaxPlayers = 5 });
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created Successfully");
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            platformDef.SetActive(false);

            startButton.SetActive(true);
            leaveButton.SetActive(true);

            joinButton.interactable = false;
            createButton.interactable = false;
            marketing.SetActive(false);
        }

        else
        {
            platformDef.SetActive(false);

            leaveButton.SetActive(true);

            joinButton.interactable = false;
            createButton.interactable = false;
            marketing.SetActive(false) ;
        }

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Vector3 spawnPosition;

       

        switch (playerCount)
        {
            case 1:
                spawnPosition = spawnPoints[0]; // Ortadaki pozisyon (3 numara)
                break;
            case 2:
                spawnPosition = spawnPoints[1]; // 2 numara
                break;
            case 3:
                spawnPosition = spawnPoints[2]; // 4 numara
                break;
            case 4:
                spawnPosition = spawnPoints[3]; // 1 numara
                break;
            case 5:
                spawnPosition = spawnPoints[4]; // 5 numara
                break;
            default:
                Debug.LogError("Invalid number of players in the room.");
                return;
        }

        GameObject platform = PhotonNetwork.Instantiate(platformPrefab.name, spawnPosition, Quaternion.identity);
        platform.name = "Platform_" + PhotonNetwork.LocalPlayer.ActorNumber;
        platform.tag = "PlayerPlatform";

        CharacterSelection characterSelection = platform.GetComponent<CharacterSelection>();
        if (characterSelection != null)
        {
            characterSelection.InitializeForPlayer(PhotonNetwork.LocalPlayer);
        }

        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "hasAvatar", true }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

        UpdatePlayerAvatars();
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.JoinLobby();

        platformDef.SetActive(true);

        joinButton.interactable = true;
        createButton.interactable = true;
        startButton.SetActive(false);
        leaveButton.SetActive(false);
        marketing.SetActive(true) ;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoomList(roomList);
    }

    private void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                continue;
            }

            RoomItem newRoom = Instantiate(roomItemPrefab, content);
            newRoom.SetRoomProperties(room.Name, room.PlayerCount.ToString());
            roomItemsList.Add(newRoom);
        }
    }

    public void JoinRoomName()
    {
        JoinRoom(roomNameInput.text);
    }

    public void JoinRoom(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Debug.LogError("JoinRoom failed. Client is not in lobby.");
        }
    }

    public void OnClickStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_LoadGameScene", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_LoadGameScene()
    {
        PhotonNetwork.LoadLevel("Game");
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
    public void InformationButton()
    {
        infopanel.SetActive(true);
        martketingbutton.interactable = false;
        settingsButton.interactable=false;
    }

    public void CloseInfo()
    {
        infopanel.SetActive(false);
        martketingbutton.interactable = true;
        settingsButton.interactable = true;
    }
    public void OpenSettings()
    {
        Settingspanel.SetActive(true);
        infobutton.interactable = false;
        martketingbutton.interactable = false;
    }
    public void CloseSettings()
    {
        Settingspanel.SetActive(false) ;
        infobutton.interactable = true;
        martketingbutton.interactable = true;
    }
}