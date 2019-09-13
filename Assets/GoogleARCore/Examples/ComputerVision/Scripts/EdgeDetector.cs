
namespace GoogleARCore.Examples.ComputerVision
{
    using System;
    using UnityEngine;

   
    public class EdgeDetector
    {
        private static byte[] s_ImageBuffer = new byte[0];
        private static int s_ImageBufferSize = 0;

       
        public static bool Detect(
            byte[] outputImage, IntPtr pixelBuffer, int width, int height, int rowStride)
        {
            if (outputImage.Length < width * height)
            {
                Debug.Log("Input buffer is too small!");
                return false;
            }

            Sobel(outputImage, pixelBuffer, width, height, rowStride);

            return true;
        }

        private static void Sobel(
            byte[] outputImage, IntPtr inputImage, int width, int height, int rowStride)
        {
            
            int bufferSize = rowStride * height;
            if (bufferSize != s_ImageBufferSize || s_ImageBuffer.Length == 0)
            {
                s_ImageBufferSize = bufferSize;
                s_ImageBuffer = new byte[bufferSize];
            }

            
            System.Runtime.InteropServices.Marshal.Copy(inputImage, s_ImageBuffer, 0, bufferSize);

            
            int threshold = 128 * 128;

            for (int j = 1; j < height - 1; j++)
            {
                for (int i = 1; i < width - 1; i++)
                {
                    
                    int offset = (j * rowStride) + i;

                    
                    int a00 = s_ImageBuffer[offset - rowStride - 1];
                    int a01 = s_ImageBuffer[offset - rowStride];
                    int a02 = s_ImageBuffer[offset - rowStride + 1];
                    int a10 = s_ImageBuffer[offset - 1];
                    int a12 = s_ImageBuffer[offset + 1];
                    int a20 = s_ImageBuffer[offset + rowStride - 1];
                    int a21 = s_ImageBuffer[offset + rowStride];
                    int a22 = s_ImageBuffer[offset + rowStride + 1];

                   
                    int xSum = -a00 - (2 * a10) - a20 + a02 + (2 * a12) + a22;

                    
                    int ySum = a00 + (2 * a01) + a02 - a20 - (2 * a21) - a22;

                    if ((xSum * xSum) + (ySum * ySum) > threshold)
                    {
                        outputImage[(j * width) + i] = 0xFF;
                    }
                    else
                    {
                        outputImage[(j * width) + i] = 0x1F;
                    }
                }
            }
        }
    }
}
