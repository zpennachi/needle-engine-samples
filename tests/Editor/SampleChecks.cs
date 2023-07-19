using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Needle.Engine.Samples;
using Needle.MissingReferences;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Needle.Engine;
using Needle.Engine.Deployment;
using Object = UnityEngine.Object;

namespace SampleChecks
{
    internal class SampleChecks
    {
        internal static List<SampleInfo> GetSamples()
        {
            return AssetDatabase.FindAssets("t:SampleInfo")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<SampleInfo>)
                .ToList();
        }
    }

    [TestFixtureSource(typeof(SampleChecks), nameof(SampleChecks.GetSamples))]
    internal class @_
    {
        private readonly SampleInfo sample;
        private const string PublicInfoCategoryName = "Docs and Deployments";
        
        public @_(SampleInfo sampleInfo)
        {
            sample = sampleInfo;
        }
        
        [Test]
        [Category(PublicInfoCategoryName)]
        public async Task IsLive()
        {
            var sampleLiveUrl = sample.LiveUrl;
            Assert.IsNotEmpty(sampleLiveUrl, "Live URL is invalid");
            
            // HTTP Header Only Request
            var request = new UnityWebRequest(sampleLiveUrl, "HEAD");
            request.timeout = 5;

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            Assert.That(request.responseCode, Is.EqualTo(200), "Sample is not live: " + sample.name);
        }

        // TODO this could be based on the current package version and e.g. only allow 2-3 minor version deviations
        private const int RequiredMajorVersion = 3;
        private const int RequiredMinorVersion = 4;
        
        [Test]
        [Category(PublicInfoCategoryName)]
        public async Task VersionIsNotTooOld()
        {
            // fetch the HTML page
            var sampleLiveUrl = sample.LiveUrl;
            Assert.IsNotEmpty(sampleLiveUrl, "Live URL is invalid");
            
            var request = UnityWebRequest.Get(sampleLiveUrl);
            request.timeout = 5;
            
            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();
            
            // this is the HTML file; if it was generated by Needle, it should contain a meta tag with generator and version
            var html = request.downloadHandler.text;

            // create a regex for this that extracts the content
            // <meta name="needle-engine" content="3.5.8-alpha">
            var regex = new System.Text.RegularExpressions.Regex("<meta name=\"needle-engine\" content=\"(?<version>.*)\">");
            var match = regex.Match(html);
            var version = match.Groups["version"].Value;
            
            // regex check with matches for major/minor/patch-pre so that we can use these as matches
            var regex2 = new System.Text.RegularExpressions.Regex(@"(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(-(?<suffix>\w+))?"); 
            match = regex2.Match(version);
            var major = match.Groups["major"].Value;
            var minor = match.Groups["minor"].Value;
            var patch = match.Groups["patch"].Value;    
            var pre = match.Groups["suffix"].Value;
            
            // TODO proper SemVer check
            var isSemver = !string.IsNullOrEmpty(major) && !string.IsNullOrEmpty(minor) && !string.IsNullOrEmpty(patch);

            Debug.Log("Version: " + version);
            if (!isSemver)
            {
                Assert.Inconclusive("Version not detected in the HTML meta tags of the live sample. That usually means the used Needle Engine version is too old.");
            }
            else
            {
                Assert.GreaterOrEqual(int.Parse(major), RequiredMajorVersion, "Version is too old: " + version);
                Assert.GreaterOrEqual(int.Parse(minor), RequiredMinorVersion, "Version is too old: " + version);
            }
        }

        [Test]
        [Category(PublicInfoCategoryName)]
        public void HasValidInfo()
        {
            Assert.True(sample.Thumbnail, "No thumbnail");
            
            var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sample.Thumbnail));

            // check if it's non power of two
            bool validImport = importer is TextureImporter textureImporter && textureImporter.npotScale == TextureImporterNPOTScale.None;

            int w = sample.Thumbnail.width;
            int h = sample.Thumbnail.height;
            bool textureIsPOT = (w != 0 && (w & (w - 1)) == 0) &&
                                (h != 0 && (h & (h - 1)) == 0);
            
            Assert.True(validImport || textureIsPOT, "Thumbnail is power of two. (results in bad aspect ratio)");
            
            // check for description
            Assert.IsNotEmpty(sample.DisplayNameOrName, "No Display Name");
            Assert.IsNotEmpty(sample.Description, "No description");
            Assert.IsNotEmpty(sample.Tags, "No tags");
            Assert.True(sample.Scene, "No scene assigned");
        }

        [Test]
        [Category(PublicInfoCategoryName)]
        public void HasReadme()
        {
            var sampleDirectory = AssetDatabase.GetAssetPath(sample.Scene);
            sampleDirectory = Path.GetDirectoryName(sampleDirectory);
            if (sampleDirectory == null) return;
            var readmePath = Path.Combine(sampleDirectory, "README.md");
            Assert.IsTrue(File.Exists(readmePath), "No README.md found");
            
            // TODO maybe we can rename it directly here to avoid issues
            Assert.IsTrue(Directory.GetFileSystemEntries(sampleDirectory, "README.md").FirstOrDefault() != null, "File should be called README.md (uppercase)");
        }

        static string[] GetDependencies(Object obj)
        {
            // get path of scene
            var path = AssetDatabase.GetAssetPath(obj);
            
            // get all dependencies of scene
            var dependencies = AssetDatabase.GetDependencies(path, true);
            
            // we can ignore all dependencies that are from com.unity packages (and org.khronos packages?)
            dependencies = dependencies
                .Where(dependency => 
                    !dependency.StartsWith("Packages/com.unity") && 
                    !dependency.StartsWith("Packages/org.khronos"))
                .ToArray();
            
            return dependencies;
        }

        [Test]
        public void DependencySizeBelow10MB()
        {
            var dependencies = GetDependencies(sample.Scene);
            
            // summarize file size of all of them
            var size = dependencies.Sum(dependency => File.Exists(dependency) ? new FileInfo(dependency).Length : 0);
            
            // check if below 10 MB
            var sizeInMb = size / 1024f / 1024f;
            AssertFileSize(sizeInMb, 10, dependencies.ToList(), "Dependency size is too large");
            Debug.Log($"Dependency size: {sizeInMb:F2} MB");
        }

        [Test]
        public void DependenciesInsideKnownPackages()
        {
            var dependencies = GetDependencies(sample.Scene);
            
            var allowedPaths = new[] {
                "Packages/com.needle.engine-samples",
                "Packages/com.needle.engine-exporter",
                "Packages/com.unity.render-pipelines.universal",
                "Packages/com.unity.render-pipelines.core",
                "Packages/org.khronos.unitygltf",
                "Packages/com.needle.dev-assets/Needle FTP Server.asset"
            };
            
            dependencies = dependencies
                .Where(dependency => !allowedPaths.Any(dependency.StartsWith))
                .ToArray();
            
            Assert.IsEmpty(dependencies, $"Some dependencies are outside allowed packages ({dependencies.Length}):\n{string.Join("\n", dependencies)}");
        }

        private void OpenSceneAndCopyIfNeeded()
        {
            var path = AssetDatabase.GetAssetPath(sample.Scene);

            // immutable scenes can't be opened (e.g. from installed sample package)
            // so we're making a mutable copy here to run tests on it.
            // to simplify things, we're always making a copy right now.
            var sceneIsImmutableAndNeedsCopy = true;
            var scenePath = path;
            
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (sceneIsImmutableAndNeedsCopy)
            {
                // create new unique path in Assets to copy the scene to
                var sceneName = Path.GetFileName(path);
                scenePath = $"Assets/{Guid.NewGuid()}_{sceneName}";

                AssetDatabase.CopyAsset(path, scenePath);
            }
            
            // open the temp scene
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single); 

            // remove the mutable copy of the immutable scene
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (sceneIsImmutableAndNeedsCopy)
            {
                AssetDatabase.DeleteAsset(scenePath);
            }
        }

        [Test]
        public void MissingReferencesTest()
        {
            OpenSceneAndCopyIfNeeded();
            
            // perform scans on the opened scene
            var options = new SceneScanner.Options
            {
                IncludeEmptyEvents = true,
                IncludeMissingMethods = true,
                IncludeUnsetMethods = true
            };
            var scanner = new SceneScanner(options);
            if (scanner.FindMissingReferences())
            {
                // log them nicely
                var missingReferences = scanner.SceneRoots;
                var sb = new StringBuilder();

                foreach (var container in missingReferences)
                    container.Value.FormatAsLog(sb);
                
                Assert.Fail("Missing References:\n" + sb);
            }
        }

        [Test]
        public void DeploymentSetupCorrect()
        {
            OpenSceneAndCopyIfNeeded();

            // explicitly get the DeployToFTP component so we can check the path
            var deployToFtps = Object.FindObjectsOfType<DeployToFTP>();
            Assert.LessOrEqual(deployToFtps.Length, 1, "More than one DeployToFTP component found");

            // We want to avoid accidentally keeping staging paths in the scene
            var first = deployToFtps.FirstOrDefault();
            if (first)
            {
                var usesStagingFolder = first.Path.IndexOf("staging", StringComparison.OrdinalIgnoreCase) >= 0;
                Assert.IsFalse(usesStagingFolder, nameof(DeployToFTP) + " component has staging path in it");
            }
            
            // find all types that are DeploymentComponents
            var deploymentTypes = TypeCache.GetTypesWithAttribute<DeploymentComponentAttribute>();
            var allDeploymentComponentsInScene = new List<MonoBehaviour>();
            foreach (var deploymentType in deploymentTypes)
            {
                var components = Object.FindObjectsOfType(deploymentType) as MonoBehaviour[];
                if (components == null) continue;
                allDeploymentComponentsInScene.AddRange(components);
            }
            
            Assert.LessOrEqual(allDeploymentComponentsInScene.Count, 1, "Too many deployment components found: " + string.Join(", ", allDeploymentComponentsInScene.Select(x => x.GetType().Name)));
        }

        private readonly string[] ignoreSizeFolderNames =
        {
            "node_modules",
        };

        [Test]
        public void FolderSizeBelow10MB()
        {
            var path = AssetDatabase.GetAssetPath(sample.Scene);
            
            // walk up until the parent is the "runtime" folder
            var di = new DirectoryInfo(path);
            while (true)
            {
                if (di.Parent == null)
                    break;
                if (di.Parent.Name == "Runtime")
                    break;
                di = di.Parent;
            }
            
            // get files and filter them based on a blacklist
            var fileInfos = di.GetFiles("*.*", SearchOption.AllDirectories);
            fileInfos = fileInfos.Where(x => !ignoreSizeFolderNames.Any(ignoredFolder => x.FullName.Contains(ignoredFolder)))
                                 .ToArray();

            // calculate total file size
            var size = fileInfos.Sum(file => file.Exists ? file.Length : 0);
            
            // runtime folder asset: 17ecbeb2072245a44ad506ab94d30db5
            var packageFolderPath = Path.GetDirectoryName(Path.GetFullPath(AssetDatabase.GUIDToAssetPath("17ecbeb2072245a44ad506ab94d30db5")));
            
            var files = fileInfos.Select(fi =>
            {
                // convert to package-relative path, we know all files are inside the Samples package here.
                var f = fi.FullName;
                if (f.StartsWith(packageFolderPath))
                    f = f.Substring(packageFolderPath.Length + 1);
                f = f.Replace("\\", "/");
                return "Packages/com.needle.engine-samples/" + f;
            }).ToList();
            
            // check if below 10 MB
            var sizeInMb = size / 1024f / 1024f;
            AssertFileSize(sizeInMb, 10, files, "Folder size is too large");
            Debug.Log($"Folder size: {sizeInMb:F2} MB");
        }

        private void AssertFileSize(float sizeInMb, float allowedSize, List<string> files, string message)
        {
            Assert.LessOrEqual(sizeInMb, allowedSize,
                $"{message}: {sizeInMb:F2} MB. List of files ({files.Count}): \n" + string.Join("\n",
                    files
                        .Select(x => (path: x, fileInfo: new FileInfo(x)))
                        .OrderByDescending(f => f.fileInfo.Length)
                        .Select(fi => $"[{(fi.fileInfo.Length / 1024f / 1024f):F2} MB] {fi.path}")
                ));
        }
    }
}
