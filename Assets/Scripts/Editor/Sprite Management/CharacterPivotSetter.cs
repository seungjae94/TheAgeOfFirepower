using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;

#if UNITY_EDITOR
public class CharacterPivotSetter : MonoBehaviour
{
    [MenuItem("Sprite/Set pivots for selected character sprite")]
    private static void Menu()
    {
        Vector2 characterPivot = new Vector2(0.5f, 0.25f);

        Texture2D[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);
        
        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            var factory = new SpriteDataProviderFactories();
            factory.Init();

            var spriteEditorDataProvider = factory.GetSpriteEditorDataProviderFromObject(textureImporter);
            spriteEditorDataProvider.InitSpriteEditorDataProvider();

            var spriteRects = spriteEditorDataProvider.GetSpriteRects();

            Debug.Log(spriteRects.Length);

            for (int i = 0; i < spriteRects.Length; ++i)
            {
                spriteRects[i].alignment = SpriteAlignment.Custom;
                spriteRects[i].pivot = characterPivot;
            }
            spriteEditorDataProvider.SetSpriteRects(spriteRects);
            spriteEditorDataProvider.Apply();
        }

        Debug.Log("[Sprite Action] character sprite pivot setting done!");
    }
}

#endif