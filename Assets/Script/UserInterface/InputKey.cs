using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserInterface
{
    using UnityEngine.UI;

    public class InputKey : MonoBehaviour
    {
        private char key;
        public Image backingImage;
        public Text text;
        private CustomKeyboard parentKeyboard;
        
        #if UNITY_EDITOR
        public Vector2 GetBackingImageDimensions()
        {
            Rect rect = backingImage.rectTransform.rect;
            
            return new Vector2(rect.width, rect.height);
        }
        #endif
        
        public void ApplyInput()
        {
            parentKeyboard.AddInput(GetKey());
        }
        
        public string GetKey()
        {   
            return key.ToString();
        }
        
        public void SetKey(char key)
        {
            this.key = key;
            text.text = key.ToString();
        }
        
        public void SetDimensions(Vector2 position)
        {
            transform.localPosition = position;
        }
        
        public void SetKeyboard(CustomKeyboard parentKeyboard)
        {
            this.parentKeyboard = parentKeyboard;
        }
    }
}