using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Needle;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

public class AssetChecks
{
    internal static string SamplePackage => Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(samplePackageJsonGuid));
    internal static string EnginePackage => Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(enginePackageJsonGuid));

    public class Samples
    {
        [Test]
        public void SampleModelsUseUnityGltfImporter() => ModelsUseUnityGltfImporter(SamplePackage);

        [Test]
        public void SampleModelsUseAllowedMaterials() => MaterialsUseAllowedShaders(SamplePackage);
            
        [Test]
        public void SampleShadersAreCorrect() => ShadersUseCorrectTargets(SamplePackage);

        [Test]
        public void NoEmptyFolders() => NoEmptyFoldersFound(SamplePackage);
        
        [Test]
        public void NoExternalDependencies() => NoExternalDependenciesFound(SamplePackage);
        
        [Test]
        public void SampleCubemapsAreHDR() => CubemapsAreHDR(SamplePackage);
    }

    public class Engine
    {
        [Test]
        public void EngineModelsUseUnityGltfImporter() => ModelsUseUnityGltfImporter(EnginePackage);

        [Test]
        public void EngineModelsUseAllowedMaterials() => MaterialsUseAllowedShaders(EnginePackage);

        [Test]
        public void EngineShadersAreCorrect() => ShadersUseCorrectTargets(EnginePackage);

        [Test]
        public void NoEmptyFolders() => NoEmptyFoldersFound(EnginePackage);
        
        [Test]
        public void NoExternalDependencies() => NoExternalDependenciesFound(EnginePackage);
        
        [Test]
        public void EngineCubemapsAreHDR() => CubemapsAreHDR(EnginePackage);
    }
    
    private const string samplePackageJsonGuid = "fd17907bb2ad1444d9c584fde3e7715b";
    private const string enginePackageJsonGuid = "041e32dc0df5f4641b30907afb5926e6";

    private static void GetFolders(string path, List<DirectoryInfo> results)
    {
        var di = new DirectoryInfo(path);
        if (!di.Exists)
            return;
            
        results.Add(di);
            
        try {
            foreach (var directory in di.GetDirectories())
            {
                if (SampleChecks.SampleChecks.ignoreSizeFolderNames.Any(ignoredFolder => directory.FullName.Contains(ignoredFolder)))
                    continue;
                
                GetFolders(directory.FullName, results);
            }
        } catch {
            // ignored
        }
    }
    
    private static void NoEmptyFoldersFound(string assetFolder)
    {
        var folders = new List<DirectoryInfo>();
        GetFolders(assetFolder, folders);
        var emptyFolders = new List<DirectoryInfo>();
        
        foreach (var folder in folders)
        {
            try
            {
                var files = folder.GetFiles();
                var directories = folder.GetDirectories();
                
                var fileCount = files.Length;
                // Mac-specific; hidden file
                if (fileCount == 1 && files[0].Name == ".DS_Store")
                    fileCount = 0;
                var directoryCount = directories.Length;
                // directories that end with ~ don't count, we should not have folders with just that
                for (int i = 0; i < directories.Length; i++)
                    if (directories[i].Name.EndsWith("~", StringComparison.Ordinal))
                        directoryCount--;
                // node_modules shouldn't count either, we don't want to ship those
                if (directoryCount == 1 && directories[0].Name == "node_modules")
                    directoryCount = 0;
                
                if (fileCount == 0 && directoryCount == 0)
                    emptyFolders.Add(folder);
            } catch {
                // ignored
            }
        }

        if (emptyFolders.Count <= 0) return;
        
        var sb = new StringBuilder();
        sb.AppendLine("Empty folders found:");
        foreach (var emptyFolder in emptyFolders)
            sb.AppendLine(emptyFolder.FullName);
        Assert.Fail(sb.ToString());
    }
    
    private static void NoExternalDependenciesFound(string assetFolder, params string[] additionalAllowedPackageNames)
    {
        var allowedExternalDependencies = new List<string>()
        { 
            "Packages/com.unity.render-pipelines.universal/Runtime/UniversalAdditionalCameraData.cs",
            "Packages/com.unity.render-pipelines.universal/Runtime/UniversalAdditionalLightData.cs",
            "Packages/com.unity.ugui/Runtime/EventSystem/EventSystem.cs",
            "Packages/com.unity.render-pipelines.universal/Shaders/Utils/ScreenSpaceAmbientOcclusion.shader",
            
            // TODO we should patch this reference out in SampleUpdater.cs, it will be missing in User projects
            "Packages/com.needle.dev-assets/Needle Samples FTP.asset",
            
            // TODO check if we actually want to allow these
            "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/UniversalMetadata.cs",
            "Packages/com.unity.render-pipelines.universal/Editor/AssetVersion.cs",
        };
        
        // also allowed: references to UniversalRenderPipelineAsset and everything that this depends on
        var sampleUrpAssetGuid = "9c0e5f17d9e250e45814ec5de17e094f";
        var urpAssetPath = AssetDatabase.GUIDToAssetPath(sampleUrpAssetGuid);
        var allowedUrpDependencies = AssetDatabase.GetDependencies(urpAssetPath, true);
        allowedExternalDependencies.AddRange(allowedUrpDependencies);
        allowedExternalDependencies.Add(AssetDatabase.GUIDToAssetPath("60e1133243b97e347b653163a8c01b64")); // XRSystemData.asset
        var extraFolders = new string[]
        {
            "Packages/com.unity.render-pipelines.universal/Runtime/Overrides/",
            "Packages/com.unity.render-pipelines.universal/Runtime/RendererFeatures/",
        };
        var extraFoldersContent = AssetDatabase.FindAssets("", extraFolders).Select(AssetDatabase.GUIDToAssetPath);
        allowedExternalDependencies.AddRange(extraFoldersContent);
        
        var allAssetsInFolder = AssetDatabase
            .FindAssets("", new[] { assetFolder })
            .Select(AssetDatabase.GUIDToAssetPath)
            // Exclude FBX models, those automatically import materials in the right RP,
            // as long as they don't have an importer override
            .Where(x => !(AssetImporter.GetAtPath(x) is ModelImporter modelImporter))
            .ToArray();

        bool HasInvalidDependencies(bool recursive, bool logInfo, params string[] allAssetsInFolder)
        {
            // replace path for a scenetemplate with the embeding scene to avoid a default material dependency
            // which can be a URP Lit material causing the test to fail
            for (int i = 0; i < allAssetsInFolder.Length; i++)
            {
                var assetPath = allAssetsInFolder[i];
                
                if (assetPath.EndsWith(".scenetemplate"))
                {
                    var sceneTemplate = AssetDatabase.LoadAssetAtPath<SceneTemplateAsset>(assetPath);
                    allAssetsInFolder[i] = AssetDatabase.GetAssetPath(sceneTemplate.templateScene);
                }                
            }

            var dependencies = AssetDatabase.GetDependencies(allAssetsInFolder, recursive);
            var packages = new Dictionary<string, List<string>>();
            foreach (var dependency in dependencies)
            {
                var pi = PackageInfo.FindForAssetPath(dependency);
                var packageKey = pi == null ? "Assets" : pi.name;
                if (!packages.ContainsKey(packageKey))
                    packages.Add(packageKey, new List<string>());
                
                // some files are ok – e.g. we can't avoid URPAdditionalLightData and URPAdditionalCameraData
                if (allowedExternalDependencies.Contains(dependency)) continue;
                
                packages[packageKey].Add(dependency);
            }

            var allowedPackageNames = new List<string>()
            {
                // Direct Needle Engine dependencies
                "com.needle.engine-samples",
                "com.needle.engine-exporter",
                "com.unity.shadergraph",
                "org.khronos.unitygltf",
                "com.unity.timeline",
                // Indirect Needle Engine dependencies
                "com.unity.render-pipelines.core",
                // Not a dependency but basically always in projects
                "com.unity.ugui",
            };
            allowedPackageNames.AddRange(additionalAllowedPackageNames);

            var anyNotAllowedDependenciesFound = false;
            foreach (var kvp in packages)
            {
                if (allowedPackageNames.Contains(kvp.Key))
                {
                    if (logInfo) Debug.Log($"Found {kvp.Value.Count} dependencies in {kvp.Key} <color=#0f0>(allowed)</color>");
                    continue;
                }
                kvp.Value.Sort();
                if (kvp.Value.Count == 0)
                {
                    if (logInfo) Debug.Log($"Found dependencies in {kvp.Key} <color=#0f0>(but all of them are allowed)</color>");
                }
                else
                {
                    anyNotAllowedDependenciesFound = true;
                    Debug.Log($"Found {kvp.Value.Count} dependencies in {kvp.Key} <color=#f00>(NOT ALLOWED)</color>:\n - {string.Join("\n - ", kvp.Value)}");
                }
            }
            return anyNotAllowedDependenciesFound;
        }

        var anyNotAllowedDependenciesFound = false; 
        
        // Can't check all paths at once since then we don't know where issues come from.
        // HasInvalidDependencies(true, true, allAssetsInFolder);
        
        // Best thing we can do here is iterate manually and call
        // GetDependencies on each asset to figure out where the problem is
        foreach (var assetPath in allAssetsInFolder)
        {
            var hasInvalidDependencies = HasInvalidDependencies(false, false, assetPath);
            if (hasInvalidDependencies)
                Debug.LogError($"Found external dependencies in {assetPath}.");
            anyNotAllowedDependenciesFound |= hasInvalidDependencies;
        }
        
        if (anyNotAllowedDependenciesFound)
            Assert.Fail("Found external dependencies, please see the logs for more details.");
    }
    
    private static readonly string[] AllowedShaderNames = new string[]
    {
        "UnityGLTF/",
        "Hidden/UnityGLTF/",
        "Skybox/",
        "GUI/Text Shader",
        "Needle/Shadow Catcher",
        "VR/SpatialMapping/Occlusion",
    };

    private static readonly string[] AllowedMaterialGuids = new string[]
    {
        "e653836c30661fe419b8992e230ca189", // glow_add particle material - no good way to make a cross-platform additive material right now
        "0252d1a9babfab041b028d802c7ee9db", // also glow_add
    };

    private static bool MaterialIsAllowed(Material material)
    {
        if (!material || !material.shader) return false;
        
        // check if material is ready and properly compiled
        if (!material.shader.isSupported)
        {
            Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, material, "{0}", "Material is not supported: " + material.name);
            return false;
        }

        // these are fine - they are known to export correctly.
        if (AllowedShaderNames.Any(shaderName => material.shader.name.StartsWith(shaderName)))
            return true;
        
        // some exceptions: these have cross-platform issues
        var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(material));
        if (AllowedMaterialGuids.Contains(guid))
        {
            Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, material, "{0}", "Material is allowed but known to be problematic: " + material.name);
            return true;
        }

        // sub assets can pass; let's hope the respective importer can properly import this for different pipelines
        if (AssetDatabase.IsSubAsset(material)) 
            return true;
        
        // ShaderGraph is only allowed for custom conversions, other cases would need to be explicitly allowed
        var shaderAssetPath = AssetDatabase.GetAssetPath(material.shader);
        if (shaderAssetPath.EndsWith(".shadergraph"))
            return AssetDatabase.GetLabels(AssetImporter.GetAtPath(shaderAssetPath)).Contains("ExportShader");

        return false;
    }
    
    private static void ModelsUseUnityGltfImporter(string assetFolder)
    {
        // check if we have multiple valid glTF importers
        // load all model files in the folder where the asset for GUID packageJsonGuid is located
        // load via AssetDatabase.Find
        var modelFiles = AssetDatabase.FindAssets("t:GameObject", new[] { assetFolder });
        var errors = new List<(string, Object)>();

        // iterate over model files and check if they use the correct importer (UnityGltf)
        foreach (var modelFile in modelFiles)
        {
            var modelPath = AssetDatabase.GUIDToAssetPath(modelFile);
            var importer = AssetImporter.GetAtPath(modelPath);
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            
#if UNITY_2022_1_OR_NEWER
            var availableImporters = AssetDatabase.GetAvailableImporters(importer.assetPath);
#else
            var availableImporters = AssetDatabase.GetAvailableImporterTypes(importer.assetPath);
#endif
            if (availableImporters.Contains(typeof(UnityGLTF.GLTFImporter)))
            {
                var importerOverride = AssetDatabase.GetImporterOverride(importer.assetPath);
                if (importerOverride == null || importerOverride != typeof(UnityGLTF.GLTFImporter))
                {
#if UNITY_2022_1_OR_NEWER
                    if (importerOverride == null) continue; // Bug in 2022.1+: doesn't actually return the specified override, instead always returns null
#endif
                    errors.Add(($"Model {Path.GetFileName(modelPath)} uses the wrong importer, should use " + typeof(UnityGLTF.GLTFImporter) + ". Uses: " + importer.GetType() + ". Override: " + importerOverride, asset));
                }
            }
            // Debug.Log(Path.GetFileName(importer.assetPath) + ", " + importer, importer);
        }

        if (errors.Any())
        {
            foreach (var e in errors)
                Debug.LogError(e.Item1, e.Item2);
            Assert.Fail($"Found {errors.Count} importers with invalid importer settings");
        }
    }
    
    [MenuItem("Tools/Needle/Log Invalid Materials")]
    private static void LogInvalidDependencies()
    {
        var scenePath = SceneManager.GetActiveScene().path;
        var materialFiles = new Dictionary<Material, List<string>>();
        GatherDependenciesOfType(scenePath, materialFiles);
        var errors = CollectMaterialErrors(materialFiles);
        var errorMaterials = errors.Select(x => x.Item2).ToArray();
        var sceneRenderers = Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var sceneRenderersWithErrors = sceneRenderers.Where(x => x.sharedMaterials.Any(errorMaterials.Contains)).ToArray();
        foreach (var e in  sceneRenderersWithErrors)
            Debug.LogError($"Renderer {e.name} has invalid materials", e);
    }

    private static void GatherDependenciesOfType<T>(string scenePath, Dictionary<T, List<string>> materialToSource)
    {
        var dependencyMaterials = AssetDatabase
            .GetDependencies(scenePath)
            .Where(x => AssetDatabase.GetMainAssetTypeAtPath(x) != typeof(SceneAsset))
            .SelectMany(AssetDatabase.LoadAllAssetsAtPath)
            .Where(x => x is T)
            .Cast<T>()
            .ToList();
            
        foreach (var material in dependencyMaterials)
        {
            if (!materialToSource.ContainsKey(material))
                materialToSource.Add(material, new List<string>());
            materialToSource[material].Add(scenePath);
        }
    }
    
    private static List<(string, Object)> CollectMaterialErrors(Dictionary<Material, List<string>> materialFiles)
    {
        var errors = new List<(string, Object)>();
        foreach (var kvp in materialFiles)
        {
            var material = kvp.Key;
            var path = AssetDatabase.GetAssetPath(material);
            var guid = AssetDatabase.AssetPathToGUID(path);
            var isSubAsset = AssetDatabase.IsSubAsset(material);

            if (!MaterialIsAllowed(material))
            {
                errors.Add(($"{Path.GetFileName(path)}, subasset: {isSubAsset}, shader: {material.shader}, guid: {guid}" +
                            $"\nReferenced from:\n - {string.Join("\n - ", kvp.Value)}", material));
            }
        }
        return errors;
    }
    
    private static void MaterialsUseAllowedShaders(string assetFolder)
    {
        // find all materials in the same folder
        var materialFiles = AssetDatabase
            .FindAssets("t:Material", new[] { assetFolder })
            .Select(AssetDatabase.GUIDToAssetPath)
            .SelectMany(AssetDatabase.LoadAllAssetsAtPath)
            .Where(x => x is Material)
            .Cast<Material>()
            .Distinct()
            .ToDictionary(x => x, x => new List<string>() { AssetDatabase.GetAssetPath(x) });
        
        // find scenes inside that folder
        var scenePaths = AssetDatabase
            .FindAssets("t:Scene", new[] { assetFolder })
            .Select(AssetDatabase.GUIDToAssetPath)
            .ToArray();

        foreach (var scenePath in scenePaths)
        {
            // we're doing that scene-by-scene so that we can log better errors what comes from where
            GatherDependenciesOfType(scenePath, materialFiles);
        }
        
        var errors = CollectMaterialErrors(materialFiles);

        if (errors.Any())
        {
            Debug.LogError($"Found {errors.Count} materials that are now allowed. See the logs below for more info. Open the affected scenes and run \"Tools/Needle/Log Invalid Materials\" to find renderers referencing invalid materials.");
            foreach (var e in errors)
                Debug.LogError(e.Item1, e.Item2);
            Assert.Fail($"Found {errors.Count} materials that are now allowed.");
        }
    }

    private static void ShadersUseCorrectTargets(string assetFolder)
    {
        var shaderFiles = AssetDatabase.FindAssets("t:Shader", new[] { assetFolder });
        var problems = new List<string>();
        Debug.Log($"Checking {shaderFiles.Length} shaders in {assetFolder}");
        foreach (var shaderFile in shaderFiles)
        {
            var shaderPath = AssetDatabase.GUIDToAssetPath(shaderFile);
            var shaderName = Path.GetFileNameWithoutExtension(shaderPath);
            if (!shaderPath.EndsWith(".shadergraph")) continue;
            
            var supportedTargets = ShaderGraphInternals.GetTargetsFromGraph(shaderPath);

            var hasAnyProblems = false;
            if (supportedTargets.BiRP != ShaderGraphInternals.TargetsInfo.TargetType.Unlit)
            {
                hasAnyProblems = true;
                problems.Add($"[BiRP] Shader \"{shaderName}\"\n    Expected: {ShaderGraphInternals.TargetsInfo.TargetType.Unlit}    Actual: {supportedTargets.BiRP}");
            }

            if (supportedTargets.URP != ShaderGraphInternals.TargetsInfo.TargetType.Unlit)
            {
                hasAnyProblems = true;
                problems.Add($"[URP]  Shader \"{shaderName}\"\n    Expected: {ShaderGraphInternals.TargetsInfo.TargetType.Unlit}    Actual: {supportedTargets.URP}");
            }
           
            if (hasAnyProblems)
                Debug.Log("Shader isn't configured correctly: " + shaderPath, AssetDatabase.LoadAssetAtPath<Shader>(shaderPath));
            
        }
        Assert.AreEqual(0, problems.Count, string.Join("\n", problems) + "\n\nProblems:");
    }

    private static string[] allowedNonHdrAssets = new string[]
    {
        "f7ef59c8610b47f45b9faee4a7e5ffa3", // JapanesePanorama (no EXR version available)
    };
    
    private static void CubemapsAreHDR(string assetFolder)
    {
        var cubeMaps = AssetDatabase.FindAssets("t:Cubemap", new[] { assetFolder });
        var errors = new List<(string, Object)>();
        foreach (var cubeMap in cubeMaps)
        {
            if (allowedNonHdrAssets.Contains(cubeMap))
                continue;
            
            var path = AssetDatabase.GUIDToAssetPath(cubeMap);
            var cubemap = AssetDatabase.LoadAssetAtPath<Cubemap>(path);
            
            var isHdr = !GraphicsFormatUtility.IsSRGBFormat(cubemap.graphicsFormat);
            if (cubemap && !isHdr)
                errors.Add(($"Cubemap {Path.GetFileName(path)} is an sRGB texture, should be float: " + cubemap.graphicsFormat, cubemap));
            
            var ext = Path.GetExtension(path);
            if (ext != ".exr" && ext != ".hdr")
                errors.Add(($"Cubemap {Path.GetFileName(path)} is not .hdr or .exr: " + ext, cubemap));
        }

        if (errors.Any())
        {
            foreach (var e in errors)
                Debug.LogError(e.Item1, e.Item2);
            Assert.Fail($"Found {errors.Count} cubemaps that are not HDR");
        }
    }

    // [Test]
    public void ScenesHaveNoMissingAssets()
    {
        // check with Missing Component info from parsed scene assets
    }
}
