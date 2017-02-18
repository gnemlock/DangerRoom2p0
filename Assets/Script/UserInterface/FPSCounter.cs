/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/frames-per-second/ ---
 */

using UnityEngine;

namespace UserInterface
{
    using UnityEngine.UI;
    using Log = Utility.UserInterfaceDebug;
    using StringFormat = Utility.UserInterfaceStringFormats;
    
    #if UNITY_EDITOR
    using Tooltips = Utility.FPSCounterTooltips;
    #endif
    
    /// <summary>Keeps record and manages the display of the current frame rate.</summary>
    /// <remarks>The FPSCounter will keep record of the current average frame rate, across a 
    /// user defined range, as well as the current minimum and maximum values within that range.
    /// This class is a singleton, as multiple iterations of the same instance would be
    /// counter-productive.</remarks>
    public class FPSCounter : MonoBehaviour 
    {
        /// <summary>Reference to the current active instance of the <see cref="FPSCounter"/>.
        /// </summary>
        public static FPSCounter instance;
        
        /// <summary>GUI Label to display the <see cref="highestFrameRate"/> value.</summary>
        /// <remarks>This reference needs to be set up via the inspector.</remarks>
        [Tooltip(Tooltips.highestFPSLabel)] public Text highestFPSLabel;
        /// <summary>GUI Label to display the <see cref="averageFrameRate"/> value.</summary>
        /// <remarks>This reference needs to be set up via the inspector.</remarks>
        [Tooltip(Tooltips.averageFPSLabel)] public Text averageFPSLabel;
        /// <summary>GUI Label to display the <see cref="lowestFrameRate"/> value.</summary>
        /// <remarks>This reference needs to be set up via the inspector.</remarks>
        [Tooltip(Tooltips.lowestFPSLabel)] public Text lowestFPSLabel;
        
        /// <summary>The highest frame rate value with in the current range of recorded 
        /// frame rates.</summary>
        public int highestFrameRate { get; private set; }
        /// <summary>The average frame rate value across the current range of recorded 
        /// frame rates.</summary>
        public int averageFrameRate { get; private set; }
        /// <summary>The lowest frame rate value with in the current range of recorded 
        /// frame rates.</summary>
        public int lowestFrameRate { get; private set; }
        
        /// <summary>The number of recent frame rates recorded by the <see cref="FPSCounter"/>.
        /// </summary>
        /// <remarks>This value determines the size of <see cref="frameRateBuffer"/>, and in turn, 
        /// alters the range from which <see cref="highestFrameRate"/>, 
        /// <see cref="lowestFrameRate"/> and <see cref="averageFrameRate"/> are determined.
        /// </remarks>
        [SerializeField][Tooltip(Tooltips.frameRange)] private int frameRange = 60;
        /// <summary>The colour range used to colour the GUI output text.</summary>
        /// <remarks>Each colour is tied to a minimum value. Each colour should be set up in 
        /// ascending order, as the display will iterate through each index, and use the first 
        /// colour tied to a value at which the current frame rate value is equal to or exceeds.
        /// </remarks>
        [SerializeField][Tooltip(Tooltips.colourRange)] private IntColour[] colourRange;
        
        /// <summary>Contains record of the most recently recorded frame rates.</summary>
        private int[] frameRateBuffer;
        /// <summary>The current index position in the <see cref="frameRateBuffer"/>.</summary>
        private int bufferIndex;
        /// <summary>Is the <see cref="FPSCounter"/> displaying it's values to 
        /// <see cref="UnityEngine.UI.Text"/> components?</summary>
        /// <remarks>This is a simple alternative to constantly checking if the 
        /// <see cref="UnityEngine.UI.Text"/> components are <c>null</c>. By setting this value to 
        /// <c>false</c>, the <see cref="FPSCounter"/> will still calculate frame rates, but will 
        /// not attempt to push the values to a display.</remarks>  
        [SerializeField][Tooltip(Tooltips.displayToText)] private bool displayToText = true;
        
        /// <summary>Pre-created strings for all possible values, ranging from 00 to 99.</summary>
        /// <remarks>The frame rate labels will only display values between 0 and 99, so all 
        /// possible strings are pre-created, to remove the need for dynamically create strings. 
        /// This also limits garbage collection, as strings do not need to be disposed.</remarks>
        private static string[] stringsFrom00To99 =
        {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", 
            "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", 
            "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", 
            "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", 
            "56", "57", "58", "59", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", 
            "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "80", "81", "82", "83", 
            "84", "85", "86", "87", "88", "89", "90", "91", "92", "93", "94", "95", "96", "97", 
            "98", "99"
        };
        
        #region Constructors
        /// <summary>Initializes a new instance of the <see cref="UserInterface.FPSCounter"/> class 
        /// to display frame rate values with no colour transition. Note that this class is a 
        /// singleton; as such, additional instances will be quickly removed, without 
        /// enabling the override.</summary>
        /// <param name="highestFPSLabel">The text box for displaying the highest FPS value.</param>
        /// <param name="averageFPSLabel">The text box for displaying the average FPS value.</param>
        /// <param name="lowestFPSLabel">The text box for displaying the lowest FPS value.</param>
        /// <param name="frameRange">The number of recent frames to keep record of.</param>
        /// <param name="overrideOriginal">If set to <c>true</c>, the created 
        /// <see cref="UserInterface.FPSCounter"/> will override any existing instance, preventing 
        /// this instance from being deleted as a singleton class.</param>
        public FPSCounter(Text highestFPSLabel, Text averageFPSLabel, Text lowestFPSLabel, 
            int frameRange = 60, bool overrideOriginal = false)
        {
            if(overrideOriginal && instance != null)
            {
                // If we are overriding the original FPSCounter, and we already have an instance 
                // of FPSCounter, destroy the older instance and set the instance reference to
                // null, so the Awake() method reset the instance reference.
                Destroy(instance);
                instance = null;
            }
            
            // Set the passed in parameters.
            this.highestFPSLabel = highestFPSLabel;
            this.averageFPSLabel = averageFPSLabel;
            this.lowestFPSLabel = lowestFPSLabel;
            this.frameRange = frameRange;
            
            // We are displaying, as we have Text references.
            displayToText = true;
            
            // Set a default colour, so all values display in white.
            colourRange = new IntColour[] { new IntColour(Color.white, 0) };
        }
        
        /// <summary>Initializes a new instance of the <see cref="UserInterface.FPSCounter"/> class 
        /// to display frame rate values with colour transition. Note that this class is a 
        /// singleton; as such, additional instances will be quickly removed, without 
        /// enabling the override.</summary>
        /// <param name="highestFPSLabel">The text box for displaying the highest FPS value.</param>
        /// <param name="averageFPSLabel">The text box for displaying the average FPS value.</param>
        /// <param name="lowestFPSLabel">The text box for displaying the lowest FPS value.</param>
        /// <param name="colourRange">The <see cref="UserInterface.IntColour"/> range used to 
        /// colour the values. This class will use minimum values, so each set should contain the 
        /// minimum desired value to reflect the corresponding colour.</param>
        /// <param name="frameRange">The number of recent frames to keep record of.</param>
        /// <param name="overrideOriginal">If set to <c>true</c>, the created 
        /// <see cref="UserInterface.FPSCounter"/> will override any existing instance, preventing 
        /// this instance from being deleted as a singleton class.</param>
        public FPSCounter(Text highestFPSLabel, Text averageFPSLabel, Text lowestFPSLabel, 
            IntColour[] colourRange, int frameRange = 60, bool overrideOriginal = false)
        {
            if(overrideOriginal && instance != null)
            {
                // If we are overriding the original FPSCounter, and we already have an instance 
                // of FPSCounter, destroy the older instance and set the instance reference to
                // null, so the Awake() method reset the instance reference.
                Destroy(instance);
                instance = null;
            }
            
            // Set the passed in parameters.
            this.highestFPSLabel = highestFPSLabel;
            this.averageFPSLabel = averageFPSLabel;
            this.lowestFPSLabel = lowestFPSLabel;
            this.frameRange = frameRange;
            this.colourRange = colourRange;
            
            // We are displaying, as we have Text references.
            displayToText = true;
        }
        
        /// <summary>Initializes a new instance of the <see cref="UserInterface.FPSCounter"/> class 
        /// to simply keep record of the current frame rate, without display. Note that this class 
        /// is a singleton; as such, additional instances will be quickly removed, without 
        /// enabling the override.</summary>
        /// <param name="frameRange">The number of recent frames to keep record of.</param>
        /// <param name="overrideOriginal">If set to <c>true</c>, the created 
        /// <see cref="UserInterface.FPSCounter"/> will override any existing instance, preventing 
        /// this instance from being deleted as a singleton class.</param>
        public FPSCounter(int frameRange = 60, bool overrideOriginal = false)
        {
            if(overrideOriginal && instance != null)
            {
                // If we are overriding the original FPSCounter, and we already have an instance 
                // of FPSCounter, destroy the older instance and set the instance reference to
                // null, so the Awake() method reset the instance reference.
                Destroy(instance);
                instance = null;
            }
            
            // Set the passed in parameter.
            this.frameRange = frameRange;
            
            // We are not displaying, as we have no Text references.
            displayToText = false;
        }
        #endregion
        
        #region MonoBehaviour Events and Parent Overrides
        /// <summary>This method is called when the instance is loaded.</summary>
        private void Awake()
        {
            if(instance == null)
            {
                // If we have not set up an instance reference, yet, set this instance as the 
                // instance reference.
                instance = this;
            }
            else if(instance != this)
            {
                // Else, if we have set up the instance reference, and it does not refer to this
                // instance, we have a duplicate. Push a debug warning, and destroy this instancce.
                Log.AdditionalFPSCounterWarning(this);
                Destroy(this);
            }
        }
        
        /// <summary>This method is called every frame, presuming the target game object is 
        /// enabled.</summary>
        private void Update()
        {
            if(frameRateBuffer == null || frameRateBuffer.Length != frameRange)
            {
                // If the frame rate buffer has not been set up, or if there has been a change 
                // to the required frame range, initialise the frame rate buffer.
                InitialiseFrameRateBuffer();
            }
            
            // Update the frame rate buffer and re-calculate the FPS values.
            UpdateFrameRateBuffer();
            CalculateFPS();
            
            if(displayToText)
            {
                // If we are to display the values, update the new FPS values to the corresponding 
                // text boxes.
                DisplayFPS(highestFPSLabel, highestFrameRate);
                DisplayFPS(averageFPSLabel, averageFrameRate);
                DisplayFPS(lowestFPSLabel, lowestFrameRate);
            }
        }
        
        /// <summary>Returns a preformated string containing <see cref="lowestFrameRate"/>, 
        /// <see cref="highestFrameRate"/> and <see cref="averageFrameRate"/>.</summary>
        /// <returns>A preformated string containing <see cref="lowestFrameRate"/>, 
        /// <see cref="highestFrameRate"/> and <see cref="averageFrameRate"/>.</returns>
        public override string ToString()
        {
            return string.Format(StringFormat.FPSCounter, highestFrameRate, averageFrameRate, 
                lowestFrameRate);
        }
        #endregion
        
        /// <summary>Updates <see cref="lowestFrameRate"/>, <see cref="highestFrameRate"/> and 
        /// <see cref="averageFrameRate"/> based off the current set of values in 
        /// <see cref="frameRateBuffer"/>.</summary>
        private void CalculateFPS()
        {
            // Create temporary placeholders to work out the sum, highest and lowest values.
            int sum = 0;
            int highest = 0;
            int lowest = int.MaxValue;
            
            for(int i = 0; i < frameRange; i++)
            {
                // For each value in frame range, add to the current sum value.
                int frameRate = frameRateBuffer[i];
                sum += frameRate;
                
                if(frameRate > highest)
                {
                    // If the frame rate is higher than the current highest frame rate placeholder, 
                    // set this frame rate as the new highest frame rate placeholder.
                    highest = frameRate;
                }
                else if(frameRate < lowest && frameRate != 0)
                {
                    // Else, if the frame rate is lower than the current lowest frame rate 
                    // placeholder, and not a default value of 0, set this frame rate as the new 
                    // lowest frame rate placeholder.
                    lowest = frameRate;
                }
            }
            
            // Now we have the placeholder sum, highest and lowest values from iterating through 
            // the current range of frame rate values, we can set them to the class variables.
            averageFrameRate = sum / frameRange;
            highestFrameRate = highest;
            lowestFrameRate = lowest;
        }
        
        /// <summary>Updates a <see cref="Text"/> label with a corresponding frame rate value, and 
        /// updates the text colour according to the values set in <see cref="colourRange"/>.
        /// </summary>
        /// <param name="fpsLabel">The text field used to display the frame rate value.</param>
        /// <param name="fpsValue">The frame rate value.</param>
        private void DisplayFPS(Text fpsLabel, int fpsValue)
        {
            // Set the label text as the pre-defined string defined by the frame rate value, 
            // clamped between 0 and 99.
            fpsLabel.text = stringsFrom00To99[Mathf.Clamp(fpsValue, 0, 99)];
            
           for(int i = 0; i < colourRange.Length; i++)
            {
                if(fpsValue >= colourRange[i].value)
                {
                    // For each set in the colour range, see if the frame rate value is greater 
                    // than or equal to the corresponding colour value; at the first instance where 
                    // it is, set this colour as the text colour, and break out of the for loop.
                    fpsLabel.color = colourRange[i].colour;
                    break;
                }
            }
        }
        
        /// <summary>Initialises <see cref="frameRateBuffer"/>.</summary>
        /// <remarks>Ensures that <see cref="frameRange"/> is greater than <c>0</c>, initialises 
        /// <see cref="frameRateBuffer"/> as a new array and resets the <see cref="bufferIndex"/> 
        /// to <c>0</c>.</remarks> 
        private void InitialiseFrameRateBuffer()
        {
            if(frameRange <= 0)
            {
                // If the frame range is set to 0 or less, reset it to 1.
                frameRange = 1;
            }
            
            // Create a new array for the frame rate buffer, using the new frame range, and 
            // reset the buffer index to 0.
            frameRateBuffer = new int[frameRange];
            bufferIndex = 0;
        }
        
        /// <summary>Determines wether the <see cref="FPSCounter"/> is displaying to a 
        /// <see cref="UnityEngine.UI.Text"/> interface, or only calculating the frame rate.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="FPSCounter"/> is displaying; otherwise, 
        /// <c>false</c>.</returns>
        public bool IsDisplayingToInterface()
        {
            return displayToText;
        }
        
        /// <summary>Updates the <see cref="frameRateBuffer"/> array with a new frame rate reading, 
        /// and adjusts <see cref="bufferIndex"/> to accomodate to the next update.</summary>
        private void UpdateFrameRateBuffer()
        {
            // Find the per-second time it took to complete the last frame, and set it to the 
            // current index of frame rate buffer, before immediately incrementing the index.
            frameRateBuffer[bufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
            
            if(bufferIndex >= frameRange)
            {
                // If the new buffer index is greater than or equal to the desired frame range, 
                // we have reached the end of the array, so reset the index back to 0.
                bufferIndex = 0;
            }
        }
    }
}

namespace UserInterface.Utility
{
    /// <summary>This class holds debug functionality for the UserInterface namespace.</summary>
    public static partial class UserInterfaceDebug
    {
        private const string additionalFPSCounterWarning = "Warning: Detecting multiple iterations " 
            + "of FPSCounter. Removing newest instance.";
        
        /// <summary>Provides debug concerning additional instances of the singleton FPSCounter.
        /// </summary>
        /// <param name="fpsCounter">The new FPSCounter.</param>
        /// <remarks>FPSCounter is a singleton class, and as such, additional instances will be 
        /// deleted. This warning alerts users to the fact that there were multiple instances 
        /// of the FPSCounter singleton class.</remarks>
        public static void AdditionalFPSCounterWarning(FPSCounter fpsCounter)
        {
            Debug.Log(additionalFPSCounterWarning, fpsCounter);
        }
    }
    
    /// <summary>This class holds string formats for the UserInterface namespace.</summary>
    public static partial class UserInterfaceStringFormats
    {
        /// <summary>Formats the values of an FPSCounter, with the intention of being formatted 
        /// with the highestFrameRate, lowestFrameRate and averageFrameRate.</summary>
        public const string FPSCounter = "FPS (Highest = {0} | Lowest = {1} | Average = {2})";
    }
    
    /// <summary>This class holds the tooltips for variables serialized to the inspector.</summary>
    public static class FPSCounterTooltips
    {
        #if UNITY_EDITOR
        public const string highestFPSLabel = "GUI Text to display highest FPS value.";
        public const string averageFPSLabel = "GUI Text to display average FPS value.";
        public const string lowestFPSLabel = "GUI Text to display lowest FPS value.";
        public const string frameRange = "The number of recent frame rates to keep record of " 
            + "in order to determine average values";
        public const string colourRange = "Colour and minimum value sets, used to colour code "
            + "the GUI text output.";
        public const string displayToText = "Is this FPSCounter displaying its values to "
            + "GUI Text fields?";
        #endif
    }
}