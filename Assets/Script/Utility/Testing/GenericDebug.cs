using UnityEngine;

namespace Utility.Testing
{
    using Log = Utility.TestingDebug;

    public class GenericDebug 
    {
        public static void SendArrayToDebug<T>(T[] array)
        {
            string debugOutput = "";
            int lastIndex = array.Length - 1;

            for(int i = 0; i < lastIndex; i++)
            {
                debugOutput += array[i].ToString() + Log.seperator;
            }

            debugOutput += array[lastIndex].ToString();
            Debug.Log(debugOutput);
        }
    }
}

namespace Utility.Testing.Utility
{
    public static partial class TestingDebug
    {
        public const string seperator = ", ";
    }
}