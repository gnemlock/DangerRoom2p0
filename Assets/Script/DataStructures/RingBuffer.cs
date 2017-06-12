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

        public RingBuffer(int size)
        {
            data = new T[size];
            count = 0;
            readIndex = 0;
            writeIndex = 0;
        }

        public T ReadAndRemoveNext()
        {
            return ReadAndRemoveNext(default(T));
        }

        public T ReadAndRemoveNext(T defaultValue)
        {
            T resultingData;

            if(count == 0)
            {
                Log.EmptyRingBufferError();
                resultingData = defaultValue;
            }
            else
            {
                T temporaryData = data[readIndex];
                data[readIndex] = default(T);
                readIndex = (readIndex + 1) % data.Length;

                count--;
                version++;

                resultingData = temporaryData;
            }

            return resultingData;
        }

        public void Add(T newData)
        {
            data[writeIndex] = newData;
            writeIndex = (writeIndex + 1) % data.Length;

            if(count == data.Length)
            {
                readIndex =- (readIndex + 1) % data.Length;
            }
            else
            {
                count++;
            }

            version++;
        }

        public void Clear()
        {
            for(int i = 0; i < data.Length; i++)
            {
                data[i] = default(T);
            }

            readIndex = 0;
            writeIndex = 0;
            count = 0;
            version++;
        }

        public bool Contains(T item)
        {
            for(int i = 0; i < data.Length; i++)
            {
                if(EqualityComparer<T>.Default.Equals(item, data[i]))
                {
                    return true;
                }
            }

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

        public IEnumerator<T> GetEnumerator()
        {
            int initialVersion = version;

            for(int i = 0; i < count; i++)
            {
                int index = (readIndex + i) % data.Length;

                yield return data[index];

                if(version != initialVersion)
                {
                    
                    break;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            for(int i = 0; i < count; i++)
            {
                int index = (readIndex + i) % data.Length;
                if(EqualityComparer<T>.Default.Equals(data[i], item))
                {
                    return i;
                }
            }

            return -1;
        }

        public void RemoveAt(int index)
        {
            index %= count;

            int toIndex = (readIndex + index) % data.Length;

            for(int i = index; i < count; i++)
            {
                int fromIndex = (toIndex + 1) % data.Length;
                data[toIndex] = data[fromIndex];
                toIndex = fromIndex;
            }

            data[toIndex] = default(T);
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);

            if(index == -1)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        public void Insert(int index, T item)
        {
            if(count != data.Length)
            {
                for(int i = count - 1; i > readIndex; i--)
                {
                    data[i + 1] = data[i];
                }

                count++;
            }
            else
            {
                for(int i = readIndex - 1; i != readIndex; i = (i - 1) % data.Length)
                {
                    data[(i + 1) % data.Length] = data[i];
                }

                data[(readIndex + index) % data.Length] = item;
            }
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