using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using Azerion.BlueStack.Editor;

namespace GoogleMobileAds.Editor
{
    public class AdInspectorEditor : UnityEditor.Editor
    {
        static readonly List<ExcludeFolder> PathAndFoldersToExclude = new List<ExcludeFolder> () {
            new ExcludeFolder ("Assets/AdInspector/GoogleMobileAds/Plugins/", "iOS", new ExcludeFolder.Platform[] { ExcludeFolder.Platform.Editor, ExcludeFolder.Platform.IOS }),
            new ExcludeFolder ("Assets/AdInspector/GoogleMobileAds/Runtime/", "iOS", new ExcludeFolder.Platform[] { ExcludeFolder.Platform.Editor, ExcludeFolder.Platform.IOS }),

            new ExcludeFolder ("Assets/AdInspector/GoogleMobileAds/Plugins/", "Android", new ExcludeFolder.Platform[] { ExcludeFolder.Platform.Editor, ExcludeFolder.Platform.Android }),
            new ExcludeFolder ("Assets/AdInspector/GoogleMobileAds/Runtime/", "Android", new ExcludeFolder.Platform[] { ExcludeFolder.Platform.Editor, ExcludeFolder.Platform.Android })
        };
        
        [InitializeOnLoadMethod]
        private static void SetupListener()
        {
            BlueStackSettingsEditor.onIOSMediationNetworksUpdateEvent += ManageIOSPluginsFolder;
        }
        
        private static void ManageIOSPluginsFolder(object sender, List<MediationNetworkDependency> mediationNetworkDependencies)
        {
            AdInspectorHandler inspectorHandler = FindObjectOfType<AdInspectorHandler>();

            foreach (MediationNetworkDependency dependency in mediationNetworkDependencies)
            {
                if (dependency.Name == "AdMob" && dependency.Active )
                {
                    RevertFolders (PathAndFoldersToExclude.Where (folder => folder.platformsToExclude.Contains (ExcludeFolder.Platform.IOS)).ToList ());
                    inspectorHandler.SetInspectorButtonActive(true);
                }
                else if (dependency.Name == "AdMob" && !dependency.Active )
                {
                    ExcludeFolders (PathAndFoldersToExclude.Where (folder => folder.platformsToExclude.Contains (ExcludeFolder.Platform.IOS)).ToList ());
                    inspectorHandler.SetInspectorButtonActive(false);
                }
            }
        }
        
        private static void ExcludeFolders (List<ExcludeFolder> folders)
        {
            foreach (var excludeFolder in folders)
            {
                if (Directory.Exists($"{excludeFolder.path}{excludeFolder.folder}"))
                {
                    Directory.Move($"{excludeFolder.path}{excludeFolder.folder}",
                        $"{excludeFolder.path}.{excludeFolder.folder}");
                    AssetDatabase.Refresh();
                }
            }
        }
        
        private static void RevertFolders (List<ExcludeFolder> folders)
        {
            foreach (var excludeFolder in folders)
            {
                if (Directory.Exists ($"{excludeFolder.path}{excludeFolder.folder}") &&
                    Directory.Exists ($"{excludeFolder.path}.{excludeFolder.folder}")) {
                    Directory.Delete ($"{excludeFolder.path}{excludeFolder.folder}");
                    Directory.Move ($"{excludeFolder.path}.{excludeFolder.folder}",
                        $"{excludeFolder.path}{excludeFolder.folder}");
                    AssetDatabase.Refresh();
                }
            }
        }
    }

    public class ExcludeFolder {
        public enum Platform {
            Editor,
            IOS,
            Android,
        }
 
        public string path;
        public string folder;
        public Platform[] platformsToExclude;
 
        public ExcludeFolder (string path, string folder, Platform[] platformsToExclude) {
            this.path = path;
            this.folder = folder;
            this.platformsToExclude = platformsToExclude;
        }
    }
}