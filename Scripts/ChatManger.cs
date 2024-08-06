using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManger : MonoBehaviour
{
    [SerializeField] private TMP_InputField messageInput;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private Transform content;
    [SerializeField] private Image microphone;
    [SerializeField] private GameObject voice;

    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void SendMessage()
    {
        if (messageInput.text != "")
        {
            photonView.RPC("GetMessage", RpcTarget.All, PhotonNetwork.NickName + " : " + messageInput.text);
            messageInput.text = "";
        }
    }

    [PunRPC]
    public void GetMessage(string receiveMessage)
    {
        GameObject message = Instantiate(messagePrefab, Vector3.zero, Quaternion.identity, content);
        message.GetComponent<Message>().messageText.text = receiveMessage;
    }
}