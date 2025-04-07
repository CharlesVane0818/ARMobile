using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Management;
using System.Collections;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;
//using UnityEngine.XR.ARFoundation;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    [SerializeField] private ARSession arSession;
    [SerializeField] private GameObject xrOrigin;
    [SerializeField] private Vector2Int defaultResolution = new(1920, 1080);
    private ARCameraManager arCameraManager;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void StartAR()
    {
        StartCoroutine(InitializeARSession());
    }

    private IEnumerator InitializeARSession()
    {
        if (ARSession.state == ARSessionState.None || ARSession.state == ARSessionState.CheckingAvailability)
        {
            yield return ARSession.CheckAvailability();
        }

        if (ARSession.state == ARSessionState.Unsupported)
        {
            // 处理不支持AR的情况
            yield break;
        }

        // 初始化XR子系统
        var xrManagerSettings = XRGeneralSettings.Instance.Manager;
        yield return xrManagerSettings.InitializeLoader();

        if (xrManagerSettings.activeLoader == null)
        {
            Debug.LogError("初始化XR Loader失败");
            yield break;
        }

        // 启动子系统
        xrManagerSettings.StartSubsystems();

        // 重置跟踪配置
        if (arSession != null)
        {
            arSession.Reset();
            yield return new WaitForEndOfFrame();
        }

        // 激活ARSession和XROrigin
        if (xrOrigin != null)
        {
            xrOrigin.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            xrOrigin.SetActive(false);
            yield return null;
            xrOrigin.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
        if (arSession != null)
        {
            arSession.enabled = true;
            yield return new WaitForEndOfFrame();
        }

        SetDefaultResolution();
    }

    private void SetDefaultResolution()
    {
        if (arCameraManager != null)
        {
            if (arCameraManager.currentConfiguration.HasValue)
            {
                //arCameraManager.GetConfigurations(Allocator.Temp)[0];
                var desiredResolution = new Vector2Int(defaultResolution.x, defaultResolution.y);
                foreach (var config in arCameraManager.GetConfigurations(Allocator.Temp))
                {
                    if (config.resolution == desiredResolution)
                    {
                        arCameraManager.currentConfiguration = config;
                        Debug.Log($"AR相机分辨率已设置为：{desiredResolution}");
                        break;
                    }
                }
            }
        }


    }

    private void Start()
    {
        arCameraManager = xrOrigin.GetComponent<ARCameraManager>();
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        xrOrigin.SetActive(false);
        arSession.enabled = false;
        arSession.Reset();  // 实例方法调用
        Application.Quit();
#endif
    }
}