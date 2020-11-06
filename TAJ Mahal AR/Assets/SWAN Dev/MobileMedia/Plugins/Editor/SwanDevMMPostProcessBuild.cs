
#if UNITY_IOS
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

/// <summary>
/// Automatically manage the framework settings when building XCode project. 
/// </summary>
public class SwanDevMMPostProcessBuild
{
    private const string PHOTO_LIBRARY_USAGE_DESCRIPTION = "For saving and picking media";
    private const string PHOTO_LIBRARY_ADDITIONS_USAGE_DESCRIPTION = "For saving and picking media";

#pragma warning disable 0162
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string buildPath)
    {
        if (target == BuildTarget.iOS)
        {
            string pbxProjectPath = PBXProject.GetPBXProjectPath(buildPath);
            string plistPath = Path.Combine(buildPath, "Info.plist");

            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(pbxProjectPath);

            List<string> targetGuids = new List<string>();
            targetGuids.Add(pbxProject.ProjectGuid());
#if UNITY_2019_3_OR_NEWER
            targetGuids.Add(pbxProject.GetUnityMainTargetGuid());
#else
            targetGuids.Add(pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName()));
#endif
            foreach(string guid in targetGuids)
            {
                pbxProject.AddBuildProperty(guid, "OTHER_LDFLAGS", "-framework Photos");
                pbxProject.AddBuildProperty(guid, "OTHER_LDFLAGS", "-framework MobileCoreServices");
                pbxProject.RemoveFrameworkFromProject(guid, "Photos.framework");
            }

            File.WriteAllText(pbxProjectPath, pbxProject.WriteToString());

            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            PlistElementDict rootDict = plist.root;
            rootDict.SetString("NSPhotoLibraryUsageDescription", PHOTO_LIBRARY_USAGE_DESCRIPTION);
            rootDict.SetString("NSPhotoLibraryAddUsageDescription", PHOTO_LIBRARY_ADDITIONS_USAGE_DESCRIPTION);

            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
#pragma warning restore 0162
}
#endif