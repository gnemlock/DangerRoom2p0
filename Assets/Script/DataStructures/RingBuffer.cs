/* 
 * Created by Matthew Francis Keating, based off code supplied by "Bunny83" 
 * --------- http://answers.unity3d.com/answers/1360603/view.html ---------
 */

using System.Collections;
using System.Collections.Generic;

namespace DataStructures
{
    using Log = Utility.DataStructuresDebug;

    /// <summary>Represents a circular array, commonly used to hold buffer data.</summary>
    /// <remarks>A <see cref="DataStructures.RingBuffer"/> is intended to hold temporary data, with 
    /// the intention of removing older data in favour of newer data. Every time data is added, an 
    /// internal indexor moves forward, instead of shifting all elements down. This indexor is then 
    /// referenced as the origin, when treating the data via index.</remarks>
    public class RingBuffer<T> : ICollection<T>, IList<T> 
    {
        /// <summary>The data held by this <see cref="DataStructures.RingBuffer"/>.</summary>
        private T[] data;
        /// <summary>The number of valid elements in the <see cref="DataStructures.data"/> 
        /// array.</summary>
        /// <remarks>Does not include default values, where elements have not been added to the 
        /// <see cref="DataStructures.data"/> array, yet.</remarks>
        private int count;
        /// <summary>The current index to start reading data from.</summary>
        private int readIndex;
        /// <summary>The current index to write data to.</summary>
        private int writeIndex;
        /// <summary>The current version of this <see cref="DataStructures.RingBuffer"/>.</summary>
        /// <remarks>This value is primarily used to check for changes to the original 
        /// <see cref="DataStructures.RingBuffer"/> during unsafe behaviour.</remarks>
        private int version;

        /// <summary>The number of valid elements in the <see cref="DataStructures.data"/> 
        /// array.</summary>
        /// <value>Immediately returns <see cref="DataStructures.RingBuffer.count"/>.</value>
        /// <remarks>Does not include default values, where elements have not been added to the 
        /// <see cref="DataStructures.data"/> array, yet. Immediately returns 
        /// <see cref="DataStructures.RingBuffer.count"/>, providing public interface.</remarks>
        public int Count { get { return count; } }
        /// <summary>Provides public interface to check whether this instance of 
        /// <see cref="DataStructures.RingBuffer"/> is read only.</summary>
        /// <value><c>false</c>, as the <see cref="DataStructures.RingBuffer"/> is never read-only.
        /// </value>
        /// <remarks>Required as part of the included interface.</remarks>
        public bool IsReadOnly { get { return false; } }

        public int Length{ get { return data.Length; } }

        /// <summary>Gets or sets the <see cref="DataStructures.RingBuffer.data"/> element at the 
        /// specified index.</summary>
        /// <param name="index">The index of the <see cref="DataStructures.RingBuffer.data"/> 
        /// element to be accessed.</param>
        public T this[int index]
        {
            get
            {
                if(count == 0)
                {
                    // If there is currently no valid elements in the data array, log the error, 
                    // and return the default value for the type.
                    Log.EmptyRingBufferError();
                    return default(T);
                }
                else
                {
                    // Else, there are valid elements in the data array; determine the true index 
                    // by counting from the readIndex, and reseting at the end of the array. Return 
                    // the data pointed to by this derived index.
                    index = (readIndex + index) % data.Length;
                    return data[index];
                }
            }

            set
            {
                if(count == 0)
                {
                    // If there is currently no valid elements in the data array, log the error.
                    Log.EmptyRingBufferError();
                }
                else
                {
                    // Els
                    index = (readIndex + index) % data.Length;
                    data[index] = value;
                }
            }
        }

        /// <summary>Creates a new instance of <see cref="DataStructures.RingBuffer"/>, with an 
        /// empty buffer.</summary>
        /// <param name="size">The element size of the buffer.</param>
        public RingBuffer(int size)
        {
            // Create a new array of the specified size, and initialise the additional values.
            data = new T[size];
            count = 0;
            readIndex = 0;
            writeIndex = 0;
            version = 0;
        }

        /// <summary>Creates a new instance of <see cref="DataStructures.RingBuffer"/>, with a 
        /// specified buffer.</summary>
        /// <param name="data">The data to include, as the buffer.</param>
        public RingBuffer(T[] data)
        {
            // Set the passed array as the data array, and initialise the additional values.
            this.data = data;
            count = data.Length;
            readIndex = 0;
            writeIndex = 0;
            version = 0;
        }

        /// <summary>Adds a new element to the <see cref="DataStructures.RingBuffer.data"/> 
        /// array, at the<see cref="DataStructures.RingBuffer.writeIndex"/>.</summary>
        /// <param name="newData">The new data element to add to the 
        /// <see cref="DataStructures.RingBuffer.data"/> array.</param>
        public void Add(T newData)
        {
            // Write the passed data to the current writeIndex, and increment writeIndex. Wrap 
            // writeIndex, so if it exceeds the limitations of the data array, it resets back to 
            // the start.
            data[writeIndex] = newData;
            writeIndex = (writeIndex + 1) % data.Length;

            if(count == data.Length)
            {
                // If the count is equal to the array length, the array is already full of 
                // valid elements; we should rotate the ring buffer. Increment readIndex, and wrap 
                // the value, so if it exceeds the limitations of the data array, it resets back to 
                // the start.
                readIndex =- (readIndex + 1) % data.Length;
            }
            else
            {
                // Else, the count is not yet equal to the array length, and the array is not yet 
                // full of valid elements. Increment count, to reflect an additional valid element.
                count++;
            }

            // Increment the version number, to reflect the change.
            version++;
        }

        /// <summary>Clears the elements in the <see cref="DataStructures.RingBuffer.data"/> array, 
        /// and resets them to the default value for the type.</summary>
        public void Clear()
        {
            for(int i = 0; i < data.Length; i++)
            {
                // For each element in the data array, make the element the 
                // default value for the type.
                data[i] = default(T);
            }

            // Reset the index and count values, and incremenet the version number 
            // to reflect the change.
            readIndex = 0;
            writeIndex = 0;
            count = 0;
            version++;
        }

        /// <summary>Checks if the <see cref="DataStructures.RingBuffer.data"/> array contains a 
        /// specific item.</summary>
        /// <returns><c>True</c>, if the passed item exists in the 
        /// <see cref="DataStructures.RingBuffer.data"/> array, else returns <c>False</c>.</returns>
        /// <param name="item">The item to check for, in the 
        /// <see cref="DataStructures.RingBuffer.data"/> array.</param>
        public bool Contains(T item)
        {
            for(int i = 0; i < data.Length; i++)
            {
                if(EqualityComparer<T>.Default.Equals(item, data[i]))
                {
                    // For each element in the data array, if the element passes an equality 
                    // comparison with the item, we have a match; return true.
                    return true;
                }
            }

            // If we get to this point, no element has matched with the item; return false.
            return false;
        }

        /// <summary>Copies the elements in the local <see cref="DataStructures.RingBuffer.data"/> 
        /// array to a specified array.</summary>
        /// <param name="destinationArray">The destination array of which to copy the local 
        /// <see cref="DataStructures.RingBuffer.data"/> array to.</param>
        /// <param name="destinationArrayIndex">The index in the destination array of which to 
        /// start copying elements from the local 
        /// <see cref="DataStructures.RingBuffer.data"/> array to.</param>
        /// <remarks>This method will start copying data from the local 
        /// <see cref="DataStructures.RingBuffer.data"/> array at the local 
        /// <see cref="DataStructures.RingBuffer.readIndex"/>. As such, the local 
        /// <see cref="DataStructures.RingBuffer.data"/> will be the first element placed into the 
        /// new array, with a second <see cref="System.Array.Copy"/> method called to ensure 
        /// trailing data is included.</remarks>
        public void CopyTo(T[] destinationArray, int destinationArrayIndex = 0)
        {
            if(destinationArray == null)
            {
                // If the destinationArray is currently null, log the error, and return.
                Log.RingBufferCopyToPassedNullArrayError();
                return;
            }

            if(destinationArrayIndex < 0 || destinationArrayIndex >= destinationArray.Length)
            {
                // If the destinationArrayIndex is an invalid inxex in the destinationArray, 
                // log the error, and return.
                Log.RingBufferCopyToPassedOutOfRangeIndexError(destinationArrayIndex, 
                    destinationArray.Length);
                return;
            }

            if((destinationArray.Length - destinationArrayIndex) < count)
            {
                // If the destinationArray can not fit the internal data, log the error, and return.
                Log.RingBufferCopyToArrayTooSmallError(destinationArray.Length, 
                    destinationArrayIndex, data.Length);
                return;
            }

            // Determine the initial copy length, which will either be the number of valid elements  
            // in the local data array, or the length of the local data array proceeding the 
            // current local readIndex; whichever is smaller. Copy data from the local data array, 
            // starting from the current local readIndex, into the destinationArray, starting from 
            // the passed destinationArrayIndex, for the length of the determined initialCopyLength.
            int initialCopyLength = System.Math.Min(count, data.Length - readIndex);
            System.Array
                .Copy(data, readIndex, destinationArray, destinationArrayIndex, initialCopyLength);

            if(initialCopyLength < count)
            {
                // If the initialCopyLength is less than the count of valid elements in the local 
                // data array, we have not copied the current end of the local data array, which 
                // is located at the start of the array. Increment the destinationArrayIndex 
                // by the initialCopyLength, in order to accomodate for the elements we have 
                // already copied, and copy the rest of the data from the local data array, 
                // starting at the start, to the destinationArray, starting at the new 
                // destinationArrayIndex, for the length of the remaining valid elements.
                destinationArrayIndex += initialCopyLength;
                System.Array.Copy(data, 0, destinationArray, destinationArrayIndex, 
                    count - initialCopyLength);
            }
        }

        /// <summary>Returns an enumerator for iteration through the 
        /// <see cref="DataStructures.RingBuffer.data"/> array.</summary>
        /// <returns>The enumerator.</returns>
        /// <remarks>At each iteration, the <see cref="DataStructures.RingBuffer.version"/> is 
        /// checked against an initial value. A change in this value reflects an external change to 
        /// the <see cref="DataStructures.RingBuffer.data"/> array, and will cancel this method.
        /// </remarks>
        public IEnumerator<T> GetEnumerator()
        {
            // Cache a copy of the initial version, to ensure no external changes are made while 
            // this method iterates across the data.
            int initialVersion = version;

            for(int i = 0; i < count; i++)
            {
                // For each index pointing to a valid element, in the data array, adjust and 
                // store the value as an index, reading from the readIndex, and wrapping to the 
                // length of the data array.
                int index = (readIndex + i) % data.Length;

                // Yield, and return the data pointed to by the current index.
                yield return data[index];

                if(version != initialVersion)
                {
                    // If the data version has changed since the initialVersion, logged at the 
                    // start of this method, there has been external change. Break out of this 
                    // method.
                    break;
                }
            }
        }

        /// <summary>Returns an enumerator for iteration through the 
        /// <see cref="DataStructures.RingBuffer.data"/> array.</summary>
        /// <returns>The enumerator.</returns>
        /// <remarks>Provides interface functionality, but otherwise, only calls and returns 
        /// <see cref="DataStructures.RingBuffer.GetEnumerator()"/>. At each iteration, the 
        /// <see cref="DataStructures.RingBuffer.version"/> is checked against an initial value. A 
        /// change in this value reflects an external change to the 
        /// <see cref="DataStructures.RingBuffer.data"/> array, and will cancel this method.
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator()
        {
            // Run and return the value from the local GetEnumerator method.
            return GetEnumerator();
        }
            
        /// <summary>Finds the first index for a specified item, in the 
        /// <see cref="DataStructures.RingBuffer.data"/> array.</summary>
        /// <returns>The first index for the specified item, in the 
        /// <see cref="DataStructures.RingBuffer.data"/> array. Returns <c>-1</c>, if the item 
        /// can not be found.</returns>
        /// <param name="item">The item to check for, in the 
        /// <see cref="DataStructures.RingBuffer.data"/> array.</param>
        public int IndexOf(T item)
        {
            for(int i = 0; i < count; i++)
            {
                // For each valid index in the data array, determine the index, adjusted for the 
                // position of the readIndex and wrapped to the length of the data array.
                int index = (readIndex + i) % data.Length;

                if(EqualityComparer<T>.Default.Equals(data[i], item))
                {
                    // If the current index points to an element identical to that which was passed 
                    // in, return the index.
                    return i;
                }
            }

            // If we get to this point, we did not have a match. Return -1.
            return -1;
        }

        /// <summary>Inserts a specific item in to the <see cref="DataStructures.RingBuffer.data"/> 
        /// array, at a specified index.</summary>
        /// <param name="index">The index to place the new item. Note that the array will treat 
        /// <see cref="DataStructures.RingBuffer.readIndex"/> as the current <c>[0]</c> position, 
        /// and the index will be treated relative to this value.</param>
        /// <param name="item">The item to insert into the 
        /// <see cref="DataStructures.RingBuffer.data"/> array.</param>
        public void Insert(int index, T item)
        {
            // Wrap the index around the data array length, to ensure it does not fall outside of 
            // the array.
            index %= data.Length;

            if(count != data.Length)
            {
                // If the current count of valid elements, in the data array, is not equal to the 
                // size of the data array; the data array has not been filled, yet.

                for(int i = count - 1; i >= index; i--)
                {
                    // For every index, starting at the last valid index, and rolling back to 
                    // the specified index; move the data value down in the array.
                    data[i + 1] = data[i];
                }

                // Now the rest of the array has been shuffled down, place the new item in the 
                // specified element, and increment count to reflect the additional value.
                data[index] = item;
                count++;
            }
            else
            {
                // If the current count of valid elements, in the data array, is equal to the 
                // size of the data array; the data array has been filled with valid elements, 
                // and likely wraps.

                for(int i = readIndex - 1; i != readIndex; i = (i - 1) % data.Length)
                {
                    // For every index in the data array, staring at the index before the current 
                    // readIndex, incrementing backwards and wrapping around the length of the 
                    // array; move the data value down in the array.
                    data[(i + 1) % data.Length] = data[i];
                }

                // Now the rest of the array has been shuffled down, place the new item in the 
                // specified element, adjusted for the current readIndex and size of the array.
                data[(readIndex + index) % data.Length] = item;
            }
        }

        /// <summary>Reads and removes the next element pointed to by the 
        /// <see cref="DataStructures.RingBuffer.readIndex"/>.</summary>
        /// <returns>The removed element, or the default value of T, if there are no active 
        /// elements in the array.</returns>
        /// <remarks>This method calls the 
        /// <see cref="DataStructures.RingBuffer.ReadAndReplaceNext(T)"/> method, passing over the 
        /// default value for T. This method also moves the readIndex down by one. If there are 
        /// issues accessing the array, the default value for T will be returned.</remarks>
        public T ReadAndReplaceNext()
        {
            // Read and replace the next element, using the default value for the current type, 
            // returning the results.
            return ReadAndReplaceNext(default(T));
        }

        /// <summary>Reads and removes the next element pointed to by the 
        /// <see cref="DataStructures.RingBuffer.readIndex"/>.</summary>
        /// <returns>The removed element, or if there are no active 
        /// elements in the array, the passed default value.</returns>
        /// <param name="defaultValue">The value to return, if no elements can be found.</param>
        /// <remarks>This method moves the readIndex down by one. If there are 
        /// issues accessing the array, the default value for T will be returned.</remarks>
        public T ReadAndReplaceNext(T defaultValue)
        {
            // Create a temporary store, to cache the retrieved value.
            T resultingData;

            if(count == 0)
            {
                // If the current count is zero, there are no active elements in the array;
                // log the error, and set the defaultValue as our resultingData.
                Log.EmptyRingBufferError();
                resultingData = defaultValue;
            }
            else
            {
                // Else, we have active elements, store the current data pointed to by the 
                // readIndex as our resultingData, set that element to the default value for the 
                // type, and increment the readIndex. Wrap readIndex, so if it exceeds the 
                // limitations of the data array, it resets back to the start.
                resultingData = data[readIndex];
                data[readIndex] = default(T);;
                readIndex = (readIndex + 1) % data.Length;

                // Decrement the count, and increment the version number, to reflect the 
                // removal of an element.
                count--;
                version++;
            }

            // Return the derived resultingData.
            return resultingData;
        }

        /// <summary>Removes an element from the <see cref="DataStructures.RingBuffer.data"/> 
        /// array, finding it by comparison with an identical item.</summary>
        /// <returns><c>True</c>, if an item was found and removed, else returns <c>False</c>.
        /// </returns>
        /// <param name="item">The item to match for removal.</param>
        /// <remarks>This method will determine the position of the item to remove by using the 
        /// <see cref="DataStructures.RingBuffer.IndexOf(T)"/> method, and as such, will only 
        /// identify the first instance of the passed in item. Once the index has been determined, 
        /// this method will use the <see cref="DataStructures.RingBuffer.RemoveAt(int)"/> method 
        /// to perform the removal.</remarks> 
        public bool Remove(T item)
        {
            // Find the first index at which the passed in item is found, in the data array.
            int index = IndexOf(item);

            if(index == -1)
            {
                // If the derived index is -1, the item does not exist in the data array;
                // return false to flag the method as unsuccessful.
                return false;
            }

            // If we get to this point, we have found a valid item; remove an item from the data 
            // array, by the derived index, and return true to flag the method as successful.
            RemoveAt(index);
            return true;
        }
            
        /// <summary>Removes an element from the <see cref="DataStructures.RingBuffer.data"/> 
        /// array, at the specified position.</summary>
        /// <param name="index">The index for the element to be removed.</param>
        public void RemoveAt(int index)
        {
            // Wrap the index around the count value, to ensure it points to a valid element.
            index %= count;

            // Determine the first index to replace, by adjusting the determined index by the 
            // readIndex, and wrapping it around the data array length.
            int toIndex = (readIndex + index) % data.Length;

            for(int i = index; i < count; i++)
            {
                // For each valid element in the data array, determine the element to move by 
                // incrementing the toIndex, and wrapping to the data array length. Store the value 
                // of the "fromIndex" element in the "toIndex" element, and set the fromIndex as 
                // the new toIndex, as we shuffle through the array.
                int fromIndex = (toIndex + 1) % data.Length;
                data[toIndex] = data[fromIndex];
                toIndex = fromIndex;
            }

            // Set the final "toIndex" element as the default value of the type.
            data[toIndex] = default(T);
        }
    }
}

namespace DataStructures.Utility
{
    // Provides debug functionality, including methods and customised string messages.
    public static partial class DataStructuresDebug
    {
        private const string emptyRingBufferError = "Error: Trying to access element in an empty "
            + "RingBuffer array.";
        private const string ringBuggerCopyToPassedNullArrayError = "Error: RingBuffer CopyTo "
            + "method has been passed a null array.";
        private const string ringBufferCopyToPassedOutOfRangeIndexError = "Error: RingBuffer "
            + "CopyTo method has been passed an index outside of the length of the included array. "
            + "Index: {0}; Array Length: {1}.";
        private const string ringBufferCopyToArrayTooSmallError = "Error: RingBuffer CopyTo method "
            + "passed an array to small to fit internal data. Attempting to copy to an array of "
            + "length {0}, starting at index {1}. This only leaves a length of {2} to store an "
            + "array of length {3}.";
        private const string ringBufferEnumeratorWorkingOnOldVersionError = "Error: IENumerator "
            + "is working on version {0} or RingBuffer, but RingBuffer has now been changed to "
            + "version {1}. Operation can not continue.";

        public static void EmptyRingBufferError()
        {
            UnityEngine.Debug.LogError(emptyRingBufferError);
        }

        public static void RingBufferCopyToPassedNullArrayError()
        {
            UnityEngine.Debug.LogError(ringBuggerCopyToPassedNullArrayError);
        }

        public static void RingBufferCopyToPassedOutOfRangeIndexError(int index, int arrayLength)
        {
            UnityEngine.Debug.LogError(string.Format(ringBufferCopyToPassedOutOfRangeIndexError, 
                index, arrayLength));
        }

        public static void RingBufferCopyToArrayTooSmallError(int arrayLength, int startingIndex, 
            int dataArrayLength)
        {
            UnityEngine.Debug.LogError(string.Format(ringBufferCopyToArrayTooSmallError, 
                arrayLength, startingIndex, (arrayLength - (startingIndex + 1)), dataArrayLength));
        }

        public static void RingBufferEnumeratorWorkingOnOldVersionError(int oldVersion, 
            int newVersion)
        {
            UnityEngine.Debug.Log(string.Format(ringBufferEnumeratorWorkingOnOldVersionError, 
                oldVersion, newVersion));
        }
    }
}