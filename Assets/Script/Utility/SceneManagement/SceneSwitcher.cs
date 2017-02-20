using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utility.SceneManagement
{
    using UnityEngine.SceneManagement;
    
    public class SceneSwitcher : MonoBehaviour 
    {
        public void SwitchScene()
        {
            int nextLevel = (SceneManager.GetActiveScene().buildIndex + 1)
                % SceneManager.sceneCount;
            
            SceneManager.LoadScene(nextLevel);
        }
        
        public void SwitchScene(int sceneIndex)
        {
            int nextLevel = sceneIndex % SceneManager.sceneCount;
            
            SceneManager.LoadScene(sceneIndex);
        }
    }
}

namespace Utility.SceneManagement.Utility
{
    public static class SceneManagementLabels
    {
        #if UNITY_EDITOR
        public const string switchScenes = "Switch Scenes";
        #endif
    }
    [CustomEditor(typeof(SceneSwitcher))] public class SceneSwitcherEditor : Editor
    {
        #if UNITY_EDITOR
        public override void OnInspectorGUI()
        {
            SceneSwitcher sceneSwitcher = target as SceneSwitcher;
            
            if(GUILayout.Button(SceneManagementLabels.switchScenes))
            {
                sceneSwitcher.SwitchScene();
            }
        }
        #endif
    }
}