using UnityEngine;

namespace DataStructures
{
    using Log = Utility.DataStructuresDebug;
    using StringFormats = Utility.DataStructuresStringFormats;

    /// <summary>Static class containing functionality for <see cref="System.Array"/> 
    /// manipulation.</summary>
    public static class ArrayManipulation
    {
        /// <summary>Resizes an <see cref="System.Array"/> while retaining the data at the end of 
        /// the array.</summary>
        /// <param name="array">The <see cref="System.Array"/> to be resized.</param>
        /// <param name="newSize">The new size of the <see cref="System.Array"/>.</param>
        /// <typeparam name="T">The base type of the <see cref="System.Array"/>.</typeparam>
        /// <remarks>This method takes an array, and resizes it with 
        /// <see cref="System.Array.Resize"/>, while attempting to retain data with a "forward" 
        /// focus. If the <see cref="System.Array"/> is to be shortened, data at the end of the 
        /// array will be retained. If the <see cref="System.Array"/> is extended, existing data 
        /// will be pushed to the end. If the <see cref="System.Array"/> is extended, trailing 
        /// data will be replaced with the types default value. The <see cref="System.Array"/> 
        /// will be initialised, if it has not been already initialised.</remarks>
        public static void ResizeArrayWithGravity<T>(ref T[] array, int newSize)
        {
            ResizeArrayWithGravity<T>(ref array, newSize, default(T));
        }

        /// <summary>Resizes an <see cref="System.Array"/> while retaining the data at the end of 
        /// the array.</summary>
        /// <param name="array">The <see cref="System.Array"/> to be resized.</param>
        /// <param name="newSize">The new size of the <see cref="System.Array"/>.</param>
        /// <param name="defaultValue">The default value to use for nullifying elements in the 
        /// <see cref="System.Array"/>, where data is moved forward.</param>
        /// <typeparam name="T">The base type of the <see cref="System.Array"/>.</typeparam>
        /// <remarks>This method takes an array, and resizes it with 
        /// <see cref="System.Array.Resize"/>, while attempting to retain data with a "forward" 
        /// focus. If the <see cref="System.Array"/> is to be shortened, data at the end of the 
        /// array will be retained. If the <see cref="System.Array"/> is extended, existing data 
        /// will be pushed to the end. If the <see cref="System.Array"/> is extended, trailing 
        /// data will be replaced with defaultValue. The <see cref="System.Array"/> will be 
        /// initialised, if it has not been already initialised.</remarks>
        public static void ResizeArrayWithGravity<T>(ref T[] array, int newSize, T defaultValue)
        {
            if(array == null)
            {
                // If the array has not been initialised, simply initialise it to the newSize.
                array = new T[newSize];
            }
            else
            {
                // Else, the array has been initialised; cache the array length, and determine 
                // the sizeDifference between the newSize and the oldSize.
                int oldSize = array.Length;
                int sizeDifference = newSize - oldSize;

                if(sizeDifference > 0)
                {
                    // If the difference between the two arrays is greater than 0, we are extending 
                    // the array; resize it, first, to make room for the new values.
                    System.Array.Resize(ref array, newSize);

                    for(int i = 1; i <= oldSize; i++)
                    {
                        // For each element in the original array, move it forward to the new 
                        // extended array position, and set the defaultValue to the original 
                        // position.
                        array[newSize - i] = array[oldSize - i];
                        array[oldSize - i] = defaultValue;
                    }
                }
                else if(sizeDifference < 0)
                {
                    // Else, If the difference between the two arrays is less than 0, we are 
                    // shortening the array; we need to move the data, first. Invert the 
                    // sizeDifference, so we can treat it as a real length.
                    sizeDifference *= -1;

                    for(int i = sizeDifference; i < oldSize; i++)
                    {
                        // For each element in the original array, past the initial elements that 
                        // should be removed through the array shortening, move the element back 
                        // to the new shortened array position.
                        array[i - sizeDifference] = array[i];
                    }

                    // Resize the array.
                    System.Array.Resize(ref array, newSize);
                }
            }
        }

        #if UNITY_EDITOR
        private static string ArrayToString(int[] array)
        {
            if(array != null)
            {
                if(array.Length != 0)
                {
                    return string
                        .Format(StringFormats.arrayBase, array[0], ArrayToString(array, 1));
                }
                else
                {
                    return string.Format(StringFormats.arrayBase, "", "");
                }
            }
            else
            {
                return StringFormats.arrayIsNull;
            }
        }

        private static string ArrayToString(int[] array, int index)
        {
            if(array != null && array.Length > index)
            {
                return string.Format(StringFormats.arrayContent, array[index]) 
                    + ArrayToString(array, (index + 1));
            }
            else
            {
                return "";
            }
        }

        private static void ResetTestArray(ref int[] array, int size)
        {
            System.Array.Resize(ref array, size);

            for(int i = 0; i < size; i++)
            {
                array[i] = i + 1;
            }
        }

        public static void TestGravityArrayAdjustment(int minimumSize = 1, int maximumSize = 5, 
            bool testIncrease = true, bool testDecrease = true)
        {
            if(!testIncrease && !testDecrease)
            {
                Log.TestingForNoChanges();
                return;
            }

            if(maximumSize < 0)
            {
                maximumSize *= -1;
            }

            if(minimumSize == maximumSize)
            {
                Log.TestingInvalidAdjustment(minimumSize);
                return;
            }
            else if(minimumSize > maximumSize)
            {
                int temporarySize = maximumSize;
                maximumSize = minimumSize;
                minimumSize = temporarySize;
            }

            if(minimumSize == 0)
            {
                Log.OmittingEmptyArrayWarning();

                minimumSize = 1;
            }
            else if(minimumSize < 0)
            {
                minimumSize *= -1;
            }

            for(int i = minimumSize; i < maximumSize; i++)
            {
                for(int j = i + 1; j <= maximumSize; j++)
                {
                    string arrayOutput = "";
                    int[] testArray = new int[0];

                    if(testIncrease)
                    {
                        ResetTestArray(ref testArray, i);
                    }
                    else
                    {
                        ResetTestArray(ref testArray, j);
                    }

                    arrayOutput += ArrayToString(testArray) 
                        + (testIncrease ? StringFormats.sizeUp : StringFormats.sizeDown);

                    if(testIncrease)
                    {
                        ResizeArrayWithGravity<int>(ref testArray, j);

                        arrayOutput += ArrayToString(testArray) 
                            + (testDecrease ? StringFormats.sizeDown : "");
                    }

                    if(testDecrease)
                    {
                        ResizeArrayWithGravity<int>(ref testArray, i);

                        arrayOutput += testArray.ToString();
                    }

                    Debug.Log(arrayOutput);
                }
            }
        }
        #endif
    }
}

namespace DataStructures.Utility
{
    public static partial class DataStructuresDebug
    {
        private const string omittingEmptyArrayWarning = "Warning: Trying to test an array of "
            + "size [0]. With an initial size of [0], there will be no visible data manipulation; "
            + "size [0] will be omitted, and array will start at size [1].";
        private const string testingInvalidAdjustmentWarning = "Warning: Trying to test array "
            + "adjustments using identical array sizes. Without changing the size of the array, "
            + "there is nothing to test. Size: ";
        private const string testingForNoChangesWarning = "Warning: Trying to test array with "
            + "without testing either an increase or decrease. Without testing either of these "
            + "parameters, there is nothing to test.";

        public static void OmittingEmptyArrayWarning()
        {
            Debug.LogWarning(omittingEmptyArrayWarning);
        }

        public static void TestingInvalidAdjustment(int size)
        {
            Debug.LogWarning(testingInvalidAdjustmentWarning + size);
        }

        public static void TestingForNoChanges()
        {
            Debug.LogWarning(testingForNoChangesWarning);
        }
    }

    public static partial class DataStructuresStringFormats
    {
        public const string arrayBase = "Array [{0}{1}]";
        public const string arrayContent = ", {0}";
        public const string arrayIsNull = "Array does not currently contain any elements.";
        public const string sizeUp = " > ";
        public const string sizeDown = " < ";
    }
}