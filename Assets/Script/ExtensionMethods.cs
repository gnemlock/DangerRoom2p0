using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ExtensionMethods 
{
    // Used in: Drawing
    /// <summary>Prepares an object for change, so it may be undone, and recognised for the 
    /// purpose of alerting the user to unsaved changes.</summary>
    /// <param name="editor">This method is part of the <see cref="Editor"/> class.</param>
    /// <param name="changingObject">The object being changed.</param>
    /// <param name="actionName">Action name to use when listing the change in the log.</param>
    public static void PrepareChange(this Editor editor, Object changingObject, 
        string actionName = "")
    {
        if(actionName == "")
        {
            // If we have passed in a default action name, use the derived target's ToString() 
            // method to construct a more helpful name.
            actionName = "Change to " + editor.target.ToString();
        }
        
        // Record the current object, using the desired description, and mark it as dirty.
        Undo.RecordObject(changingObject, actionName);
        EditorUtility.SetDirty(changingObject);
    }
}
