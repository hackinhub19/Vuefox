

namespace GoogleARCore.Examples.Common
{
    using System.Collections;
    using UnityEngine;

    
    public class SafeAreaScaler : MonoBehaviour
    {
        private Rect m_ScreenSafeArea = new Rect(0, 0, 0, 0);

        public void Update()
        {
            Rect safeArea;
#if UNITY_2017_2_OR_NEWER
            safeArea = Screen.safeArea;
#else
            safeArea = new Rect(0, 0, Screen.width, Screen.height);
#endif

            if (m_ScreenSafeArea != safeArea)
            {
                m_ScreenSafeArea = safeArea;
                _MatchRectTransformToSafeArea();
            }
        }

        private void _MatchRectTransformToSafeArea()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            
            Vector2 offsetMin = new Vector2(m_ScreenSafeArea.xMin,
                Screen.height - m_ScreenSafeArea.yMax);

            
            Vector2 offsetMax = new Vector2(m_ScreenSafeArea.xMax - Screen.width,
                -m_ScreenSafeArea.yMin);

            rectTransform.offsetMin = offsetMin;
            rectTransform.offsetMax = offsetMax;
        }
    }
}
