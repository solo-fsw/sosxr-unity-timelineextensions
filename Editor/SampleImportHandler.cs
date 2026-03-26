using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;


namespace SOSXR.TimelineExtensions.Editor
{
    /// <summary>
    ///     Detects when a Timeline Extensions sample is imported via the Package Manager and automatically installs the
    ///     required companion Unity package (e.g. Animation Rigging or Post Processing) if it is not already present.
    /// </summary>
    public class SampleImportHandler : AssetPostprocessor
    {
        private static readonly List<SamplePackageMapping> SamplePackageMappings = new()
        {
            new("Animation Rigging", "com.unity.animation.rigging", "SOSXR.TimelineExtensions.AnimationRigging", "RigBehaviour.cs", "AnimationRigging"),
            new("Post Processing", "com.unity.postprocessing", "SOSXR.TimelineExtensions.PostProcessing", "PostProcessingBehaviour.cs", "PostProcessing"),
            new("Post Processing (URP)", "com.unity.render-pipelines.universal", "SOSXR.TimelineExtensions.PostProcessing", "PostProcessingBehaviour.cs", "PostProcessing")
        };

        private static readonly HashSet<string> PendingPackages = new();
        private static readonly Dictionary<string, string> PackageSampleNames = new();
        private static readonly Queue<AddOperation> PendingAddOperations = new();

        private static ListRequest activeListRequest;
        private static AddRequest activeAddRequest;
        private static AddOperation activeAddOperation;


        public static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (importedAssets == null || importedAssets.Length == 0)
            {
                return;
            }

            var queuedInstall = false;

            foreach (var mapping in SamplePackageMappings)
            {
                if (!IsSampleImported(importedAssets, mapping))
                {
                    continue;
                }

                if (PendingPackages.Add(mapping.PackageName))
                {
                    PackageSampleNames[mapping.PackageName] = mapping.SampleName;
                    queuedInstall = true;
                }
            }

            if (!queuedInstall)
            {
                return;
            }

            EditorApplication.delayCall += ProcessPendingPackages;
        }


        private static bool IsSampleImported(IEnumerable<string> importedAssets, SamplePackageMapping mapping)
        {
            var asmdefMarker = mapping.AsmdefMarker.ToLowerInvariant();
            var keyFileMarker = mapping.KeyFileName.ToLowerInvariant();
            var sampleFolderMarker = mapping.SampleFolderName.ToLowerInvariant();

            foreach (var assetPath in importedAssets)
            {
                if (string.IsNullOrEmpty(assetPath))
                {
                    continue;
                }

                var normalizedPath = assetPath.Replace('\\', '/').ToLowerInvariant();

                if (normalizedPath.Contains(asmdefMarker) || normalizedPath.Contains(keyFileMarker) || IsImportedSamplePath(normalizedPath, sampleFolderMarker))
                {
                    return true;
                }
            }

            return false;
        }


        private static bool IsImportedSamplePath(string normalizedPath, string sampleFolderName)
        {
            if (!normalizedPath.Contains("/assets/samples/"))
            {
                return false;
            }

            if (!normalizedPath.Contains("/timeline extensions/"))
            {
                return false;
            }

            return normalizedPath.Contains($"/{sampleFolderName}/");
        }


        private static void ProcessPendingPackages()
        {
            if (PendingPackages.Count == 0 || activeListRequest != null)
            {
                return;
            }

            activeListRequest = Client.List(true, true);
            EditorApplication.update += PollListRequest;
        }


        private static void PollListRequest()
        {
            if (activeListRequest == null || !activeListRequest.IsCompleted)
            {
                return;
            }

            EditorApplication.update -= PollListRequest;

            if (activeListRequest.Status == StatusCode.Failure)
            {
                Debug.LogWarning($"Timeline Extensions: Could not query installed packages. Skipping automatic sample package installation. Error: {activeListRequest.Error?.message}");
                activeListRequest = null;

                return;
            }

            var installedPackages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var packageInfo in activeListRequest.Result)
            {
                installedPackages.Add(packageInfo.name);
            }

            foreach (var packageName in PendingPackages)
            {
                var sampleName = PackageSampleNames.TryGetValue(packageName, out var resolvedSampleName) ? resolvedSampleName : "Unknown";

                if (installedPackages.Contains(packageName))
                {
                    Debug.Log($"Timeline Extensions: Package '{packageName}' is already installed. Skipping install for {sampleName} sample.");

                    continue;
                }

                Debug.Log($"Timeline Extensions: Installing {packageName} required by {sampleName} sample...");
                PendingAddOperations.Enqueue(new AddOperation(sampleName, packageName));
            }

            PendingPackages.Clear();
            PackageSampleNames.Clear();
            activeListRequest = null;

            StartNextAddOperation();
        }


        private static void StartNextAddOperation()
        {
            if (activeAddRequest != null || PendingAddOperations.Count == 0)
            {
                return;
            }

            activeAddOperation = PendingAddOperations.Dequeue();
            activeAddRequest = Client.Add(activeAddOperation.PackageName);
            EditorApplication.update += PollAddRequest;
        }


        private static void PollAddRequest()
        {
            if (activeAddRequest == null || !activeAddRequest.IsCompleted)
            {
                return;
            }

            EditorApplication.update -= PollAddRequest;

            if (activeAddRequest.Status == StatusCode.Success)
            {
                Debug.Log($"Timeline Extensions: Installed {activeAddOperation.PackageName} for {activeAddOperation.SampleName} sample.");
            }
            else
            {
                Debug.LogWarning($"Timeline Extensions: Failed to install {activeAddOperation.PackageName} for {activeAddOperation.SampleName} sample. Error: {activeAddRequest.Error?.message}");
            }

            activeAddRequest = null;
            activeAddOperation = null;

            StartNextAddOperation();
        }


        private sealed class SamplePackageMapping
        {
            public readonly string SampleName;
            public readonly string PackageName;
            public readonly string AsmdefMarker;
            public readonly string KeyFileName;
            public readonly string SampleFolderName;

            public SamplePackageMapping(string sampleName, string packageName, string asmdefMarker, string keyFileName, string sampleFolderName)
            {
                SampleName = sampleName;
                PackageName = packageName;
                AsmdefMarker = asmdefMarker;
                KeyFileName = keyFileName;
                SampleFolderName = sampleFolderName;
            }
        }


        private sealed class AddOperation
        {
            public readonly string SampleName;
            public readonly string PackageName;

            public AddOperation(string sampleName, string packageName)
            {
                SampleName = sampleName;
                PackageName = packageName;
            }
        }
    }
}
