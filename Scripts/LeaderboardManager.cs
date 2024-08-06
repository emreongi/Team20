using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public GameObject playersHolder;

    [SerializeField] private float refreshRate = 1;

    [SerializeField] private GameObject[] slots;

    [SerializeField] private TMP_Text[] playerNameTexts;
    [SerializeField] private TMP_Text[] playerSkorTexts;

    private int playerSkor;

    void Start()
    {
        InvokeRepeating(nameof(Refresh), 1f, refreshRate);
    }

    public void Refresh()
    {
        foreach (var slot in slots)
        {
            slot.SetActive(false);
        }

        var sortedPlayerList = (from player in PhotonNetwork.PlayerList orderby player.GetScore() descending select player).ToList();

        int i = 0;
        foreach (var player in sortedPlayerList)
        {
            slots[i].SetActive(true);

            if (player.NickName == "")
            {
                player.NickName = "Unnamed";
            }

            playerNameTexts[i].text = player.NickName;
            playerSkorTexts[i].text = player.GetScore().ToString();

            i++;
        }
    }
}
