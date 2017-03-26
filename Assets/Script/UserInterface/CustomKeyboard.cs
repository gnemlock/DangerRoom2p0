using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserInterface
{
    public abstract class CustomKeyboard : MonoBehaviour
    {
        /// <summary>The key prefab used to instantiate keys.</summary>
        public InputKey keyPrefab;
        /// <summary>The default distance between each key, along both x and y dimensions.</summary>
        public float keyBorderLength = 0.1f;
        
        /// <summary>The current input registered to this <see cref="UserInterface.CustomKeyboard"/>.</summary>
        protected string currentInput;
        /// <summary>The list of characters used for simple character-based keys</summary>
        protected string characterSet;
        /// <summary>The list of keys presently in the keyboard.</summary>
        protected InputKey[] inputKeys;
        
        /// <summary>Has the keyboard been created?</summary>
        public bool isCreated { get; protected set; }
        /// <summary>Is the keyboard enabled?</summary>
        public bool isEnabled { get; protected set; }
        
        #if UNITY_EDITOR
        protected float xOffset { get { return keyPrefab.GetBackingImageDimensions().x + keyBorderLength; } }
        protected float yOffset { get { return keyPrefab.GetBackingImageDimensions().y + keyBorderLength; } }
        protected float xHalfOffset { get { return keyPrefab.GetBackingImageDimensions().x / 2.0f; } }
        protected float xOffsetNoBorder { get { return keyPrefab.GetBackingImageDimensions().x; } }
        protected float yOffsetNoBorder { get { return keyPrefab.GetBackingImageDimensions().y; } }
        #endif
        
        protected virtual void Start()
        {
            isCreated = false;
            isEnabled = false;
        }
        
        public virtual void AddInput(string input)
        {
            currentInput += input;
            Debug.Log(currentInput);
        }
        
        #if UNITY_EDITOR
        public abstract void CreateKeyboard();
        //TODO:Implement "PrepareChange" for CreateKeyboard and DestroyKeyboard
        //TODO:Implement return, backspace, capslock and shift keys
        public abstract void DestroyKeyboard();
        
        /// <summary>Determines the position for the first key on the first row of the keyboard.</summary>
        /// <returns>The first key on the first row of the keyboard.</returns>
        /// <param name="topRowCount">The number of keys along the top row of the keyboard</param>
        /// <param name="rowCount">The number of  rows in the keyboard</param>
        /// <param name="offset">The y offset of the first key on the first row of the keyboard.</param>
        protected Vector2 GetStartPosition(int topRowCount = 10, int rowCount = 3, float offset = 0f)
        {
            RectTransform rectTransform = transform .parent as RectTransform;
            Vector2 canvasDimension = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
            Vector2 canvasCenter = rectTransform.rect.center;
            //TODO: Align to center of canvas properly
            //TODO: Figure out why border distance only applys some of the time
            //TODO: Figure out why text has gone blurry
            //TODO: Figure out input
            //TODO: Split InputKey up into two classes, to represent keys that are selected by external input, and keys that allow direct input
            Debug.Log(canvasDimension + " " + canvasCenter);
            
            // y offset from bottom of canvas
            float bottomOffset = rowCount * yOffset;
            // x offset from middle of canvas
            float middleOffset = xOffsetNoBorder + (xOffset * (rowCount - 1));
            
            return new Vector2(canvasCenter.x - (middleOffset / 2.0f) + offset, 
                canvasCenter.y - canvasDimension.y + bottomOffset);
        }
        
        /// <summary>Returns a reset start position with offset to the next row.</summary>
        /// <returns>The next row start position.</returns>
        /// <param name="startPosition">Initial start position.</param>
        /// <param name="xMultiplier">Multiplies the x offset of the new start position by xOffset.</param>
        /// <param name="yMultiplier">Multiplies the y offset of the new start position by yOffset.</param>
        protected Vector2 ResetStartPosition(Vector2 startPosition, float xMultiplier = 1.0f, float yMultiplier = 1.0f)
        {
            startPosition.x += xOffset * xMultiplier;
            startPosition.y -= yOffset * yMultiplier;
            
            return startPosition;
        }
        #endif
        
        /// <summary>Creates a new key.</summary>
        /// <returns>The x buffer required to move position to the next position in the row.</returns>
        /// <param name="index">The key index, corresponding to the appropriate character in the characterSet string.</param>
        /// <param name="position">The local position of the new key.</param>
        protected abstract float CreateKey(int index, Vector2 position);
    }
    
    public interface IAlphabetical
    {
        bool isLowercase { get; }
        
        void ToUpper();
        
        void ToLower();
        
        bool ChangeCapitalisation();
    }
}