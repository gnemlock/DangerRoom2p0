/* Created by Matthew Francis Keating */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using UnityEditor;

/// <summary>Inserts namespace usage in a script, based off folder location.</summary>
/// <remarks></remarks>
public class InsertNamespace : UnityEditor.AssetModificationProcessor
{
    private static readonly string namespacePathUsageDirective = "#NAMESPACE#";
    private static readonly string namespaceNameUsageDirective = "#namespace#";
    private static readonly string baseNamespace = "";
    private static readonly string[] baseDirectory = { "Assets", "Script" };

    /// <summary>This method will be called whenever an asset is created from the editor.</summary>
    /// <param name="path">The asset path of the created asset.</param>
    public static void OnWillCreateAsset(string path)
    {
        // Store the direct asset path with any meta extension omitted.
        string assetPath = Regex.Replace(path, @".meta$", string.Empty);

        if(!assetPath.EndsWith(".cs"))
        {
            // If the assetPath does not end with ".cs", this is not a C# script;
            // exit from this method.
            return;
        }

        // Create a string array containing each directory, in the asset path.
        string[] splitAssetPath = assetPath.Split('/');
		string namespacePath;
        string namespaceName;

        if(!CompareAgainstBaseDirectory(splitAssetPath))
        {
            namespacePath = "";
            namespaceName = "";
        }
        else
        {
            namespacePath = ConvertToNamespacePath(splitAssetPath);
            namespaceName = namespacePath.Replace(".", "");
        }

        // Create a placeholder, and store the script as a single string.
        string finalCode = string.Join("\n", File.ReadAllLines(assetPath));

        finalCode = finalCode.Replace(namespacePathUsageDirective, namespacePath);
        finalCode = finalCode.Replace(namespaceNameUsageDirective, namespaceName);

        // Write the final code back to the initial asset path, and refresh the asset database.
        File.WriteAllText(assetPath, finalCode);
        AssetDatabase.Refresh();
    }

    private static bool CompareAgainstBaseDirectory(string[] splitPath)
    {
        if(splitPath.Count() < baseDirectory.Count())
        {
            return false;
        }
        else
        {
            for(int i = 0; i < baseDirectory.Count(); i++)
            {
                if(splitPath[i] != baseDirectory[i])
                {
                    return false;
                }
            }

            return true;
        }
    }

    private static string ConvertToNamespacePath(string[] splitPath)
    {
        // Since we do not want to include the base directory, set the starting index to the 
        // baseDirectory count. Since we have already established that the splitPath starts with 
        // the baseDirectory, this should completely omit the baseDirectory elements.
        int startingPathIndex = baseDirectory.Count();

        // Create a string to store the final namespace. If we are using a baseNamespace, 
        // start the namespacePath by inserting the initial baseNamespace.
        string namespacePath = (baseNamespace == "" ? "" : (baseNamespace + "."));

        for(int i = startingPathIndex; i < splitPath.Count() - 1; i++)
        {
            // For each element in the splitPath, starting from our startingPathIndex, and omitting 
            // the final element (which would be the actual script file name, not a directory);
            // add the next splitPath element to the namespacePath.
            namespacePath += splitPath[i] + ".";
        }

        // We have added a final '.' that should not be included; remove the final character to 
        // omit the additional '.', and return the results.
        return namespacePath.Remove(namespacePath.Length - 1);
    }
}