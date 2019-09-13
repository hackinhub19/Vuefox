

namespace GoogleARCore.Examples.CloudAnchors
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;

   
    public class ARCoreWorldOriginHelper : MonoBehaviour
    {
        
        public Transform ARCoreDeviceTransform;

        
        public GameObject DetectedPlanePrefab;

        
        private List<DetectedPlane> m_NewPlanes = new List<DetectedPlane>();

        
        private List<GameObject> m_PlanesBeforeOrigin = new List<GameObject>();

       
        private bool m_IsOriginPlaced = false;

        
        private Transform m_AnchorTransform;

        
        public void Update()
        {
            
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }

            Pose worldPose = _WorldToAnchorPose(Pose.identity);

           
            Session.GetTrackables<DetectedPlane>(m_NewPlanes, TrackableQueryFilter.New);
            for (int i = 0; i < m_NewPlanes.Count; i++)
            {
                
                GameObject planeObject = Instantiate(
                    DetectedPlanePrefab, worldPose.position, worldPose.rotation, transform);
                planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(m_NewPlanes[i]);

                if (!m_IsOriginPlaced)
                {
                    m_PlanesBeforeOrigin.Add(planeObject);
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

            m_AnchorTransform = anchorTransform;

            Pose worldPose = _WorldToAnchorPose(new Pose(ARCoreDeviceTransform.position,
                                                         ARCoreDeviceTransform.rotation));
            ARCoreDeviceTransform.SetPositionAndRotation(worldPose.position, worldPose.rotation);

            foreach (GameObject plane in m_PlanesBeforeOrigin)
            {
                if (plane != null)
                {
                    plane.transform.SetPositionAndRotation(worldPose.position, worldPose.rotation);
                }
            }
        }

       
        public bool Raycast(float x, float y, TrackableHitFlags filter, out TrackableHit hitResult)
        {
            bool foundHit = Frame.Raycast(x, y, filter, out hitResult);
            if (foundHit)
            {
                Pose worldPose = _WorldToAnchorPose(hitResult.Pose);
                TrackableHit newHit = new TrackableHit(
                    worldPose, hitResult.Distance, hitResult.Flags, hitResult.Trackable);
                hitResult = newHit;
            }

            return foundHit;
        }

        
        private Pose _WorldToAnchorPose(Pose pose)
        {
            if (!m_IsOriginPlaced)
            {
                return pose;
            }

            Matrix4x4 anchorTWorld = Matrix4x4.TRS(
                m_AnchorTransform.position, m_AnchorTransform.rotation, Vector3.one).inverse;

            Vector3 position = anchorTWorld.MultiplyPoint(pose.position);
            Quaternion rotation = pose.rotation * Quaternion.LookRotation(
                anchorTWorld.GetColumn(2), anchorTWorld.GetColumn(1));

            return new Pose(position, rotation);
        }
    }
}
