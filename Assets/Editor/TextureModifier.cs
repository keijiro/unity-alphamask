using UnityEngine;
using UnityEditor;
using System.Collections;

class TextureModifier : AssetPostprocessor
{
    const string suffix = "with alpha.png";

    // Chooses a proper compression format.
    static TextureFormat CompressionFormat {
        get {
            switch (EditorUserBuildSettings.activeBuildTarget) {
            case BuildTarget.Android:
                return TextureFormat.ETC_RGB4;
            case BuildTarget.iPhone:
                return TextureFormat.PVRTC_RGB4;
            default:
                return TextureFormat.DXT1;
            }
        }
    }

    void OnPreprocessTexture ()
    {
        var importer = (assetImporter as TextureImporter);

        importer.textureType = TextureImporterType.GUI;

        if (assetPath.EndsWith (suffix)) {
            importer.textureFormat = TextureImporterFormat.RGBA32;
        }
    }

    void OnPostprocessTexture (Texture2D texture)
    {
        if (!assetPath.EndsWith (suffix)) {
            return;
        }

        // Make a mask texture temporary in true color.
        var mask = new Texture2D (texture.width, texture.height, TextureFormat.RGB24, false);
        mask.wrapMode = TextureWrapMode.Clamp;

        // Convert the source image into a mask.
        var pixels = texture.GetPixels ();
        for (int i = 0; i < pixels.Length; i++) {
            var a = pixels [i].a;
            pixels [i] = new Color (a, a, a);
        }
        mask.SetPixels (pixels);

        // Compress the mask in the proper compression format.
        EditorUtility.CompressTexture (mask, CompressionFormat, TextureCompressionQuality.Best);

        // Replace the asset which already exists, or create a new asset.
        var maskPath = assetPath.Replace (suffix, "mask.asset");
        var maskAsset = AssetDatabase.LoadAssetAtPath (maskPath, typeof(Texture2D)) as Texture2D;
        if (maskAsset == null) {
            AssetDatabase.CreateAsset (mask, maskPath);
        } else {
            EditorUtility.CopySerialized (mask, maskAsset);
        }

        // Compress the source texture.
        EditorUtility.CompressTexture (texture, CompressionFormat, TextureCompressionQuality.Best);
    }
}