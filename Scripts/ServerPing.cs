using Photon.Pun;
using TMPro;
using UnityEngine;

public class ServerPing : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text pingText;
    
    void Update()
    {
        float ping = PhotonNetwork.GetPing();

        if (ping < 30)
        {
            pingText.color = Color.green;
        }
        else if (ping < 100)
        {
            pingText.color = Color.yellow;
        }
        else
        {
            pingText.color = Color.red;
        }

        pingText.text = $"Ping: {ping} ms";
    }
}
