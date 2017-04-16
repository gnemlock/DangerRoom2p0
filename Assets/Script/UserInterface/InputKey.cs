using UnityEngine;

namespace UserInterface
{
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class InputKey : Key
    {
        [SerializeField][HideInInspector] private char key;
        public Text text;
        
        
        
        public void ApplyInput()
        {
            parentKeyboard.AddInput(GetKey());
            Debug.Log(key.ToString());
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
    }
}