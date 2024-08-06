using UnityEngine;
using Photon.Pun;

public class LoggingManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject loggingPrefab;
    [HideInInspector] public GameObject triggerObject;
    [HideInInspector] public LoggingTrigger loggingTrigger;
    [SerializeField] private Transform content;
    private string playerName;
    private int loggingIndex = 0;

    [SerializeField] private GameObject loggingOpenObject;
    [SerializeField] private GameObject loggingCloseObject;
    [SerializeField] private KeyCode keyCode = KeyCode.KeypadEnter;
    private bool isPanelActive = false;

    void Start()
    {
        playerName = PhotonNetwork.NickName;

        if (!photonView)
        {
            Debug.LogError("PhotonView component is missing.");
        }

        if (loggingTrigger == null)
        {
            Debug.LogError("LoggingTrigger component is not assigned.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            isPanelActive = !isPanelActive;
            loggingOpenObject.SetActive(isPanelActive);
            loggingCloseObject.SetActive(!isPanelActive);
        }
    }

    public void TriggerObjectDetection()
    {
        if (loggingTrigger == null)
        {
            Debug.LogError("LoggingTrigger is not assigned.");
            return;
        }

        if (loggingTrigger.loggingData == null)
        {
            Debug.LogError("Logging data is not available.");
            return;
        }

        if (string.IsNullOrEmpty(loggingTrigger.loggingData.description))
        {
            Debug.LogError("Logging description is empty or null.");
            return;
        }

        loggingIndex++;
        photonView.RPC("AddLogging", RpcTarget.AllBuffered, playerName, loggingIndex, loggingTrigger.loggingData.description);
        PhotonNetwork.Destroy(triggerObject.gameObject);
    }

    [PunRPC]
    void AddLogging(string playerName, int index, string description)
    {
        if (content == null)
        {
            Debug.LogError("Content transform is not assigned.");
            return;
        }

        GameObject newLogging = Instantiate(loggingPrefab, Vector3.zero, Quaternion.identity, content);
        newLogging.GetComponent<Logging>().SetLoggingProperties(playerName, index, description);
    }
}