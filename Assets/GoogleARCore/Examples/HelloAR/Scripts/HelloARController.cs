

namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.EventSystems;

#if UNITY_EDITOR
   
    using Input = InstantPreviewInput;
#endif

   
    public class HelloARController : MonoBehaviour
    {
       
        public Camera FirstPersonCamera;

        
        public GameObject DetectedPlanePrefab;

       
        public GameObject AndyPlanePrefab;

        public GameObject AndyRamp;
       
        public GameObject AndyPointPrefab;

       
        private const float k_ModelRotation = 180.0f;

       
        private bool m_IsQuitting = false;

      
        public DragonControls dragonControls;
        int spawned = 0;

        Color[] colors = new Color[6];


    void Start()
     {
         colors[0] = Color.cyan;
         colors[1] = Color.red;
         colors[2] = Color.green;
         colors[3] = new Color(255, 165, 0);
         colors[4] = Color.yellow;
         colors[5] = Color.magenta;
     }
        
        public void Update()
        {
            _UpdateApplicationLifecycle();

            
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit)) 
            {
                
                
                if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                        hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {
                    if(spawned == 0)
                    {
                        var andyObject = Instantiate(AndyPlanePrefab, hit.Pose.position, hit.Pose.rotation);
                        var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                        dragonControls.Dragon = andyObject.transform;
                        dragonControls.gameObject.SetActive(true);
                        andyObject.transform.parent = anchor.transform;
                        spawned = 1;
                    }else if(spawned == 1)
                    {
                        var andyObject = Instantiate(AndyRamp, hit.Pose.position, hit.Pose.rotation);
                        var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                        andyObject.transform.parent = anchor.transform;
                        spawned = 2;
                    }
                    else if(spawned == 2)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = FirstPersonCamera.transform.TransformPoint(0, 0, 0.5f);
                        cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        cube.GetComponent<Renderer>().material.color = colors[Random.Range(0, colors.Length)];
                        cube.AddComponent<Rigidbody>();
                        cube.GetComponent<Rigidbody>().AddForce(FirstPersonCamera.transform.TransformDirection(0, 1f, 2f),ForceMode.Impulse); 
                    }
                }
            }
        }

        
        private void _UpdateApplicationLifecycle()
        {
           
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (m_IsQuitting)
            {
                return;
            }

            
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage(
                    "ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

       
        private void _DoQuit()
        {
            Application.Quit();
        }

       
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity =
                unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject =
                        toastClass.CallStatic<AndroidJavaObject>(
                            "makeText", unityActivity, message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}
