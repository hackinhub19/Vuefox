

namespace GoogleARCore.Examples.ComputerVision
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

   
    public class PointClickHandler : MonoBehaviour, IPointerClickHandler
    {
       
        public event Action OnPointClickDetected;

        
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            if (OnPointClickDetected != null)
            {
                OnPointClickDetected();
            }
        }
    }
}
