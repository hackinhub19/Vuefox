

namespace GoogleARCore.Examples.CloudAnchors
{
    using System.Collections.Generic;
    using UnityEngine;

#if ARCORE_IOS_SUPPORT
    using UnityEngine.XR.iOS;
    using UnityARUserAnchorComponent = UnityEngine.XR.iOS.UnityARUserAnchorComponent;
#else
    using UnityARUserAnchorComponent = UnityEngine.Component;
#endif

   
    public class ARKitHelper
    {
#if ARCORE_IOS_SUPPORT
        private List<ARHitTestResult> m_HitResultList = new List<ARHitTestResult>();
#endif
       
        public bool RaycastPlane(Camera camera, float x, float y, out Pose hitPose)
        {
            hitPose = new Pose();
#if ARCORE_IOS_SUPPORT
            var session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

            var viewportPoint = camera.ScreenToViewportPoint(new Vector2(x, y));
            ARPoint arPoint = new ARPoint
            {
                x = viewportPoint.x,
                y = viewportPoint.y
            };

            m_HitResultList = session.HitTest(arPoint, ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent);
            if (m_HitResultList.Count > 0)
            {
                int minDistanceIndex = 0;
                for (int i = 1; i < m_HitResultList.Count; i++)
                {
                    if (m_HitResultList[i].distance < m_HitResultList[minDistanceIndex].distance)
                    {
                        minDistanceIndex = i;
                    }
                }

                hitPose.position = UnityARMatrixOps.GetPosition(m_HitResultList[minDistanceIndex].worldTransform);

                // Original ARKit hit pose is the plane rotation.
                Quaternion planeRotation = UnityARMatrixOps.GetRotation(
                    m_HitResultList[minDistanceIndex].worldTransform);

                // Try to match the hit rotation to the one ARCore uses.
                Vector3 planeNormal = planeRotation * Vector3.up;
                Vector3 rayDir = camera.ViewportPointToRay(viewportPoint).direction;
                Vector3 planeProjection = Vector3.ProjectOnPlane(rayDir, planeNormal);
                Vector3 forwardDir = -planeProjection.normalized;

                Quaternion hitRotation = Quaternion.LookRotation(forwardDir, planeNormal);
                hitPose.rotation = hitRotation;

                return true;
            }
#endif
            return false;
        }

        
        public UnityARUserAnchorComponent CreateAnchor(Pose pose)
        {
            var anchorGO = new GameObject("User Anchor");
            var anchor = anchorGO.AddComponent<UnityARUserAnchorComponent>();
            anchorGO.transform.position = pose.position;
            anchorGO.transform.rotation = pose.rotation;
            return anchor;
        }

        
        public void SetWorldOrigin(Transform transform)
        {
#if ARCORE_IOS_SUPPORT
            UnityARSessionNativeInterface.GetARSessionNativeInterface().SetWorldOrigin(transform);
#endif
        }
    }
}
