using System.Collections;

using UnityEngine.UI;
using UnityEngine;

namespace Narrative
{
    [RequireComponent(typeof(Text))]
    public class ScrollingDialogue : MonoBehaviour
    {
        public float characterPacing = 0.5f;
        public float characterFastPacing = 0.1f;
        public Dialogue script;
        public KeyCode nextLineKey;

        private Text text;
        public string currentDialogue = "";
        public bool canAddNextLetter = true;
        public bool canReadNextLine = true;
        public bool waitingForNextLine = false;
        public int currentCharacterIndex = 0;
        public int currentLineIndex = 0;

        private void Awake()
        {
            text = GetComponent<Text>();
            ResetReader();
        }
        
        private void Update()
        {
            if(canReadNextLine)
            {
                canReadNextLine = false;
                StartCoroutine("ReadLine");
            }
            
            if(Input.GetKeyDown(nextLineKey))
            {
                StopCoroutine(ReadLine());
                ResetReader();
                
                if(waitingForNextLine)
                {
                    currentLineIndex++;
                    
                    if(currentLineIndex >= script.lines.Length)
                    {
                        ActivateTextUI(false);
                    }
                }
                else
                {
                    currentDialogue = script.lines[currentLineIndex].line;
                    canReadNextLine = false;
                    waitingForNextLine = true;
                }
            }
        }
        
        private void ActivateTextUI(bool active)
        {
            text.gameObject.SetActive(active);
        }

        public void ReadLines(Dialogue script)
        {
            this.script = script;
            ActivateTextUI(true);
        }
        
        private void ResetReader()
        {
            currentDialogue = "";
            UpdateText();
            currentCharacterIndex = 0;
            canAddNextLetter = true;
            canReadNextLine = true;
            waitingForNextLine = false;
        }

        private IEnumerator ReadLine()
        {
            string line = script.lines[currentLineIndex].line;
            int length = line.Length;
            
            Debug.Log(line + " | characterPacing = " + characterPacing + " | length = " + length);
            
            while(currentCharacterIndex < length)
            {
                Debug.Log(Time.time + ": " + currentCharacterIndex);
                currentDialogue += line[currentCharacterIndex];
                UpdateText();
                currentCharacterIndex++;
                Debug.Log(currentDialogue);
                yield return new WaitForSeconds(characterPacing);
            }
            
            canAddNextLetter = false;
            waitingForNextLine = true;
        }

        private void UpdateText()
        {
            text.text = currentDialogue;
        }
    }
    
    public interface IIdler
    {
        void Continue();
    }
}