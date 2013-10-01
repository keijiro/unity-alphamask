using UnityEngine;
using UnityEditor;
using System.Collections;

class TextureConfigurator : AssetPostprocessor
{
    void OnPreprocessTexture ()
    {
        var importer = (assetImporter as TextureImporter);

        if (assetPath.EndsWith ("Default.png")) {
            importer.textureType = TextureImporterType.GUI;
            importer.textureFormat = TextureImporterFormat.AutomaticCompressed;
            importer.compressionQuality = (int)TextureCompressionQuality.Best;
        }

        if (assetPath.EndsWith ("Truecolor.png")) {
            importer.textureType = TextureImporterType.GUI;
            importer.textureFormat = TextureImporterFormat.RGBA32;
        }

        if (assetPath.EndsWith ("Base.png")) {
            importer.textureType = TextureImporterType.GUI;
            importer.textureFormat = TextureImporterFormat.AutomaticCompressed;
            importer.compressionQuality = (int)TextureCompressionQuality.Best;
        }

        if (assetPath.EndsWith ("Mask.png")) {
            importer.textureType = TextureImporterType.GUI;
            importer.textureFormat = TextureImporterFormat.AutomaticCompressed;
            importer.compressionQuality = (int)TextureCompressionQuality.Best;
        }
    }
}