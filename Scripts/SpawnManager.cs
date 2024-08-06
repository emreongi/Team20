using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] playerPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject freeLookCameraPrefab;
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject inventoryPanel;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] enemySpawnPoints;

    [SerializeField] private GameObject crystalUnbreakablePrefab;
    [SerializeField] private Transform[] crystalSpawnPoints;

    [SerializeField] private GameObject friendAlien1;
    [SerializeField] private GameObject friendAlien2;
    [SerializeField] private Transform[] alien1points;
    [SerializeField] private Transform[] alien2points;

    [SerializeField] private GameObject oxygen;
    [SerializeField] private Transform[] oxygenSpawnPoints;

    private void Start()
    {
        if (inventoryPanel != null && canvas != null)
        {
            //Instantiate(inventoryPanel, canvas);
        }

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned.");
            return;
        }

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogWarning("This script should only be run when connected to Photon.");
            return;
        }

        SpawnLocalPlayer();

        if (PhotonNetwork.IsMasterClient)
        {
            InvokeRepeating(nameof(EnemySpawner), 0f, 90f);
            SpawnInitialObjects();
        }
    }

    private void SpawnLocalPlayer()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("playerAvatar", out object avatarIndexObj))
        {
            int avatarIndex = (int)avatarIndexObj;
            if (avatarIndex < 0 || avatarIndex >= playerPrefabs.Length)
            {
                Debug.LogError("Invalid player avatar index for player: " + PhotonNetwork.LocalPlayer.NickName);
                return;
            }

            int randomNumber = UnityEngine.Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomNumber];

            GameObject spawnedPlayer = PhotonNetwork.Instantiate(playerPrefabs[avatarIndex].name, spawnPoint.position, Quaternion.identity);
            if (spawnedPlayer == null)
            {
                Debug.LogError("Failed to instantiate player prefab for: " + PhotonNetwork.LocalPlayer.NickName);
                return;
            }

            spawnedPlayer.name = PhotonNetwork.LocalPlayer.NickName;
            Debug.Log("Player instantiated: " + spawnedPlayer.name + " at " + spawnPoint.position);

            // Add PhotonTransformView if not already present
            if (spawnedPlayer.GetComponent<PhotonTransformView>() == null)
            {
                var photonTransformView = spawnedPlayer.AddComponent<PhotonTransformView>();
                photonTransformView.m_SynchronizePosition = true;
                photonTransformView.m_SynchronizeRotation = true;
                photonTransformView.m_SynchronizeScale = false;
            }

            if (freeLookCameraPrefab != null)
            {
                GameObject freeLookCamera = Instantiate(freeLookCameraPrefab);
                freeLookCamera.name = "FreeLook Camera";

                CinemachineFreeLook cinemachineFreeLook = freeLookCamera.GetComponent<CinemachineFreeLook>();
                if (cinemachineFreeLook != null)
                {
                    cinemachineFreeLook.Follow = spawnedPlayer.transform;
                    cinemachineFreeLook.LookAt = spawnedPlayer.transform.Find("AimPoint");
                }
            }
        }
        else
        {
            Debug.LogError("Player avatar index not found for player: " + PhotonNetwork.LocalPlayer.NickName);
        }
    }

    private void EnemySpawner()
    {
        int randomNumber = Random.Range(0, enemySpawnPoints.Length);
        Transform spawnPoint = enemySpawnPoints[randomNumber];
        PhotonNetwork.Instantiate(enemyPrefab.name, spawnPoint.position, Quaternion.identity);
    }

    private void SpawnInitialObjects()
    {
        SpawnCrystalPoint();
        SpawnAliens();
        SpawnOxygen();
        SpawnEnemyAliens();
    }

    private void SpawnCrystalPoint()
    {
        foreach (Transform spawnPoint in crystalSpawnPoints)
        {
            PhotonNetwork.Instantiate(crystalUnbreakablePrefab.name, spawnPoint.position, Quaternion.identity);
        }
    }

    private void SpawnAliens()
    {
        foreach (Transform spawnpoint in alien1points)
        {
            PhotonNetwork.Instantiate(friendAlien1.name, spawnpoint.position, Quaternion.identity);
        }
        foreach (Transform spawnpoint in alien2points)
        {
            PhotonNetwork.Instantiate(friendAlien2.name, spawnpoint.position, Quaternion.identity);
        }
    }

    private void SpawnOxygen()
    {
        foreach (Transform spawnpoint in oxygenSpawnPoints)
        {
            PhotonNetwork.Instantiate(oxygen.name, spawnpoint.position, Quaternion.identity);
        }
    }

    private void SpawnEnemyAliens()
    {
        foreach (Transform enemySpawnPoint in enemySpawnPoints)
        {
            PhotonNetwork.Instantiate(enemyPrefab.name, enemySpawnPoint.position, Quaternion.identity);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnInitialObjects();
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnInitialObjects();
        }
    }
}