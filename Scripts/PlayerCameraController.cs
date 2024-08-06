using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using System;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Cinemachine FreeLook")]
    [SerializeField] private string freeLookCameraName = "FreeLook Camera";
    private CinemachineFreeLook cinemachineFreeLook;

    [Header("FPS-TPS")]
    [SerializeField] [Tooltip("Select 'False' for camera viewpoint 'TPS' or True for camera viewpoint FPS.")] private bool isFPS = false;
    [SerializeField] private float transitionSpeed = 0.5f;
    [SerializeField] private int engagementButton = 1;
    [SerializeField] private int fireButton = 0;
    [SerializeField] private KeyCode FPS_TPS_SwitchKey = KeyCode.V;

    [Header("Follow Speed")]
    [SerializeField] private float followSpeed_FPS = 300f;
    [SerializeField] private float followSpeed_TPS = 100f;

    [Header("FPS Zoom")]
    [SerializeField] private float minFOV_FPS = 30f;
    [SerializeField] private float maxFOV_FPS = 40f;
    [SerializeField] private float zoomSpeed_FPS = 0.2f;

    [Header("TPS Zoom")]
    [SerializeField] private float minFOV_TPS = 30f;
    [SerializeField] private float maxFOV_TPS = 40f;
    [SerializeField] private float zoomSpeed_TPS = 0.5f;

    [Header("FPS Orbits")]
    private const float topRigHeight_FPS = 35f;
    private const float topRigRadius_FPS = 0f;
    private const float middleRigHeight_FPS = 35f;
    private const float middileRigRadius_FPS = -6f;
    private const float bottomRigHeight_FPS = 35f;
    private const float bottomRigRadius_FPS = 0f;

    [Header("TPS Orbits")]
    private const float topRigHeight_TPS = 22f;
    private const float topRigRadius_TPS = 38.5f;
    private const float middleRigHeight_TPS = 123f;
    private const float middileRigRadius_TPS = 194f;
    private const float bottomRigHeight_TPS = 0.9f;
    private const float bottomRigRadius_TPS = 11f;

    [Header("TPS Bias")]
    [SerializeField] private float minBias = 0f;
    [SerializeField] private float maxBias = -30f;
    [SerializeField] private float biasSpeed = 0.5f;

    [Header("FPS-TPS Aim")]
    [SerializeField] private float targetEngagementShootingSpeed_FPS = 10f;
    [SerializeField] private float freeCrosshairSpeed_FPS = 20f;
    [SerializeField] private float targetEngagementShootingSpeed_TPS = 10f;
    [SerializeField] private float freeCrosshairSpeed_TPS = 20f;
    private float crosshairSpeed = 10f;
    [SerializeField] private float minY = -10f;
    [SerializeField] private float maxY = 10f;
    [SerializeField] private float focusSpotSpeed = 0.5f;
    [SerializeField] private Transform aimPoint;
    private float targetY;
    private Vector3 aimPointPosition;
    private Vector3 aimZeroPosition;
    private float velocityY = 0f;

    [Header("Crosshair UI FPS-TPS")]
    public GameObject crosshair;
    public WaitManager waitManager;
    [SerializeField] private string crosshairName = "Crosshair";

    [Header("JSON Manager")]
    public JsonManager jsonManager;
    private PlayerData playerData;

    Animator animator;

    void Start()
    {
        jsonManager = FindObjectOfType<JsonManager>();

        cinemachineFreeLook = GameObject.Find(freeLookCameraName).GetComponent<CinemachineFreeLook>();

        crosshair = GameObject.Find(crosshairName);
        waitManager = crosshair.GetComponent<WaitManager>();

        HealthCheck();

        JSONSettingsReading();

        SetupAimPoint();

        SetFollowSpeed();

        waitManager.CrosshairActive(isFPS);

        crosshairSpeed = freeCrosshairSpeed_TPS;

        animator = GetComponent<Animator>();
        
        // Cinemachine Damping Ayarları
        cinemachineFreeLook.m_XAxis.m_MaxSpeed = 300f;
        cinemachineFreeLook.m_YAxis.m_MaxSpeed = 1f;
        cinemachineFreeLook.m_YAxis.m_AccelTime = 0.1f;
        cinemachineFreeLook.m_YAxis.m_DecelTime = 0.1f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(engagementButton)) // Hedefe odaklanma talebi alınıyor.
        {
            TargetingOn();
            animator.SetBool("shooting",true);
        }
        
        if (Input.GetMouseButtonUp(engagementButton)) // Hedefe odaklanma talebi geri alınıyor.
        {
            TargetingOff();
            animator.SetBool("shooting",false);
        }

        if (Input.GetKeyDown(FPS_TPS_SwitchKey)) // Bakış açısı değişiklik talebi alınıyor.
        {
            ToggleViewpoint();
        }

        AimMousePosition();
    }

    // Null değer alabilecek değişkenlerin durumu kontrol ediliyor.
    // Null değer alınırsa uyarı veriliyor.
    void HealthCheck()
    {
        if (!cinemachineFreeLook)
        {
            Debug.LogWarning("FreeLook Camera Not Found");
            return;
        }

        if (!aimPoint)
        {
            Debug.LogError("AimPoint Not Found");
            return;
        }

        if (!crosshair)
        {
            Debug.LogWarning("Crosshair Not Found");
            return;
        }

        if (!jsonManager)
        {
            Debug.LogError("JSON Manager Not Found");
            return;
        }
    }

    // Aim Point nesnesinin Transform özellikleri ayarlanıyor.
    void SetupAimPoint()
    {
        targetY = aimPoint.localPosition.y;
        aimPointPosition = aimPoint.transform.localPosition;
        aimZeroPosition = new Vector3(aimPoint.transform.localPosition.x, aimPoint.transform.localPosition.y, 0);

        aimPoint.localPosition = new Vector3(aimPoint.localPosition.x, aimPoint.localPosition.y, 0);
    }

    // FPS ve TPS seçimine göre kameranın takip etme hızı ayarlanıyor.
    void SetFollowSpeed()
    {
        if (isFPS)
        {
            cinemachineFreeLook.m_XAxis.m_MaxSpeed = followSpeed_FPS;
            cinemachineFreeLook.m_YAxis.m_MaxSpeed = followSpeed_FPS;
        }
        else
        {
            cinemachineFreeLook.m_XAxis.m_MaxSpeed = followSpeed_TPS;
            cinemachineFreeLook.m_YAxis.m_MaxSpeed = followSpeed_TPS;
        }
    }

    // FPS ve TPS seçimine göre Aim Point konumu ayarlanıyor.
    // Crosshair 'in aktiflik durumu TPS ve FPS seçimine göre belirleniyor.
    void ToggleViewpoint()
    {
        isFPS = !isFPS;
        aimPoint.localPosition = isFPS ? aimPointPosition : aimZeroPosition;
        waitManager.CrosshairActive(isFPS);

        SetFollowSpeed();
        StartCoroutine(PerspectiveShifter(transitionSpeed));
    }

    // Crosshair ve Aim Point 'e kamera hem yaklaşıyor hemde odaklanıyor.
    // FPS ve TPS seçimine göre Crosshair 'ın hızı belirleniyor.
    // FPS ve TPS seçimine göre kamera lensinin odaklanma değeri ve buna bağlı değişim hızı belirleniyor.
    // FPS ve TPS seçimine göre Crosshair 'in aktiflik durumu ayarlanıyor.
    void TargetingOn()
    {
        crosshairSpeed = isFPS ? targetEngagementShootingSpeed_FPS : targetEngagementShootingSpeed_TPS;
        StartCoroutine(ZoomCoroutine(isFPS ? minFOV_FPS : minFOV_TPS, isFPS ? zoomSpeed_FPS : zoomSpeed_TPS));
        StartCoroutine(ChangeBiasCoroutine(maxBias, biasSpeed));
        StartCoroutine(FocusPointPositioning(aimPointPosition, focusSpotSpeed));

        if (!isFPS)
        {
            waitManager.CrosshairActive(true);
        }
    }

    // Crosshair ve Aim Point 'e kamera uzaklaşıyor.
    // FPS ve TPS seçimine göre Crosshair 'ın hızı belirleniyor.
    // FPS ve TPS seçimine göre kamera lensinin odaklanma değeri ve buna bağlı değişim hızı belirleniyor.
    // FPS ve TPS seçimine göre Crosshair 'in aktiflik durumu ayarlanıyor.
    void TargetingOff()
    {
        crosshairSpeed = isFPS ? freeCrosshairSpeed_FPS : freeCrosshairSpeed_TPS;
        StartCoroutine(ZoomCoroutine(isFPS ? maxFOV_FPS : maxFOV_TPS, isFPS ? zoomSpeed_FPS : zoomSpeed_TPS));
        StartCoroutine(ChangeBiasCoroutine(minBias, biasSpeed));
        StartCoroutine(FocusPointPositioning(isFPS ? aimPointPosition : aimZeroPosition, focusSpotSpeed));

        if (!isFPS)
        {
            waitManager.CrosshairActive(false);
        }
    }

    // Farenin Y ekseni üzerinde değişimini hesaplayarak Aim Point 'i lokal konumu değiştiriliyor.
    void AimMousePosition()
    {
        float mouseY = Input.GetAxis("Mouse Y");

        targetY += mouseY * 100 * Time.deltaTime;
        targetY = Mathf.Clamp(targetY, minY, maxY);
        float newY = Mathf.SmoothDamp(aimPoint.localPosition.y, targetY, ref velocityY, 0.1f);
        aimPoint.localPosition = new Vector3(aimPoint.localPosition.x, newY, aimPoint.localPosition.z);
    }

    // Yumuşak şekilde kamera lensi odaklanması sağlanıyor. A -> B (*t)
    IEnumerator ZoomCoroutine(float targetFOV, float duration)
    {
        float startFOV = cinemachineFreeLook.m_Lens.FieldOfView;
        float time = 0;

        while (time < duration)
        {
            cinemachineFreeLook.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        cinemachineFreeLook.m_Lens.FieldOfView = targetFOV;
    }

    // Kameranın Bias değerini yumuşak şekilde değiştiriliyor. A -> B (*t)
    IEnumerator ChangeBiasCoroutine(float targetBias, float duration)
    {
        float startBias = cinemachineFreeLook.m_Heading.m_Bias;
        float time = 0;

        while (time < duration)
        {
            cinemachineFreeLook.m_Heading.m_Bias = Mathf.Lerp(startBias, targetBias, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        cinemachineFreeLook.m_Heading.m_Bias = targetBias;
    }

    // Aim Point 'in yumuşak hareketi sağlanıyor. A -> B (*t)
    IEnumerator FocusPointPositioning(Vector3 localPosition, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            aimPoint.localPosition = Vector3.Lerp(aimPoint.localPosition, localPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
    }

    // FreeLook kameranın Orbits değerleri yumuşak değişim yapabilmek için LerpObits fonksiyonuna gönderiliyor.
    IEnumerator PerspectiveShifter(float duration)
    {
        float time = 0;

        while (time < duration)
        {
            if (isFPS)
            {
                LerpOrbits(topRigHeight_FPS, topRigRadius_FPS, middleRigHeight_FPS, middileRigRadius_FPS, bottomRigHeight_FPS, bottomRigRadius_FPS, time / duration);
            }
            else
            {
                LerpOrbits(topRigHeight_TPS, topRigRadius_TPS, middleRigHeight_TPS, middileRigRadius_TPS, bottomRigHeight_TPS, bottomRigRadius_TPS, time / duration);
            }

            time += Time.deltaTime;
            yield return null;
        }
    }

    // Orbits değerleri yumuşak şekilde değiştiriliyor.
    void LerpOrbits(float topHeight, float topRadius, float middleHeight, float middleRadius, float bottomHeight, float bottomRadius, float time)
    {
        cinemachineFreeLook.m_Orbits[0].m_Height = Mathf.Lerp(cinemachineFreeLook.m_Orbits[0].m_Height, topHeight, time);
        cinemachineFreeLook.m_Orbits[0].m_Radius = Mathf.Lerp(cinemachineFreeLook.m_Orbits[0].m_Radius, topRadius, time);

        cinemachineFreeLook.m_Orbits[1].m_Height = Mathf.Lerp(cinemachineFreeLook.m_Orbits[1].m_Height, middleHeight, time);
        cinemachineFreeLook.m_Orbits[1].m_Radius = Mathf.Lerp(cinemachineFreeLook.m_Orbits[1].m_Radius, middleRadius, time);

        cinemachineFreeLook.m_Orbits[2].m_Height = Mathf.Lerp(cinemachineFreeLook.m_Orbits[2].m_Height, bottomHeight, time);
        cinemachineFreeLook.m_Orbits[2].m_Radius = Mathf.Lerp(cinemachineFreeLook.m_Orbits[2].m_Radius, bottomRadius, time);
    }

    // JSON 'dan hassasiyet ayarları çekiliyor.
    void JSONSettingsReading()
    {
        playerData = jsonManager.ReadJson(); // JSON dosyasını okuyan fonk. 'u çalıştırıyoruz.
        if (playerData != null)
        {
            transitionSpeed = playerData.transitionSpeed;
            biasSpeed = playerData.biasSpeed;
            zoomSpeed_FPS = playerData.zoomSpeed_FPS;
            zoomSpeed_TPS = playerData.zoomSpeed_TPS;
            targetEngagementShootingSpeed_FPS = playerData.targetEngagementShootingSpeed_FPS;
            targetEngagementShootingSpeed_TPS = playerData.targetEngagementShootingSpeed_TPS;
            freeCrosshairSpeed_FPS = playerData.freeCrosshairSpeed_FPS;
            freeCrosshairSpeed_TPS = playerData.freeCrosshairSpeed_TPS;
            focusSpotSpeed = playerData.focusSpotSpeed;
            //FPS_TPS_SwitchKey = (KeyCode)Enum.Parse(typeof(KeyCode), playerData.FPS_TPS_SwitchKey);
            engagementButton = playerData.engagementButton;
            fireButton = playerData.fireButton;
        }
    }
}