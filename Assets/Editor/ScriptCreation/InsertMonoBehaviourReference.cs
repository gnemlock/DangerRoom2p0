/* Created by Matthew Francis Keating */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using UnityEditor;

/// <summary> </summary>
public class InsertMonoBehaviourReference : UnityEditor.AssetModificationProcessor
{
    private static readonly string monoBehaviourNameDirective = "#scriptname#";

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

        string[] splitAssetPath = assetPath.Split('/');
        string[] splitFileName = splitAssetPath[splitAssetPath.Count() - 1].Split('.');
        string monoBehaviourName = splitFileName[0];

        monoBehaviourName 
            = char.ToLowerInvariant(monoBehaviourName[0]) + monoBehaviourName.Substring(1);

        // Create a placeholder, and store each line in the script, as a list.
        string finalCode = string.Join("\n", File.ReadAllLines(assetPath));

        finalCode = finalCode.Replace(monoBehaviourNameDirective, monoBehaviourName);

        // Write the final code back to the initial asset path, and refresh the asset database.
        File.WriteAllText(assetPath, finalCode);
        AssetDatabase.Refresh();
    }
}