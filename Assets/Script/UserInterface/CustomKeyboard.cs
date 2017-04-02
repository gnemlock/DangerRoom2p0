using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserInterface
{
    public abstract class CustomKeyboard : MonoBehaviour
    {
        #if UNITY_EDITOR
        /// <summary>The key prefab used to instantiate keys.</summary>
        public InputKey keyPrefab;
        /// <summary>The default distance between each key, along both x and y dimensions.</summary>
        public float horizontalBorderLength = 0.5f;
        public float verticalBorderLength = 1.0f;
        #endif
        
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
        protected float xOffset { get { return keyDimensions.x + verticalBorderLength; } }
        protected float yOffset { get { return keyDimensions.y + horizontalBorderLength; } }
        protected float xHalfOffset { get { return keyDimensions.x / 2.0f; } }
        protected float yHalfOffset { get { return keyDimensions.y / 2.0f; } }
        protected Vector2 halfOffset { get { return new Vector2(xHalfOffset, yHalfOffset); } }
        protected Vector2 keyDimensions { get { return keyPrefab.GetBackingImageDimensions(); } }
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
        
        //TODO: Figure out input
        //TODO: Split InputKey up into two classes, to represent keys that are selected by external input, and keys that allow direct input
        
        
        /// <summary>Determines the position for the first key on the first row of the keyboard.</summary>
        /// <returns>The first key on the first row of the keyboard.</returns>
        /// <param name="topRowCount">The number of keys along the top row of the keyboard</param>
        /// <param name="rowCount">The number of  rows in the keyboard</param>
        /// <param name="offset">The y offset of the first key on the first row of the keyboard.</param>
        protected Vector2 GetStartPosition(int topRowCount = 10, int rowCount = 3, float offset = 0f)
        {
            // Use the inital transform position as our initial startPosition.
            Vector2 startPosition = transform.localPosition;
            
            // Move the starting position down to the bottom of the canvas. We must ensure that 
            // we use the base canvas for this measurement.
            startPosition.y -= GetBaseRectTransform(transform).rect.height / 2.0f;
            
            // We now need to adjust for the size of the keyboard. The keyboard width is the total 
            // sum of keys along the top row, multiplied by the key width, including the border. As 
            // we only have borders between keys, we deduct one border length to balance out the 
            // initial inclusion of border in all key widths. The keyboard height is the total sum 
            // of key rows, multiplied by the key height, including the border. We include the 
            // additional border as buffer from the bottom.
            float keyboardWidth = (xOffset * topRowCount) - horizontalBorderLength;
            float keyboardHeight = yOffset * rowCount;
            Debug.Log(keyboardHeight);
            
            // We need to move the startPosition to the left by half the width of the keyboard, 
            // to centre it, and move it up by the height, to fit it on to the screen.
            startPosition.x -= keyboardWidth / 2.0f;
            startPosition.y += keyboardHeight;
            
            // Finally, we need to adjust 
            startPosition += halfOffset;
            
            return startPosition;
        }

        protected static RectTransform GetBaseRectTransform(Transform currentTransform)
        {
            if(currentTransform.parent == null)
            {
                return currentTransform as RectTransform;
            }
            else
            {
                return GetBaseRectTransform(currentTransform.parent);
            }
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