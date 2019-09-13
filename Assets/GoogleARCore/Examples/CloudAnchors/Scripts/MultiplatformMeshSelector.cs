

namespace GoogleARCore.Examples.CloudAnchors
{
    using UnityEngine;

   
    public class MultiplatformMeshSelector : MonoBehaviour
    {
        
        public void Start()
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                transform.Find("ARCoreMesh").gameObject.SetActive(true);
                transform.Find("ARKitMesh").gameObject.SetActive(false);
            }
            else
            {
                transform.Find("ARCoreMesh").gameObject.SetActive(false);
                transform.Find("ARKitMesh").gameObject.SetActive(true);
            }
        }
    }
}
