using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pong
{
    public class DeveloperCheats : MonoBehaviour
    {
        [SerializeField] private PongGameManager pongGameManager;
        public static DeveloperCheats Instance { get; private set; }
        private void Awake() 
        { 
            // If there is an instance, and it's not me, delete myself.
    
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Instance = this; 
            } 
        }
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnValidate()
        {
            if (pongGameManager == null)
            {
                Debug.Log("Set pongManager");
            }
        }
    }
}
