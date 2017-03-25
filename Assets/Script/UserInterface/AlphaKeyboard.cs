using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UserInterface
{
    public class AlphaKeyboard : CustomKeyboard
    {
        public InputKey keyPrefab;
        public float buffer = 0.1f;
        public Vector2 rowStart;

        private string keySet = "qwertyuiopasdfghjklzxcvbnm";
        private InputKey[] inputKeys = new InputKey[26];
        private bool isLowercase = false;
        private bool created = false;
        private bool enabled = false;
        private Rect parentTransform;

        private Vector2 startPosition;
        private Vector2 keyDimension;
        
        private float screenWidth;
        private float screenHeight;
        private ScreenOrientation currentOrientation;
        
        public void Start()
        {
            currentOrientation = Screen.orientation;
            CreateKeyboard();
        }
        
        public void CreateKeyboard()
        {
            Vector2 dimension = keyPrefab.GetBackingImageDimensions();
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float localBuffer = 0f;
            Vector2 inputPositionBuffer = rowStart;

            for(int i = 0; i < 10; i++)
            {
                CreateKey(i, inputPositionBuffer);
                inputPositionBuffer.x += buffer + dimension.x;
            }
            
            rowStart.x += dimension.x / 2.0f;
            rowStart.y -= dimension.y + buffer;
            inputPositionBuffer = rowStart;
            localBuffer = 0f;
            
            for(int i = 10; i < 19; i++)
            {
                CreateKey(i, inputPositionBuffer);
                inputPositionBuffer.x += buffer + dimension.x;
            }
            
            rowStart.x += dimension.x;
            rowStart.y -= dimension.y + buffer;
            inputPositionBuffer = rowStart;
            localBuffer = 0f;
            
            for(int i = 19; i < 26; i++)
            {
                CreateKey(i, inputPositionBuffer);
                inputPositionBuffer.x += buffer + dimension.x;
            }
        }
        
        public void SetParentTransform()
        {
            RectTransform parentRectTransform = (RectTransform)transform.parent;
            parentTransform = parentRectTransform.rect;
            parentTransform.position *= -1;
        }
        
        #region Key Creation        
        private void CreateKey(int index, Vector2 position)
        {
            inputKeys[index] = (InputKey)Instantiate(keyPrefab);
            inputKeys[index].SetKey(keySet[index]);
            inputKeys[index].SetDimensions(position, keyDimension);
            inputKeys[index].transform.SetParent(transform);
        }
        
        public void RefreshKeys()
        {
            for(int i = 0; i < 26; i++)
            {
                inputKeys[i].SetKey(keySet[i]);
            }
        }
        #endregion
        
        #region Capitalisation
        public void ToUpper()
        {
            keySet = keySet.ToUpper();
            FinaliseCapitalisation(false);
        }

        public void ToLower()
        {
            keySet = keySet.ToLower();
            FinaliseCapitalisation(true);
        }
        
        private void FinaliseCapitalisation(bool isLowercase)
        {
            this.isLowercase = isLowercase;
            RefreshKeys();
        }

        public bool ChangeCapitalisation()
        {
            if(isLowercase)
            {
                ToUpper();
                return true;
            }
            else
            {
                ToLower();
                return false;
            }
        }
        #endregion
        
        #region Member Access
        public bool IsEnabled()
        {
            return enabled;
        }
        
        public bool IsCreated()
        {
            return created;
        }
        #endregion
    }
    
    [CustomEditor(typeof(AlphaKeyboard))]
    public class AlphaKeyboardEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AlphaKeyboard alphaKeyboard = target as AlphaKeyboard;
            
            if(GUILayout.Button("GetParent"))
            {
                alphaKeyboard.SetParentTransform();
            }

            if(GUILayout.Button("CreateKeyboard"))
            {
                alphaKeyboard.CreateKeyboard();
            }
        }
    }   
}

namespace UserInterface.Utility
{
    
}