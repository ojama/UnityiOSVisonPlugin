using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class DemoHandDetectorManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The ARCameraManager which will produce frame events.")]
    ARCameraManager m_CameraManager;

    public ARCameraManager cameraManager
    {
        get => m_CameraManager;
        set => m_CameraManager = value;
    }

    [SerializeField]
    Camera m_Cam;

    [Tooltip("Hand point prefab")]
    public GameObject dotPrefab;
    [Tooltip("Transform root for spawning hand points")]
    public Transform dotRoot;
    [Tooltip("Control the interval time of updating hand points in UI")]
    public float updateInterval;
    private bool processing;
    public int maxHandCount = 1;
    [Header("Demo data received from native for testing in Editor")]
    public TextAsset testText;
    [Header("Runtime results")]
    public List<VNDetectedPoint> points;
    public List<RectTransform> pointTransforms;

    // Start is called before the first frame update
    void Start()
    {
        pointTransforms = new List<RectTransform>();
        StartCoroutine(DrawPointsCoroutine());
        Screen.sleepTimeout = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DrawPointsCoroutine()
    {
#if UNITY_EDITOR
        if(testText!=null)
        {
            ParseDetectResult(testText.text);
        }
#endif
        while (true)
        {
            if (points.Count > pointTransforms.Count)
            {
                int n = points.Count - pointTransforms.Count;
                for (int i = 0; i < n; i++)
                {
                    GameObject dot = Instantiate(dotPrefab, dotRoot);
                    pointTransforms.Add(dot.GetComponent<RectTransform>());

                }

            }

            try
            {
                for (int i = 0; i < points.Count; i++)
                {
                    pointTransforms[i].anchoredPosition = new Vector3(Screen.width * ((float)points[i].y - 0.5f), Screen.height * (0.5f - (float)points[i].x));
                    pointTransforms[i].SendMessage("SetText", points[i].pointName);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }

    void OnEnable()
    {
        if (m_CameraManager != null)
        {
            m_CameraManager.frameReceived += OnCameraFrameReceived;
        }

    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        if (processing)
        {
            return;
        }
        Debug.Log("OnCameraFrameReceived");
        processing = true;
        var cameraParams = new XRCameraParams
        {
            zNear = m_Cam.nearClipPlane,
            zFar = m_Cam.farClipPlane,
            screenWidth = Screen.width,
            screenHeight = Screen.height,
            screenOrientation = Screen.orientation
        };

        XRCameraFrame frame;

        if (cameraManager.subsystem.TryGetLatestFrame(cameraParams, out frame))
        {
            if(maxHandCount==1)
            {
                string res = IOSVisonBridgeDetectHand(frame.nativePtr);
                ParseDetectResult(res);
            }
            else if(maxHandCount>1)
            {
                string res = IOSVisonBridgeDetectMultiHand(frame.nativePtr, maxHandCount);
                ParseDetectResult(res);
            }
        }

        processing = false;
    }

    public void ParseDetectResult(string res)
    {
        //Debug.Log(res);
        points = new List<VNDetectedPoint>();
        string[] datas = res.Split('\r');
        try
        {
            for (int i = 0; i < datas.Length; i++)
            {
                string[] cols = datas[i].Split(',');
                if (cols.Length >= 4)
                {
                    VNDetectedPoint point = new VNDetectedPoint(cols[0], cols[1], cols[2], cols[3]);
                    points.Add(point);
                }
            }

            Debug.Log("ParseDetectResult " + points.Count);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }

    }

    [DllImport("__Internal")]
    private static extern void IOSVisonBridgeInit();

    [DllImport("__Internal")]
    private static extern string IOSVisonBridgeDetectHand(IntPtr nativePtr);

    [DllImport("__Internal")]
    private static extern string IOSVisonBridgeDetectMultiHand(IntPtr nativePtr,int maxHandCount);

}
