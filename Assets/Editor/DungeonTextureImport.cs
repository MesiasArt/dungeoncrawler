using UnityEditor;
using UnityEngine;

// Explicit repair command: never reimport assets during a domain reload.
static class DungeonTextureImport
{
    static readonly string[] Names = { "textura azul luffy2", "textura rosa luffy2" };

    [MenuItem("Tools/Dungeon/Reimportar texturas")]
    static void EnsureTextures()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode ||
            EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        foreach (string name in Names)
        {
            string resourcePath = "Textures/" + name;
            string assetPath = "Assets/Resources/" + resourcePath + ".jpg";
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                Debug.LogError("No se pudo obtener el importador de " + assetPath);
                continue;
            }
            if (Resources.Load<Texture2D>(resourcePath) != null &&
                importer.textureType == TextureImporterType.Default &&
                importer.textureShape == TextureImporterShape.Texture2D) continue;

            importer.textureType = TextureImporterType.Default;
            importer.textureShape = TextureImporterShape.Texture2D;
            importer.sRGBTexture = true;
            importer.mipmapEnabled = true;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();

            if (Resources.Load<Texture2D>(resourcePath) == null)
                Debug.LogError("La textura sigue sin estar disponible tras reimportar: " + assetPath);
            else
                Debug.Log("Textura del dungeon reparada: " + assetPath);
        }
    }
}
