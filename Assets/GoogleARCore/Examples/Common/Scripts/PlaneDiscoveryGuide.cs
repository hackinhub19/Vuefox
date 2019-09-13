

namespace GoogleARCore.Examples.Common
{
    using System.Collections.Generic;
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.UI;

    public class PlaneDiscoveryGuide : MonoBehaviour
    {
        
        [Tooltip("The time to delay, after ARCore loses tracking of any planes, showing the plane " +
                 "discovery guide.")]
        public float DisplayGuideDelay = 3.0f;

        [Tooltip("The time to delay, after displaying the plane discovery guide, offering more detailed " +
                 "instructions on how to find a plane.")]
        public float OfferDetailedInstructionsDelay = 8.0f;

        
        private const float k_OnStartDelay = 1f;

        
        private const float k_HideGuideDelay = 0.75f;

        
        private const float k_AnimationFadeDuration = 0.15f;

       
        [Tooltip("The Game Object that provides feature points visualization.")]
        [SerializeField] private GameObject m_FeaturePoints = null;

        
        [Tooltip("The RawImage that provides rotating hand animation.")]
        [SerializeField] private RawImage m_HandAnimation = null;

       
        [Tooltip("The snackbar Game Object.")]
        [SerializeField] private GameObject m_SnackBar = null;

       
        [Tooltip("The snackbar text.")]
        [SerializeField] private Text m_SnackBarText = null;

        
        [Tooltip("The Game Object that contains the button to open the help window.")]
        [SerializeField] private GameObject m_OpenButton = null;

        
        [Tooltip(
            "The Game Object that contains the window with more instructions on how to find " +
            "a plane.")]
        [SerializeField] private GameObject m_MoreHelpWindow = null;

        
        [Tooltip("The Game Object that contains the button to close the help window.")]
        [SerializeField] private Button m_GotItButton = null;

      
        private float m_DetectedPlaneElapsed;

        
        private float m_NotDetectedPlaneElapsed;

        
        private bool m_IsLostTrackingDisplayed;

        
        private List<DetectedPlane> m_DetectedPlanes = new List<DetectedPlane>();

       
        public void Start()
        {
            m_OpenButton.GetComponent<Button>().onClick.AddListener(_OnOpenButtonClicked);
            m_GotItButton.onClick.AddListener(_OnGotItButtonClicked);

            _CheckFieldsAreNotNull();
            m_MoreHelpWindow.SetActive(false);
            m_IsLostTrackingDisplayed = false;
            m_NotDetectedPlaneElapsed = DisplayGuideDelay - k_OnStartDelay;
        }

       
        public void OnDestroy()
        {
            m_OpenButton.GetComponent<Button>().onClick.RemoveListener(_OnOpenButtonClicked);
            m_GotItButton.onClick.RemoveListener(_OnGotItButtonClicked);
        }

        
        public void Update()
        {
            _UpdateDetectedPlaneTrackingState();
            _UpdateUI();
        }

        
        private void _OnOpenButtonClicked()
        {
            m_MoreHelpWindow.SetActive(true);

            enabled = false;
            m_FeaturePoints.SetActive(false);
            m_HandAnimation.enabled = false;
            m_SnackBar.SetActive(false);
        }

        
        private void _OnGotItButtonClicked()
        {
            m_MoreHelpWindow.SetActive(false);
            enabled = true;
        }

        
        private void _UpdateDetectedPlaneTrackingState()
        {
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }

            Session.GetTrackables<DetectedPlane>(m_DetectedPlanes, TrackableQueryFilter.All);
            foreach (DetectedPlane plane in m_DetectedPlanes)
            {
                if (plane.TrackingState == TrackingState.Tracking)
                {
                    m_DetectedPlaneElapsed += Time.deltaTime;
                    m_NotDetectedPlaneElapsed = 0f;
                    return;
                }
            }

            m_DetectedPlaneElapsed = 0f;
            m_NotDetectedPlaneElapsed += Time.deltaTime;
        }

       
        private void _UpdateUI()
        {
            if (Session.Status == SessionStatus.LostTracking &&
                Session.LostTrackingReason != LostTrackingReason.None)
            {
                
                m_FeaturePoints.SetActive(false);
                m_HandAnimation.enabled = false;
                m_SnackBar.SetActive(true);
                switch (Session.LostTrackingReason)
                {
                    case LostTrackingReason.InsufficientLight:
                        m_SnackBarText.text = "Too dark. Try moving to a well-lit area.";
                        break;
                    case LostTrackingReason.InsufficientFeatures:
                        m_SnackBarText.text = "Aim device at a surface with more texture or color.";
                        break;
                    case LostTrackingReason.ExcessiveMotion:
                        m_SnackBarText.text = "Moving too fast. Slow down.";
                        break;
                    default:
                        m_SnackBarText.text = "Motion tracking is lost.";
                        break;
                }

                m_OpenButton.SetActive(false);
                m_IsLostTrackingDisplayed = true;
                return;
            }
            else if (m_IsLostTrackingDisplayed)
            {
               
                m_SnackBar.SetActive(false);
                m_IsLostTrackingDisplayed = false;
            }

            if (m_NotDetectedPlaneElapsed > DisplayGuideDelay)
            {
                
                m_FeaturePoints.SetActive(true);

                if (!m_HandAnimation.enabled)
                {
                    m_HandAnimation.GetComponent<CanvasRenderer>().SetAlpha(0f);
                    m_HandAnimation.CrossFadeAlpha(1f, k_AnimationFadeDuration, false);
                }

                m_HandAnimation.enabled = true;
                m_SnackBar.SetActive(true);

                if (m_NotDetectedPlaneElapsed > OfferDetailedInstructionsDelay)
                {
                    m_SnackBarText.text = "Need Help?";
                    m_OpenButton.SetActive(true);
                }
                else
                {
                    m_SnackBarText.text = "Point your camera to where you want to place an object.";
                    m_OpenButton.SetActive(false);
                }
            }
            else if (m_NotDetectedPlaneElapsed > 0f || m_DetectedPlaneElapsed > k_HideGuideDelay)
            {
                
                m_FeaturePoints.SetActive(false);
                m_SnackBar.SetActive(false);
                m_OpenButton.SetActive(false);

                if (m_HandAnimation.enabled)
                {
                    m_HandAnimation.GetComponent<CanvasRenderer>().SetAlpha(1f);
                    m_HandAnimation.CrossFadeAlpha(0f, k_AnimationFadeDuration, false);
                }

                m_HandAnimation.enabled = false;
            }
        }

       
        private void _CheckFieldsAreNotNull()
        {
            if (m_MoreHelpWindow == null)
            {
                Debug.LogError("MoreHelpWindow is null");
            }

            if (m_GotItButton == null)
            {
                Debug.LogError("GotItButton is null");
            }

            if (m_SnackBarText == null)
            {
                Debug.LogError("SnackBarText is null");
            }

            if (m_SnackBar == null)
            {
                Debug.LogError("SnackBar is null");
            }

            if (m_OpenButton == null)
            {
                Debug.LogError("OpenButton is null");
            }
            else if (m_OpenButton.GetComponent<Button>() == null)
            {
                Debug.LogError("OpenButton does not have a Button Component.");
            }

            if (m_HandAnimation == null)
            {
                Debug.LogError("HandAnimation is null");
            }

            if (m_FeaturePoints == null)
            {
                Debug.LogError("FeaturePoints is null");
            }
        }
    }
}
