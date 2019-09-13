

namespace GoogleARCore.Examples.Common
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Video;

   
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(VideoPlayer))]
    public class RawImageVideoPlayer : MonoBehaviour
    {
        
        public RawImage RawImage;

       
        public VideoPlayer VideoPlayer;

        private Texture m_RawImageTexture;

       
        public void Start()
        {
            VideoPlayer.enabled = false;
            m_RawImageTexture = RawImage.texture;
            VideoPlayer.prepareCompleted += _PrepareCompleted;
        }

       
        public void Update()
        {
            if (!Session.Status.IsValid() || Session.Status.IsError())
            {
                VideoPlayer.Stop();
                return;
            }

            if (RawImage.enabled && !VideoPlayer.enabled)
            {
                VideoPlayer.enabled = true;
                VideoPlayer.Play();
            }
            else if (!RawImage.enabled && VideoPlayer.enabled)
            {
               
                VideoPlayer.Stop();
                RawImage.texture = m_RawImageTexture;
                VideoPlayer.enabled = false;
            }
        }

        private void _PrepareCompleted(VideoPlayer player)
        {
            RawImage.texture = player.texture;
        }
    }
}
