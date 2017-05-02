using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine;

namespace Narrative
{
    #if UNITY_EDITOR
    using Tooltips = Narrative.Utility.ScrollingDialogueTooltips;
    #endif

    //TODO:Allow Text Speeding(perhaps convert keypress to key up, and implement as keyhold)
    //TODO:Figure out why text is not printing
    //TODO:Sort out default prefab
    //TODO:Sort out resolution changes
    //TODO:Documentation
    //TODO:Implement default sprites

    [RequireComponent(typeof(Text))] public class ScrollingDialogue : MonoBehaviour
    {
        /// <summary>The <see cref="UnityEngine.UI.Text"/> element to print text to.</summary>
        [Tooltip(Tooltips.text)] public Text text;
        /// <summary>The <see cref="UnityEngine.UI.Image"/> element to display the actor 
        /// image to.</summary>
        /// <remarks>If there is no <see cref="UnityEngine.UI.Image"/> linked up, it will simply 
        /// be ignored.<remarks>
        [Tooltip(Tooltips.actorImage)] public Image actorImage;
        /// <summary>The <see cref="UnityEngine.UI.Image"/> element to display the backing 
        /// image to.</summary>
        /// <remarks>If there is no <see cref="UnityEngine.UI.Image"/> linked up, it will simply 
        /// be ignored.<remarks>
        [Tooltip(Tooltips.backingImage)] public Image backingImage;
        /// <summary>The <see cref="UnityEngine.Font"/> to use, when no actor font has been 
        /// specified.</summary>
        [Tooltip(Tooltips.defaultFont)] public Font defaultFont;
        /// <summary>The intended time between printing each character, in seconds.</summary>
        [Tooltip(Tooltips.characterPacing)] public float characterPacing = 0.5f;
        /// <summary>The intended time between printing each character, in seconds, when printing 
        /// has been sped up.</summary>
        [Tooltip(Tooltips.characterFastPacing)] public float characterFastPacing = 0.1f;
        /// <summary>The <see cref="UnityEngine.KeyCode"/> used to trigger fast scrolling, 
        /// line completion and next line.</summary>
        [Tooltip(Tooltips.nextLineKey)] public KeyCode nextLineKey;

        /// <summary>The current dialogue being displayed to the 
        /// <see cref="Narrative.ScrollingDialogue.text"/> element.</summary>
        private string currentDialogue = "";
        /// <summary>Can the <see cref="Narrative.ScrollingDialogue"/> read the next line?</summary>
        /// <remarks>This boolean flags the <see cref="UnityEngine.MonoBehaviour.Update"/> method 
        /// to start the <see cref="Narrative.ScrollingDialogue.ReadLine"/> coroutine.</remarks>
        private bool canReadNextLine = true;
        /// <summary>Is the <see cref="Narrative.ScrollingDialogue"/> waiting to proceed to the 
        /// next line?</summary>
        /// <remarks>This boolean flags whether the 
        /// <see cref="Narrative.ScrollingDialogue.nextLineKey"/> triggers 
        /// <see cref="Narrative.ScrollingDialogue.MoveToNextLine"/> or 
        /// <see cref="Narrative.ScrollingDialogue.CompleteCurrentLine"/>.</remarks>
        private bool waitingForNextLine = false;
        /// <summary>The current character index, in the current line of the 
        /// <see cref="Narrative.ScrollingDialogue.script"/> being read from.</summary>
        /// <remarks>The <see cref="Narrative.ScrollingDialogue"/> uses this index to determine the 
        /// next character to print to the <see cref="Narrative.ScrollingDialogue.text"/> element.
        /// </remarks>
        private int currentCharacterIndex = 0;
        /// <summary>The current line index in the <see cref="Narrative.ScrollingDialogue.script"/> 
        /// being read from.</summary>
        private int currentLineIndex = 0;
        /// <summary>The current <see cref="Narrative.Dialogue"/> script being read from.</summary>
        private Dialogue script;
        /// <summary>A locally cached dictionary of <see cref="Narrative.ActorListing"/>s specific 
        /// to the current <see cref="Narrative.ScrollingDialogue.script"/>.</summary>
        /// <remarks>The <see cref="Narrative.ActorListing"/>s are stored with an <see cref="int"/> 
        /// key, so they may still be accessed by their original position in the larger 
        /// <see cref="Narrative.ActorList"/>.</remarks>
        private Dictionary<int, Actor> localActorList;
        /// <summary>The name of the current actor associated with the 
        /// <see cref="Narrative.ScrollingDialogue.currentLineIndex"/> in the current 
        /// <see cref="Narrative.ScrollingDialogue.script"/>.</summary>
        /// <remarks>This string is cached, locally, to make displaying the actor easier in the 
        /// <see cref="Narrative.ScrollingDialogue.currentDialogue"/>. Will be ignored, if left 
        /// empty.</remarks>
        private string currentActor;

        /// <summary>This method will be called when this instance of 
        /// <see cref="Narrative.ScrollingDialogue"/> is loaded.</summary>
        private void Awake()
        {
            // Reset the reader to ensure all parameters start at their appropriate values.
            ResetReader();
            // For testing purposes, start the dialogue immediately.
            StartScrollingDialogue(NarrationManager.instance.script.dialogue[0]);
        }

        /// <summary>This method will be called at the start of each frame where this instance of 
        /// <see cref="Narrative.ScrollingDialogue"/> is enabled.</summary>
        private void Update()
        {
            if(canReadNextLine)
            {
                // If the ScrollingDialogue is flagged to read the next line, reset the flag 
                // so it does not attempt to read the next line on the next Update frame, and start 
                // the "ReadLine" coroutine to start reading the next line.
                canReadNextLine = false;
                StartCoroutine("ReadLine");
            }
            
            if(Input.GetKeyDown(nextLineKey))
            {
                if(waitingForNextLine)
                {
                    MoveToNextLine();
                }
                else
                {
                    CompleteCurrentLine();
                }
            }
        }
        
        private void ReturnControl()
        {
        }
        
        #region Actor Manipulation
        
        /// <summary>Checks if the current line requires an actor change, and changes the actor, 
        /// if it does.</summary>
        private void CheckForNewActor()
        {
            int currentActorID = script.lines[currentLineIndex].actorID;
            
            if(currentLineIndex != 0)
            {
                SetActor(currentActorID);
            }
            else if(script.lines[currentLineIndex - 1].actorID != currentActorID)
            {
                SetActor(currentActorID);
            }
        }
        
        /// <summary>Sets a new actor.</summary>
        /// <param name="actorID">New Actor ID.</param>
        public void SetActor(int actorID)
        {
            Actor actor;
            localActorList.TryGetValue(actorID, out actor);
            currentActor = actor.name;

            if(actorImage != null && actor.actorSprite != null)
            {
                actorImage.sprite = actor.actorSprite;
            }

            if(backingImage != null && actor.backingSprite != null)
            {
                backingImage.sprite = actor.backingSprite;
            }

            if(actor.font != null)
            {
                text.font = actor.font;
            }
            else
            {
                text.font = defaultFont;
            }

            if(actor.colour != null)
            {
                text.color = actor.colour;
            }
            else
            {
                text.color = Color.black;
            }
        }
        #endregion
        
        #region Line Manipulation
        private void CompleteCurrentLine()
        {
            StopCoroutine(ReadLine());
            currentDialogue = script.lines[currentLineIndex].line;
            canReadNextLine = false;
            waitingForNextLine = true;
        }
        
        private void MoveToNextLine()
        {
            currentLineIndex++;

            if(currentLineIndex >= script.lines.Length)
            {
                ReturnControl();
                ActivateTextUI(false);
            }
            else
            {
                CheckForNewActor();
                currentCharacterIndex = 0;
                canReadNextLine = true;
                waitingForNextLine = false;
            }
        }
        
        private IEnumerator ReadLine()
        {
            string line = script.lines[currentLineIndex].line;
            int length = line.Length;
            currentDialogue = (currentActor != "" ? currentActor + ": " : "");

            while(currentCharacterIndex < length)
            {
                currentDialogue += line[currentCharacterIndex];
                UpdateText();
                currentCharacterIndex++;
                yield return new WaitForSeconds(characterPacing);
            }
            
            waitingForNextLine = true;
        }
        #endregion
        
        #region UI Manipulation
        private void ActivateTextUI(bool active)
        {
            text.gameObject.SetActive(active);
        }

        private void ResetReader()
        {
            currentDialogue = "";
            UpdateText();
            currentCharacterIndex = 0;
            canReadNextLine = true;
            waitingForNextLine = false;
        }
        
        public void StartScrollingDialogue(Dialogue dialogue)
        {
            script = dialogue;
            localActorList = new Dictionary<int, Actor>();

            for(int i = 0; i < dialogue.lines.Length; i++)
            {
                int actorID = dialogue.lines[i].actorID;

                if(!localActorList.ContainsKey(actorID))
                {
                    localActorList.Add(actorID, NarrationManager.instance.GetActorListing(actorID));
                }
            }

            ActivateTextUI(true);
            SetActor(script.lines[0].actorID);
        }

        private void UpdateText()
        {
            text.text = currentDialogue;
        }
        #endregion
    }
    
    public interface IIdler
    {
        void Continue();
    }
}

namespace Narrative.Utility
{
    #if UNITY_EDITOR
    public static class ScrollingDialogueTooltips
    {
        public const string characterPacing = "Regular time between characters, in seconds.";
        public const string characterFastPacing = "Sped up time between characters, in seconds.";
        public const string nextLineKey = "The key used to skip to the end of the line,"
            + " or move to the next.";
        public const string text = "The Text UI to display dialogue to.";
        public const string actorImage = "The Image UI to display the actor sprite to."
            + " Will be ignored, if NULL.";
        public const string backingImage = "The Image UI to display the backing sprite to."
            + " Will be ignored, if NULL.";
        public const string defaultFont = "The default font face to use, if none is specified.";
    }
    #endif
}