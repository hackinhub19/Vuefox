

namespace GoogleARCore.Examples.CloudAnchors
{
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.Networking;

   
    public class CloudAnchorsExampleController : MonoBehaviour
    {
        [Header("ARCore")]

        
        public NetworkManagerUIController UIController;

        public GameObject ARCoreRoot;

        
        public ARCoreWorldOriginHelper ARCoreWorldOriginHelper;

        [Header("ARKit")]

        public GameObject ARKitRoot;

        
        public Camera ARKitFirstPersonCamera;

        
        private ARKitHelper m_ARKit = new ARKitHelper();

       
        private bool m_IsOriginPlaced = false;

        
        private bool m_AnchorAlreadyInstantiated = false;

        private bool m_AnchorFinishedHosting = false;

       
        private bool m_IsQuitting = false;

        
        private Component m_WorldOriginAnchor = null;

        
        private Pose? m_LastHitPose = null;

       
        private ApplicationMode m_CurrentMode = ApplicationMode.Ready;

        
#pragma warning disable 618
        private NetworkManager m_NetworkManager;
#pragma warning restore 618

        private bool m_MatchStarted = false;

       
        public enum ApplicationMode
        {
            Ready,
            Hosting,
            Resolving,
        }

        
        public void Start()
        {
#pragma warning disable 618
            m_NetworkManager = UIController.GetComponent<NetworkManager>();
#pragma warning restore 618

           
            gameObject.name = "CloudAnchorsExampleController";
            ARCoreRoot.SetActive(false);
            ARKitRoot.SetActive(false);
            _ResetStatus();
        }

        
        public void Update()
        {
            _UpdateApplicationLifecycle();


            if (m_CurrentMode != ApplicationMode.Hosting &&
                m_CurrentMode != ApplicationMode.Resolving)
            {
                return;
            }

            
            if (m_CurrentMode == ApplicationMode.Resolving && !m_IsOriginPlaced)
            {
                return;
            }

           
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            TrackableHit arcoreHitResult = new TrackableHit();
            m_LastHitPose = null;

           
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                if (ARCoreWorldOriginHelper.Raycast(touch.position.x, touch.position.y,
                        TrackableHitFlags.PlaneWithinPolygon, out arcoreHitResult))
                {
                    m_LastHitPose = arcoreHitResult.Pose;
                }
            }
            else
            {
                Pose hitPose;
                if (m_ARKit.RaycastPlane(
                    ARKitFirstPersonCamera, touch.position.x, touch.position.y, out hitPose))
                {
                    m_LastHitPose = hitPose;
                }
            }

            
            if (m_LastHitPose != null)
            {
                
                if (_CanPlaceStars())
                {
                    _InstantiateStar();
                }
                else if (!m_IsOriginPlaced && m_CurrentMode == ApplicationMode.Hosting)
                {
                    if (Application.platform != RuntimePlatform.IPhonePlayer)
                    {
                        m_WorldOriginAnchor =
                            arcoreHitResult.Trackable.CreateAnchor(arcoreHitResult.Pose);
                    }
                    else
                    {
                        m_WorldOriginAnchor = m_ARKit.CreateAnchor(m_LastHitPose.Value);
                    }

                    SetWorldOrigin(m_WorldOriginAnchor.transform);
                    _InstantiateAnchor();
                    OnAnchorInstantiated(true);
                }
            }
        }

        
        public void SetWorldOrigin(Transform anchorTransform)
        {
            if (m_IsOriginPlaced)
            {
                Debug.LogWarning("The World Origin can be set only once.");
                return;
            }

            m_IsOriginPlaced = true;

            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                ARCoreWorldOriginHelper.SetWorldOrigin(anchorTransform);
            }
            else
            {
                m_ARKit.SetWorldOrigin(anchorTransform);
            }
        }

        
        public void OnEnterHostingModeClick()
        {
            if (m_CurrentMode == ApplicationMode.Hosting)
            {
                m_CurrentMode = ApplicationMode.Ready;
                _ResetStatus();
                return;
            }

            m_CurrentMode = ApplicationMode.Hosting;
            _SetPlatformActive();
        }

        
        public void OnEnterResolvingModeClick()
        {
            if (m_CurrentMode == ApplicationMode.Resolving)
            {
                m_CurrentMode = ApplicationMode.Ready;
                _ResetStatus();
                return;
            }

            m_CurrentMode = ApplicationMode.Resolving;
            _SetPlatformActive();
        }

       
        public void OnAnchorInstantiated(bool isHost)
        {
            if (m_AnchorAlreadyInstantiated)
            {
                return;
            }

            m_AnchorAlreadyInstantiated = true;
            UIController.OnAnchorInstantiated(isHost);
        }

       
        public void OnAnchorHosted(bool success, string response)
        {
            m_AnchorFinishedHosting = success;
            UIController.OnAnchorHosted(success, response);
        }

        
        public void OnAnchorResolved(bool success, string response)
        {
            UIController.OnAnchorResolved(success, response);
        }

       
        private void _InstantiateAnchor()
        {
            
            GameObject.Find("LocalPlayer").GetComponent<LocalPlayerController>()
                .SpawnAnchor(Vector3.zero, Quaternion.identity, m_WorldOriginAnchor);
        }

       
        private void _InstantiateStar()
        {
            
            GameObject.Find("LocalPlayer").GetComponent<LocalPlayerController>()
                .CmdSpawnStar(m_LastHitPose.Value.position, m_LastHitPose.Value.rotation);
        }

        
        private void _SetPlatformActive()
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                ARCoreRoot.SetActive(true);
                ARKitRoot.SetActive(false);
            }
            else
            {
                ARCoreRoot.SetActive(false);
                ARKitRoot.SetActive(true);
            }
        }

        
        private bool _CanPlaceStars()
        {
            if (m_CurrentMode == ApplicationMode.Resolving)
            {
                return m_IsOriginPlaced;
            }

            if (m_CurrentMode == ApplicationMode.Hosting)
            {
                return m_IsOriginPlaced && m_AnchorFinishedHosting;
            }

            return false;
        }

       
        private void _ResetStatus()
        {
           
            m_CurrentMode = ApplicationMode.Ready;
            if (m_WorldOriginAnchor != null)
            {
                Destroy(m_WorldOriginAnchor.gameObject);
            }

            m_WorldOriginAnchor = null;
        }

        
        private void _UpdateApplicationLifecycle()
        {
            if (!m_MatchStarted && m_NetworkManager.IsClientConnected())
            {
                m_MatchStarted = true;
            }

            
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            var sleepTimeout = SleepTimeout.NeverSleep;

#if !UNITY_IOS
           
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                sleepTimeout = lostTrackingSleepTimeout;
            }
#endif

            Screen.sleepTimeout = sleepTimeout;

            if (m_IsQuitting)
            {
                return;
            }

            
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                UIController.ShowErrorMessage(
                    "Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 5.0f);
            }
            else if (Session.Status.IsError())
            {
                UIController.ShowErrorMessage(
                    "ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 5.0f);
            }
            else if (m_MatchStarted && !m_NetworkManager.IsClientConnected())
            {
                UIController.ShowErrorMessage(
                    "Network session disconnected!  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 5.0f);
            }
        }

      
        private void _DoQuit()
        {
            Application.Quit();
        }
    }
}
