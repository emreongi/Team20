using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneTransitionManager : MonoBehaviourPunCallbacks
{
    private static SceneTransitionManager _instance;
    public static SceneTransitionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("SceneTransitionManager");
                _instance = obj.AddComponent<SceneTransitionManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    private bool isTransitioning = false;
    private HashSet<int> readyPlayers = new HashSet<int>();
    private bool masterClientReady = false;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Application.targetFrameRate = 60; // FPS'i sabitle
    }

    public void MakeSceneTransition(string sceneName)
    {
        if (!isTransitioning)
        {
            Debug.Log("Initiating scene transition to: " + sceneName);
            isTransitioning = true;

            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RPC_TransitionScene", RpcTarget.All, sceneName);
            }
        }
    }

    [PunRPC]
    void RPC_TransitionScene(string sceneName)
    {
        Debug.Log("RPC_TransitionScene called with scene: " + sceneName);
        StartCoroutine(LoadScene(sceneName));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        Debug.Log("Starting coroutine to load scene: " + sceneName);
        yield return new WaitForSeconds(1);

        Debug.Log("Initiating async scene load for: " + sceneName);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            Debug.Log("Loading progress: " + (asyncLoad.progress * 100) + "%");
            if (asyncLoad.progress >= 0.9f)
            {
                Debug.Log("Scene loading reached 90%");
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log("Scene loaded: " + sceneName);
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "SceneLoaded", true } });

        if (PhotonNetwork.IsMasterClient)
        {
            masterClientReady = true;
            photonView.RPC("RPC_MasterClientReady", RpcTarget.AllBuffered);
        }
        else
        {
            photonView.RPC("RPC_PlayerReady", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    [PunRPC]
    void RPC_MasterClientReady()
    {
        masterClientReady = true;

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CheckAllPlayersReady());
        }
    }

    [PunRPC]
    void RPC_PlayerReady(int playerID)
    {
        readyPlayers.Add(playerID);

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CheckAllPlayersReady());
        }
    }

    private IEnumerator CheckAllPlayersReady()
    {
        while (!masterClientReady || !AllPlayersReady())
        {
            yield return null;
        }

        Debug.Log("All players are ready. Starting game...");
        StartGame();
    }

    private bool AllPlayersReady()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!readyPlayers.Contains(player.ActorNumber))
            {
                return false;
            }
        }
        return true;
    }

    private void StartGame()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "SceneLoaded", null } });
        }

        // Oyun başlama işlemleri burada yapılır
        Debug.Log("Game started.");
    }
}