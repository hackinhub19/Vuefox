

namespace GoogleARCore.Examples.CloudAnchors
{
    using GoogleARCore;
    using GoogleARCore.CrossPlatform;
    using UnityEngine;
    using UnityEngine.Networking;

    
#pragma warning disable 618
    public class AnchorController : NetworkBehaviour
#pragma warning restore 618
    {
       
#pragma warning disable 618
        [SyncVar(hook = "_OnChangeId")]
#pragma warning restore 618
        private string m_CloudAnchorId = string.Empty;

       
        private bool m_IsHost = false;

       
        private bool m_ShouldResolve = false;

        
        private CloudAnchorsExampleController m_CloudAnchorsExampleController;

       
        public void Start()
        {
            m_CloudAnchorsExampleController =
                GameObject.Find("CloudAnchorsExampleController")
                    .GetComponent<CloudAnchorsExampleController>();
        }

        
        public override void OnStartClient()
        {
            if (m_CloudAnchorId != string.Empty)
            {
                m_ShouldResolve = true;
            }
        }

        
        public void Update()
        {
            if (m_ShouldResolve)
            {
                _ResolveAnchorFromId(m_CloudAnchorId);
            }
        }

        
#pragma warning disable 618
        [Command]
#pragma warning restore 618
        public void CmdSetCloudAnchorId(string cloudAnchorId)
        {
            m_CloudAnchorId = cloudAnchorId;
        }

       
        public string GetCloudAnchorId()
        {
            return m_CloudAnchorId;
        }

       
        public void HostLastPlacedAnchor(Component lastPlacedAnchor)
        {
            m_IsHost = true;

#if !UNITY_IOS
            var anchor = (Anchor)lastPlacedAnchor;
#elif ARCORE_IOS_SUPPORT
            var anchor = (UnityEngine.XR.iOS.UnityARUserAnchorComponent)lastPlacedAnchor;
#endif

#if !UNITY_IOS || ARCORE_IOS_SUPPORT
            XPSession.CreateCloudAnchor(anchor).ThenAction(result =>
            {
                if (result.Response != CloudServiceResponse.Success)
                {
                    Debug.Log(string.Format("Failed to host Cloud Anchor: {0}", result.Response));

                    m_CloudAnchorsExampleController.OnAnchorHosted(
                        false, result.Response.ToString());
                    return;
                }

                Debug.Log(string.Format(
                    "Cloud Anchor {0} was created and saved.", result.Anchor.CloudId));
                CmdSetCloudAnchorId(result.Anchor.CloudId);

                m_CloudAnchorsExampleController.OnAnchorHosted(true, result.Response.ToString());
            });
#endif
        }

       
        private void _ResolveAnchorFromId(string cloudAnchorId)
        {
            m_CloudAnchorsExampleController.OnAnchorInstantiated(false);

            
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }

            m_ShouldResolve = false;

            XPSession.ResolveCloudAnchor(cloudAnchorId).ThenAction(
                (System.Action<CloudAnchorResult>)(result =>
                    {
                        if (result.Response != CloudServiceResponse.Success)
                        {
                            Debug.LogError(string.Format(
                                "Client could not resolve Cloud Anchor {0}: {1}",
                                cloudAnchorId, result.Response));

                            m_CloudAnchorsExampleController.OnAnchorResolved(
                                false, result.Response.ToString());
                            m_ShouldResolve = true;
                            return;
                        }

                        Debug.Log(string.Format(
                            "Client successfully resolved Cloud Anchor {0}.",
                            cloudAnchorId));

                        m_CloudAnchorsExampleController.OnAnchorResolved(
                            true, result.Response.ToString());
                        _OnResolved(result.Anchor.transform);
                    }));
        }

       
        private void _OnResolved(Transform anchorTransform)
        {
            var cloudAnchorController = GameObject.Find("CloudAnchorsExampleController")
                                                  .GetComponent<CloudAnchorsExampleController>();
            cloudAnchorController.SetWorldOrigin(anchorTransform);
        }

       
        private void _OnChangeId(string newId)
        {
            if (!m_IsHost && newId != string.Empty)
            {
                m_CloudAnchorId = newId;
                m_ShouldResolve = true;
            }
        }
    }
}
