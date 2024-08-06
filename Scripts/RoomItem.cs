using UnityEngine;
using TMPro;
using Photon.Pun;

public class RoomItem : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text roomName;
    [SerializeField] private TMP_Text roomCount;
    public UIManager manager;

    private void Awake()
    {
        manager = FindObjectOfType<UIManager>();
    }

    public void SetRoomProperties(string _roomName, string _roomCount)
    {
        roomName.text = _roomName;
        if (_roomCount == "5")
        {
            roomCount.color = Color.red;
        }
        else
        {
            roomCount.color = Color.green;
        }
        roomCount.text = _roomCount + "/5";
    }

    public void OnClickItem()
    {
        manager.JoinRoom(roomName.text);
    }
}
