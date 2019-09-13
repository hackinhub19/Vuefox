

namespace GoogleARCore.Examples.CloudAnchors
{
    using UnityEngine;
    using UnityEngine.Networking;

    
#pragma warning disable 618
    public class LocalPlayerController : NetworkBehaviour
#pragma warning restore 618
    {
        public GameObject StarPrefab;

       
        public GameObject AnchorPrefab;

        
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

           
            gameObject.name = "LocalPlayer";
        }

       
        public void SpawnAnchor(Vector3 position, Quaternion rotation, Component anchor)
        {
           
            var anchorObject = Instantiate(AnchorPrefab, position, rotation);

           
            anchorObject.GetComponent<AnchorController>().HostLastPlacedAnchor(anchor);

            
#pragma warning disable 618
            NetworkServer.Spawn(anchorObject);
#pragma warning restore 618
        }

        
#pragma warning disable 618
        [Command]
#pragma warning restore 618
        public void CmdSpawnStar(Vector3 position, Quaternion rotation)
        {
            
            var starObject = Instantiate(StarPrefab, position, rotation);

           
#pragma warning disable 618
            NetworkServer.Spawn(starObject);
#pragma warning restore 618
        }
    }
}
