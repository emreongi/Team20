using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OffScreenPlayerIndicator : MonoBehaviourPunCallbacks
{
    [Header("Camera")]
    private Camera mainCamera;
    [SerializeField] private string cameraName = "Camera";
    [SerializeField] private Transform parent;

    [Header("Target Objects")]
    [SerializeField] private string targetTagName;

    [Header("Target/Follow")]
    [SerializeField] private bool isTargetFollow = false; // Oyun başladığında takip aktif olacak
    private GameObject[] targetObjects;
    [SerializeField] private GameObject markerPrefab;
    private Dictionary<GameObject, GameObject> markerIcons = new Dictionary<GameObject, GameObject>();

    [Header("Sprites")]
    [SerializeField] private Sprite squareSprite;
    [SerializeField] private Sprite arrowSprite;

    [Header("Texts")]
    [SerializeField] private string nameTextObjectName = "Name";
    [SerializeField] private string distanceTextObjectName = "Distance";

    void Start()
    {
        mainCamera = GameObject.Find(cameraName).GetComponent<Camera>();
        parent = GameObject.Find("CanvasGeneral").transform;

        Invoke("StartFollowingPlayers", 5);
    }

    void Update()
    {
        if (targetObjects == null) return;

        if (isTargetFollow)
        {
            foreach (var targetObject in targetObjects)
            {
                if (targetObject && mainCamera)
                {
                    GameObject markerIcon;
                    if (markerIcons.TryGetValue(targetObject, out markerIcon))
                    {
                        TargetFollow(targetObject, markerIcon);
                    }
                    else
                    {
                        // Hedef nesne için marker ikonu yoksa oluştur
                        markerIcon = Instantiate(markerPrefab, parent);
                        markerIcons.Add(targetObject, markerIcon);

                        // Hedef target ismi yazılıyor.
                        TMP_Text nameText = markerIcon.transform.Find(nameTextObjectName).GetComponent<TMP_Text>();
                        nameText.text = targetObject.GetComponent<PhotonView>().Owner.NickName;

                        // İlk mesafeyi ayarla
                        TMP_Text distanceText = markerIcon.transform.Find(distanceTextObjectName).GetComponent<TMP_Text>();
                        distanceText.text = (Vector3.Distance(targetObject.transform.position, mainCamera.transform.position) / 10 ).ToString("F1") + "m";
                    }
                }
                else
                {
                    Debug.LogError("Target Object or Main Camera is not connected.");
                }
            }

            // Marker ikonlarını güncelle
            foreach (var pair in markerIcons)
            {
                if (pair.Key && pair.Key.activeSelf)
                {
                    TMP_Text distanceText = pair.Value.transform.Find(distanceTextObjectName).GetComponent<TMP_Text>();
                    distanceText.text = (Vector3.Distance(pair.Key.transform.position, mainCamera.transform.position) / 10 ).ToString("F1") + "m";
                }
            }

            // Yok olan hedef nesnelerin marker ikonlarını temizle
            List<GameObject> targetsToRemove = new List<GameObject>();
            foreach (var pair in markerIcons)
            {
                if (!pair.Key || pair.Key.activeSelf == false)
                {
                    Destroy(pair.Value);
                    targetsToRemove.Add(pair.Key);
                }
            }
            foreach (var targetToRemove in targetsToRemove)
            {
                markerIcons.Remove(targetToRemove);
            }
        }
    }

    // Oyuncu(lar) odadan ayrıldığında onları takip eden MarkerIcon nesneleride yok edilecek.
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        RemoveMarkerIconForPlayer(otherPlayer);
    }
    
    // Oyuncu(lar) oda ile bağlantıları kesildiğinde onları takip eden MarkerIcon nesneleride yok edilecek.
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        RemoveMarkerIconForPlayer(targetPlayer);
    }

    // MarkerIcon 'ların kotnrollü şekilde silinmesini sağlıyor.
    private void RemoveMarkerIconForPlayer(Player player)
    {
        foreach (var targetObject in targetObjects)
        {
            PhotonView photonView = targetObject.GetComponent<PhotonView>();
            if (photonView != null && photonView.Owner == player)
            {
                GameObject markerIcon;
                if (markerIcons.TryGetValue(targetObject, out markerIcon))
                {
                    Destroy(markerIcon);
                    markerIcons.Remove(targetObject);
                    break;
                }
            }
        }

        // Hedefleri tekrar bul ve güncelle
        TargetFind();
    }

    // İlgili taga sahip nesneleri bul
    void TargetFind()
    {
        GameObject[] allTargetObjects = GameObject.FindGameObjectsWithTag(targetTagName);

        List<GameObject> targetList = new List<GameObject>();
        foreach (GameObject target in allTargetObjects)
        {
            PhotonView photonView = target.GetComponent<PhotonView>();
            if (photonView != null)
            {
                if (!photonView.IsMine)
                {
                    targetList.Add(target);
                }
                else
                {
                    Debug.Log("Skipping local player object: " + target.name);
                }
            }
        }
        targetObjects = targetList.ToArray();
    }

    // MarkerIcon 'nun hedefi tekip etmesini sağlar.
    void TargetFollow(GameObject targetObject, GameObject markerIcon)
    {
        Vector3 positionOnScreen = mainCamera.WorldToScreenPoint(targetObject.transform.position);

        // Kamera tarafından görülmeyen nesne
        if (positionOnScreen.z < 0)
        {
            positionOnScreen *= -1;
        }

        if (positionOnScreen.x < 0 || positionOnScreen.x > Screen.width || positionOnScreen.y < 0 || positionOnScreen.y > Screen.height)
        {
            // Alanın dışında ise ok ikonu kullanılsın
            Image markerSprite = markerIcon.GetComponent<Image>();
            markerSprite.sprite = arrowSprite;

            Vector3 iconPositionDetermination = new Vector3(Mathf.Clamp(positionOnScreen.x, 0, Screen.width), Mathf.Clamp(positionOnScreen.y, 0, Screen.height), 0);
            markerIcon.transform.position = iconPositionDetermination;
            LookAtTarget(targetObject, markerIcon, true); // Hedefe bak
        }
        else
        {
            // Alanın içinde ise kutu ikonu kullanılsın
            Image markerSprite = markerIcon.GetComponent<Image>();
            markerSprite.sprite = squareSprite;

            Vector3 iconPosition = new Vector3(positionOnScreen.x, positionOnScreen.y, 0);
            markerIcon.transform.position = iconPosition;
            LookAtTarget(targetObject, markerIcon, false); // Hedefe bakma
        }
    }

    // Hedef taglı nesnein ekranın dışında iken MarkerIcon 'nun hedefe doğru bakması sağlanır.
    void LookAtTarget(GameObject targetObject, GameObject markerIcon, bool isLookAtTarget)
    {
        if (isLookAtTarget) // Ekranın dışında iken
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(targetObject.transform.position);
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Vector3 dir = (screenPos - screenCenter).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            markerIcon.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else // Ekranın içinde iken
        {
            markerIcon.transform.rotation = Quaternion.identity;
        }
    }

    // Oyun başladığında oyuncuları takip etmeye başla
    public void StartFollowingPlayers()
    {
        TargetFind();
        isTargetFollow = true;
    }
}