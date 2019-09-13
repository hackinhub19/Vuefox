
namespace GoogleARCore.Examples.ComputerVision
{
    using System;
    using UnityEngine;

#if UNITY_IOS && !UNITY_EDITOR
    using AndroidImport = GoogleARCoreInternal.DllImportNoop;
    using IOSImport = System.Runtime.InteropServices.DllImportAttribute;
#else
    using AndroidImport = System.Runtime.InteropServices.DllImportAttribute;
    using IOSImport = GoogleARCoreInternal.DllImportNoop;
#endif

    public class TextureReaderApi
    {
       
        public enum ImageFormatType
        {
           
            ImageFormatColor = 0,

           
            ImageFormatGrayscale = 1
        }

        
        public void Create(ImageFormatType format, int width, int height, bool keepAspectRatio)
        {
            ExternApi.TextureReader_create((int)format, width, height, keepAspectRatio);
        }

        
        public void Destroy()
        {
            ExternApi.TextureReader_destroy();
        }

       
        public int SubmitFrame(int textureId, int textureWidth, int textureHeight)
        {
            int bufferIndex =
                ExternApi.TextureReader_submitFrame(textureId, textureWidth, textureHeight);
            GL.InvalidateState();
            return bufferIndex;
        }

       
        public IntPtr AcquireFrame(int bufferIndex, ref int bufferSize)
        {
            IntPtr pixelBuffer = ExternApi.TextureReader_acquireFrame(bufferIndex, ref bufferSize);
            return pixelBuffer;
        }

        public void ReleaseFrame(int bufferIndex)
        {
            ExternApi.TextureReader_releaseFrame(bufferIndex);
        }

        private struct ExternApi
        {
#pragma warning disable 626
            public const string ARCoreCameraUtilityAPI = "arcore_camera_utility";

            [AndroidImport(ARCoreCameraUtilityAPI)]
            public static extern void TextureReader_create(
                int format, int width, int height, bool keepAspectRatio);

            [AndroidImport(ARCoreCameraUtilityAPI)]
            public static extern void TextureReader_destroy();

            [AndroidImport(ARCoreCameraUtilityAPI)]
            public static extern int TextureReader_submitFrame(
                int textureId, int textureWidth, int textureHeight);

            [AndroidImport(ARCoreCameraUtilityAPI)]
            public static extern IntPtr TextureReader_acquireFrame(
                int bufferIndex, ref int bufferSize);

            [AndroidImport(ARCoreCameraUtilityAPI)]
            public static extern void TextureReader_releaseFrame(int bufferIndex);
#pragma warning restore 626
        }
    }
}
