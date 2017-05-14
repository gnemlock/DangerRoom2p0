using UnityEngine;

namespace DataStructures
{
    /// <summary>Static class containing functionality for <see cref="System.Array"/> 
    /// manipulation.</summary>
    public static class ArrayManipulation
    {
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
        public static void ResizeForwardArray<T>(T[] array, int newSize, T defaultValue)
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

                    for(int i = 1; i < oldSize; i++)
                    {
                        // for each element in the original  array, move it forward to the new 
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

                    for(int i = oldSize - 1; i > sizeDifference; i--)
                    {
                        // for each element in the original array, leading back from the end of the 
                        // original array to the length of the shortened array, move the 
                        // corresponding value back, to sit in line with the shortened array.
                        array[i - sizeDifference] = array[i];
                    }

                    // Resize the array.
                    System.Array.Resize(ref array, newSize);
                }
            }
        }
        
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
        public static void ResizeForwardArray<T>(T[] array, int newSize)
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

                    for(int i = 1; i < oldSize; i++)
                    {
                        // for each element in the original  array, move it forward to the new 
                        // extended array position, and set the default value of the corresponding 
                        // type to the original position.
                        array[newSize - i] = array[oldSize - i];
                        array[oldSize - i] = default(T);
                    }
                }
                else if(sizeDifference < 0)
                {
                    // Else, If the difference between the two arrays is less than 0, we are 
                    // shortening the array; we need to move the data, first. Invert the 
                    // sizeDifference, so we can treat it as a real length.
                    sizeDifference *= -1;

                    for(int i = oldSize - 1; i > sizeDifference; i--)
                    {
                        // for each element in the original array, leading back from the end of the 
                        // original array to the length of the shortened array, move the 
                        // corresponding value back, to sit in line with the shortened array.
                        array[i - sizeDifference] = array[i];
                    }

                    // Resize the array.
                    System.Array.Resize(ref array, newSize);
                }
            }
        }
    }
}