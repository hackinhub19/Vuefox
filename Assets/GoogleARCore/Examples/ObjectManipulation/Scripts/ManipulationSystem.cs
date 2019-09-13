

namespace GoogleARCore.Examples.ObjectManipulation
{
    using UnityEngine;

   
    public class ManipulationSystem : MonoBehaviour
    {
        private static ManipulationSystem s_Instance = null;

        private DragGestureRecognizer m_DragGestureRecognizer = new DragGestureRecognizer();

        private PinchGestureRecognizer m_PinchGestureRecognizer = new PinchGestureRecognizer();

        private TwoFingerDragGestureRecognizer m_TwoFingerDragGestureRecognizer =
            new TwoFingerDragGestureRecognizer();

        private TapGestureRecognizer m_TapGestureRecognizer = new TapGestureRecognizer();

        private TwistGestureRecognizer m_TwistGestureRecognizer = new TwistGestureRecognizer();

        
        public static ManipulationSystem Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    var manipulationSystems = FindObjectsOfType<ManipulationSystem>();
                    if (manipulationSystems.Length > 0)
                    {
                        s_Instance = manipulationSystems[0];
                    }
                    else
                    {
                        Debug.LogError("No instance of ManipulationSystem exists in the scene.");
                    }
                }

                return s_Instance;
            }
        }

       
        public DragGestureRecognizer DragGestureRecognizer
        {
            get
            {
                return m_DragGestureRecognizer;
            }
        }

        
        public PinchGestureRecognizer PinchGestureRecognizer
        {
            get
            {
                return m_PinchGestureRecognizer;
            }
        }

        
        public TwoFingerDragGestureRecognizer TwoFingerDragGestureRecognizer
        {
            get
            {
                return m_TwoFingerDragGestureRecognizer;
            }
        }

       >
        public TapGestureRecognizer TapGestureRecognizer
        {
            get
            {
                return m_TapGestureRecognizer;
            }
        }

       
        public TwistGestureRecognizer TwistGestureRecognizer
        {
            get
            {
                return m_TwistGestureRecognizer;
            }
        }

        
        public GameObject SelectedObject { get; private set;  }

        
        public void Awake()
        {
            if (Instance != this)
            {
                Debug.LogWarning("Multiple instances of ManipulationSystem detected in the scene." +
                                 " Only one instance can exist at a time. The duplicate instances" +
                                 " will be destroyed.");
                DestroyImmediate(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

       
        public void Update()
        {
            DragGestureRecognizer.Update();
            PinchGestureRecognizer.Update();
            TwoFingerDragGestureRecognizer.Update();
            TapGestureRecognizer.Update();
            TwistGestureRecognizer.Update();
        }

        
        internal void Deselect()
        {
            SelectedObject = null;
        }

        
        internal void Select(GameObject target)
        {
            if (SelectedObject == target)
            {
                return;
            }

            Deselect();
            SelectedObject = target;
        }
    }
}
