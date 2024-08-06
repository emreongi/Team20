using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.VFX;

public class RockShattering : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject crystalFragmentable;
    [SerializeField] private float breakDelay = 3.0f; // Kullanıcının ne kadar süre basılı tutması gerektiği
    private float time = 0;
    [HideInInspector] public bool isBroken = false;
    private BoxCollider boxCollider;
    private MeshRenderer meshRenderer;
    private bool isBreaking = false;
    private Camera mainCamera;
    private WaitManager waitManager;


    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        mainCamera = GameObject.Find("Camera").GetComponent<Camera>();
        waitManager = GameObject.Find("Crosshair").GetComponent<WaitManager>();
    }

    void Update()
    {
        // Ekranın ortasından bir ışın gönder
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == boxCollider)
            {
                if (Input.GetMouseButton(0)) // Sol fare tuşu
                {
                    StartBreaking();
                }
                else
                {
                    StopBreaking();
                }
            }
            else
            {
                StopBreaking();
            }
        }
        else
        {
            StopBreaking();
        }

        if (isBreaking)
        {
            time += Time.deltaTime;
            if (time >= breakDelay && !isBroken)
            {
                photonView.RPC("RPC_Disintegrate", RpcTarget.AllBuffered);
            }
        }
        else
        {
            time = 0; // Kullanıcı fareyi bırakırsa süreyi sıfırla
        }
    }

    public void StartBreaking()
    {
        isBreaking = true;
        waitManager.TriggerWait(1f, breakDelay);
    }

    public void StopBreaking()
    {
        isBreaking = false;
    }

    [PunRPC]
    public void RPC_Disintegrate()
    {
        if (!isBroken)
        {
            isBroken = true;

            // PhotonNetwork.Instantiate kullanarak crystalFragmentable objesini oluşturuyoruz
            GameObject newCrystalFragmentable = PhotonNetwork.Instantiate(crystalFragmentable.name, transform.position, transform.rotation);

            Rigidbody[] rbArray = newCrystalFragmentable.GetComponentsInChildren<Rigidbody>();

            StartCoroutine(RigidbodyLock(rbArray));
            boxCollider.enabled = false;
            meshRenderer.enabled = false;
            Destroy(this.gameObject, 1.5f);
        }
    }

    IEnumerator RigidbodyLock(Rigidbody[] rbArray)
    {
        yield return new WaitForSeconds(1.2f);
        foreach (Rigidbody rb in rbArray)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}