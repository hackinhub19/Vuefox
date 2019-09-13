

namespace GoogleARCore.Examples.AugmentedFaces
{
    using System.Collections.Generic;
    using GoogleARCore;
    using UnityEngine;

   
    [RequireComponent(typeof(MeshFilter))]
    public class ARCoreAugmentedFaceMeshFilter : MonoBehaviour
    {
        
        public bool AutoBind = false;

        private AugmentedFace m_AugmentedFace = null;
        private List<AugmentedFace> m_AugmentedFaceList = null;

       
        private List<Vector3> m_MeshVertices = new List<Vector3>();
        private List<Vector3> m_MeshNormals = new List<Vector3>();
        private List<Vector2> m_MeshUVs = new List<Vector2>();
        private List<int> m_MeshIndices = new List<int>();
        private Mesh m_Mesh = null;
        private bool m_MeshInitialized = false;

       
        public AugmentedFace AumgnetedFace
        {
            get
            {
                return m_AugmentedFace;
            }

            set
            {
                m_AugmentedFace = value;
                Update();
            }
        }

        
        public void Awake()
        {
            m_Mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = m_Mesh;
            m_AugmentedFaceList = new List<AugmentedFace>();
        }

        
        public void Update()
        {
            if (AutoBind && m_AugmentedFace == null)
            {
                m_AugmentedFaceList.Clear();
                Session.GetTrackables<AugmentedFace>(m_AugmentedFaceList, TrackableQueryFilter.All);
                if (m_AugmentedFaceList.Count != 0)
                {
                    m_AugmentedFace = m_AugmentedFaceList[0];
                }
            }

            if (m_AugmentedFace == null)
            {
                return;
            }

            
            transform.position = m_AugmentedFace.CenterPose.position;
            transform.rotation = m_AugmentedFace.CenterPose.rotation;

            _UpdateMesh();
        }

       
        private void _UpdateMesh()
        {
            m_AugmentedFace.GetVertices(m_MeshVertices);
            m_AugmentedFace.GetNormals(m_MeshNormals);

            if (!m_MeshInitialized)
            {
                m_AugmentedFace.GetTextureCoordinates(m_MeshUVs);
                m_AugmentedFace.GetTriangleIndices(m_MeshIndices);

                
                m_MeshInitialized = true;
            }

            m_Mesh.Clear();
            m_Mesh.SetVertices(m_MeshVertices);
            m_Mesh.SetNormals(m_MeshNormals);
            m_Mesh.SetTriangles(m_MeshIndices, 0);
            m_Mesh.SetUVs(0, m_MeshUVs);

            m_Mesh.RecalculateBounds();
        }
    }
}
