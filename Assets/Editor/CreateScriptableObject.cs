using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

namespace ArrowCardGame
{
    public class CreateScriptableObject
    {
        [MenuItem("Assets/Create/ArrowCardGame/Card")]
        public static void CreatePattern()
        {
            CreateAsset<CardDefinition>();
        }

        [MenuItem("Assets/Create/ArrowCardGame/Deck")]
        public static void CreateBulletDefinition()
        {
            CreateAsset<DeckDefinition>();
        }

        public static void CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}