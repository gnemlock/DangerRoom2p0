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
        
        public Vector2 GetBackingImageDimensions()
        {
            Rect rect = backingImage.rectTransform.rect;
            
            return new Vector2(rect.width, rect.height);
        }
        
        public char GetKey()
        {   
            return key;
        }
        
        public void SetKey(char key)
        {
            this.key = key;
            text.text = key.ToString();
        }
        
        public void SetDimensions(Vector2 position, Vector2 dimensions)
        {
            transform.localPosition = position;
        }
    }
}