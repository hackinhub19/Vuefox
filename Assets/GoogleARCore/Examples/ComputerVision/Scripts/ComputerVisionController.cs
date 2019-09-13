

namespace GoogleARCore.Examples.ComputerVision
{
    using System;
    using System.Collections.Generic;
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.UI;

    #if UNITY_EDITOR
   
    using Input = InstantPreviewInput;
    #endif  

    public class ComputerVisionController : MonoBehaviour
    {
       
        public ARCoreSession ARSessionManager;

        
        public Image EdgeDetectionBackgroundImage;

        >
        public Text CameraIntrinsicsOutput;

        
        public Text SnackbarText;

        
        public Toggle LowResConfigToggle;

        
        
        public Toggle HighResConfigToggle;

       
        public PointClickHandler ImageTextureToggle;

        
        public Toggle AutoFocusToggle;

      
        private byte[] m_EdgeDetectionResultImage = null;

        
        private Texture2D m_EdgeDetectionBackgroundTexture = null;

        
        private DisplayUvCoords m_CameraImageToDisplayUvTransformation;

        private ScreenOrientation? m_CachedOrientation = null;
        private Vector2 m_CachedScreenDimensions = Vector2.zero;
        private bool m_IsQuitting = false;
        private bool m_UseHighResCPUTexture = false;
        private ARCoreSession.OnChooseCameraConfigurationDelegate m_OnChoseCameraConfiguration =
            null;

        private bool m_Resolutioninitialized = false;
        private Text m_ImageTextureToggleText;

       
        public void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            ImageTextureToggle.OnPointClickDetected += _OnBackgroundClicked;

            m_ImageTextureToggleText = ImageTextureToggle.GetComponentInChildren<Text>();
#if UNITY_EDITOR
            AutoFocusToggle.GetComponentInChildren<Text>().text += "\n(Not supported in editor)";
            HighResConfigToggle.GetComponentInChildren<Text>().text +=
                "\n(Not supported in editor)";
            SnackbarText.text =
                "Use mouse/keyboard in the editor Game view to toggle settings.\n" +
                "(Tapping on the device screen will not work while running in the editor)";
#else
            SnackbarText.text = string.Empty;
#endif

           
            m_OnChoseCameraConfiguration = _ChooseCameraConfiguration;
            ARSessionManager.RegisterChooseCameraConfigurationCallback(
                m_OnChoseCameraConfiguration);

            ARSessionManager.enabled = true;
        }

        
        public void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            _QuitOnConnectionErrors();

            
            LowResConfigToggle.gameObject.SetActive(EdgeDetectionBackgroundImage.enabled);
            HighResConfigToggle.gameObject.SetActive(EdgeDetectionBackgroundImage.enabled);
            m_ImageTextureToggleText.text = EdgeDetectionBackgroundImage.enabled ?
                    "Switch to GPU Texture" : "Switch to CPU Image";

            if (!Session.Status.IsValid())
            {
                return;
            }

            using (var image = Frame.CameraImage.AcquireCameraImageBytes())
            {
                if (!image.IsAvailable)
                {
                    return;
                }

                _OnImageAvailable(image.Width, image.Height, image.YRowStride, image.Y, 0);
            }

            var cameraIntrinsics = EdgeDetectionBackgroundImage.enabled
                ? Frame.CameraImage.ImageIntrinsics : Frame.CameraImage.TextureIntrinsics;
            string intrinsicsType =
                EdgeDetectionBackgroundImage.enabled ? "CPU Image" : "GPU Texture";
            CameraIntrinsicsOutput.text =
                _CameraIntrinsicsToString(cameraIntrinsics, intrinsicsType);
        }

       
        public void OnLowResolutionCheckboxValueChanged(bool newValue)
        {
            m_UseHighResCPUTexture = !newValue;
            HighResConfigToggle.isOn = !newValue;

            
            ARSessionManager.enabled = false;
            ARSessionManager.enabled = true;
        }

      
        public void OnHighResolutionCheckboxValueChanged(bool newValue)
        {
            m_UseHighResCPUTexture = newValue;
            LowResConfigToggle.isOn = !newValue;

            
            ARSessionManager.enabled = false;
            ARSessionManager.enabled = true;
        }

        
        public void OnAutoFocusCheckboxValueChanged(bool autoFocusEnabled)
        {
            var config = ARSessionManager.SessionConfig;
            if (config != null)
            {
                config.CameraFocusMode =
                    autoFocusEnabled ? CameraFocusMode.Auto : CameraFocusMode.Fixed;
            }
        }

       
        private void _OnBackgroundClicked()
        {
            EdgeDetectionBackgroundImage.enabled = !EdgeDetectionBackgroundImage.enabled;
        }

       
        private void _OnImageAvailable(
            int width, int height, int rowStride, IntPtr pixelBuffer, int bufferSize)
        {
            if (!EdgeDetectionBackgroundImage.enabled)
            {
                return;
            }

            if (m_EdgeDetectionBackgroundTexture == null ||
                m_EdgeDetectionResultImage == null ||
                m_EdgeDetectionBackgroundTexture.width != width ||
                m_EdgeDetectionBackgroundTexture.height != height)
            {
                m_EdgeDetectionBackgroundTexture =
                    new Texture2D(width, height, TextureFormat.R8, false, false);
                m_EdgeDetectionResultImage = new byte[width * height];
                m_CameraImageToDisplayUvTransformation = Frame.CameraImage.ImageDisplayUvs;
            }

            if (m_CachedOrientation != Screen.orientation ||
                m_CachedScreenDimensions.x != Screen.width ||
                m_CachedScreenDimensions.y != Screen.height)
            {
                m_CameraImageToDisplayUvTransformation = Frame.CameraImage.ImageDisplayUvs;
                m_CachedOrientation = Screen.orientation;
                m_CachedScreenDimensions = new Vector2(Screen.width, Screen.height);
            }

           
            if (EdgeDetector.Detect(
                m_EdgeDetectionResultImage, pixelBuffer, width, height, rowStride))
            {
                
                m_EdgeDetectionBackgroundTexture.LoadRawTextureData(m_EdgeDetectionResultImage);
                m_EdgeDetectionBackgroundTexture.Apply();
                EdgeDetectionBackgroundImage.material.SetTexture(
                    "_ImageTex", m_EdgeDetectionBackgroundTexture);

                const string TOP_LEFT_RIGHT = "_UvTopLeftRight";
                const string BOTTOM_LEFT_RIGHT = "_UvBottomLeftRight";
                EdgeDetectionBackgroundImage.material.SetVector(TOP_LEFT_RIGHT, new Vector4(
                    m_CameraImageToDisplayUvTransformation.TopLeft.x,
                    m_CameraImageToDisplayUvTransformation.TopLeft.y,
                    m_CameraImageToDisplayUvTransformation.TopRight.x,
                    m_CameraImageToDisplayUvTransformation.TopRight.y));
                EdgeDetectionBackgroundImage.material.SetVector(BOTTOM_LEFT_RIGHT, new Vector4(
                    m_CameraImageToDisplayUvTransformation.BottomLeft.x,
                    m_CameraImageToDisplayUvTransformation.BottomLeft.y,
                    m_CameraImageToDisplayUvTransformation.BottomRight.x,
                    m_CameraImageToDisplayUvTransformation.BottomRight.y));
            }
        }

       
        private void _QuitOnConnectionErrors()
        {
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
            else if (Session.Status == SessionStatus.FatalError)
            {
                _ShowAndroidToastMessage(
                    "ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
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

       
        private void _DoQuit()
        {
            Application.Quit();
        }

        
        private string _CameraIntrinsicsToString(CameraIntrinsics intrinsics, string intrinsicsType)
        {
            float fovX = 2.0f * Mathf.Rad2Deg * Mathf.Atan2(
                intrinsics.ImageDimensions.x, 2 * intrinsics.FocalLength.x);
            float fovY = 2.0f * Mathf.Rad2Deg * Mathf.Atan2(
                intrinsics.ImageDimensions.y, 2 * intrinsics.FocalLength.y);

            string message = string.Format(
                "Unrotated Camera {4} Intrinsics:{0}  Focal Length: {1}{0}  " +
                "Principal Point: {2}{0}  Image Dimensions: {3}{0}  " +
                "Unrotated Field of View: ({5}°, {6}°)",
                Environment.NewLine, intrinsics.FocalLength.ToString(),
                intrinsics.PrincipalPoint.ToString(), intrinsics.ImageDimensions.ToString(),
                intrinsicsType, fovX, fovY);
            return message;
        }

       
        private int _ChooseCameraConfiguration(List<CameraConfig> supportedConfigurations)
        {
            if (!m_Resolutioninitialized)
            {
                Vector2 ImageSize = supportedConfigurations[0].ImageSize;
                LowResConfigToggle.GetComponentInChildren<Text>().text = string.Format(
                    "Low Resolution CPU Image ({0} x {1})", ImageSize.x, ImageSize.y);
                ImageSize = supportedConfigurations[supportedConfigurations.Count - 1].ImageSize;
                HighResConfigToggle.GetComponentInChildren<Text>().text = string.Format(
                    "High Resolution CPU Image ({0} x {1})", ImageSize.x, ImageSize.y);

                m_Resolutioninitialized = true;
            }

            if (m_UseHighResCPUTexture)
            {
                return supportedConfigurations.Count - 1;
            }

            return 0;
        }
    }
}
