using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace UserInterface
{
    public class AlphaKeyboard : CustomKeyboard, IAlphabetical
    {
        public bool isLowercase { get; private set; }

        private bool isEnabled;
        
        private float screenWidth;
        private float screenHeight;
        private ScreenOrientation currentOrientation;
        
        protected override void Start()
        {
            base.Start();
            
            isLowercase = false;
        }
        
        public override string ToString()
        {
            return string.Format("[AlphaKeyboard: {0}]", characterSet);
        }
        
        #if UNITY_EDITOR
        public override void CreateKeyboard()
        {
            characterSet = "qwertyuiopasdfghjklzxcvbnm";
            inputKeys = new InputKey[26];
            Vector2 startPosition = GetStartPosition();
            Vector2 currentPosition = startPosition;

            for(int i = 0; i < 10; i++)
            {
                currentPosition.x += CreateKey(i, currentPosition);
            }
            
            currentPosition = startPosition = ResetStartPosition(startPosition, 0.5f);
            
            for(int i = 10; i < 19; i++)
            {
                currentPosition.x += CreateKey(i, currentPosition);
            }
            
            currentPosition = startPosition = ResetStartPosition(startPosition);
            
            for(int i = 19; i < 26; i++)
            {
                currentPosition.x += CreateKey(i, currentPosition);
            }
        }
        
        public override void DestroyKeyboard()
        {
            for(int i = 0; i < inputKeys.Length; i++)
            {
                DestroyImmediate(inputKeys[i].gameObject);
            }
        }
        #endif
        
        #region Key Creation
        #if UNITY_EDITOR
        /// <summary>Creates a new key.</summary>
        /// <returns>The x buffer required to move position to the next position in the row.</returns>
        /// <param name="index">The key index, corresponding to the appropriate character in the characterSet string.</param>
        /// <param name="position">The local position of the new key.</param>
        protected override float CreateKey(int index, Vector2 position)
        {
            InputKey newInputKey = (InputKey)Instantiate(keyPrefab);
            
            newInputKey.SetKey(characterSet[index]);
            newInputKey.SetDimensions(position);
            newInputKey.transform.SetParent(transform);
            newInputKey.name = "InputKey (" + characterSet[index].ToString() + ")";
            newInputKey.SetKeyboard(this);
            
            inputKeys[index] = newInputKey;
            
            return xOffset;
        }
        #endif
        
        public void RefreshKeys()
        {
            for(int i = 0; i < 26; i++)
            {
                inputKeys[i].SetKey(characterSet[i]);
            }
        }
        #endregion
        
        #region Capitalisation
        public void ToUpper()
        {
            characterSet = characterSet.ToUpper();
            FinaliseCapitalisation(false);
        }

        public void ToLower()
        {
            characterSet = characterSet.ToLower();
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
    }

}

namespace UserInterface.Utility
{        
    #if UNITY_EDITOR
    [CustomEditor(typeof(UserInterface.AlphaKeyboard))]
    public class AlphaKeyboardEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AlphaKeyboard alphaKeyboard = target as AlphaKeyboard;


            if(GUILayout.Button("Create Keyboard"))
            {
                alphaKeyboard.CreateKeyboard();
            }
            
            if(GUILayout.Button("Destroy Keyboard"))
            {
                alphaKeyboard.DestroyKeyboard();
            }
        }
    }
    #endif
    
    public static partial class UserInterfaceLabels
    {
        //TODO:Implement variations(include key spacing variation, use of accent, enum identifier)
        /// <summary>The qwerty keyboard is the most commonly used english layout.</summary>
        public const string qwertyKeyboard = "qwertyuiop,asdfghjkl,zxcvbnm";
        /// <summary>The ordered keyboard has an alphabetically ordered layout.</summary>
        public const string orderedKeyboard = "abcdefghij,klmnopqrs,tuvwxyz";
        /// <summary>The azerty keyboard is a common french variation of the qwerty layout.</summary>
        public const string azertyKeyboard = "azertyuiop,qsdfghjklm,wxcvbn";
        /// <summary>The qwertz keyboard is a common central european variant of the qwerty layout.</summary>
        public const string qwertzKeyboard = "qwertzuiopü,asdfghjklöä,yxcvbnm";
        /// <summary>The qzerty keyboard is a common italian variation of the qwerty layout.</summary>
        public const string qzertyKeyboard = "qzertyuiop,asdfghjklm,wxcvbn";
        /// <summary>The dvorak keyboard is a lesser known keyboard layout known mostly for it's efficiency.</summary>
        public const string dvorakKeyboard = "pyfgcrl,aoeuidhtns,qjkxbmwvz";
        /// <summary>The colemak keyboard is a lesser known keyboard layout intended to replace the qwerty layout.</summary>
        public const string colemakKeyboard = "qwfpgjluy,arstdhneio,zxcvbkm";
        /// <summary>The jcuken keyboard is a common russian layout, and traditionally uses cyrillic characters.</summary>
        public const string jcukenKeyboard = "jcukengzh,fywaproldv,qsmitxb";
        /// <summary>The workman keyboard is a linux-based keyboard layout.</summary>
        public const string workmanKeyboard = "qdrwbjfup,ashtgyneoi,zxmcvkl";
    }
}