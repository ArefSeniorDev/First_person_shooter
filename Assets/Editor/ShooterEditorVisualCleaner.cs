using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ShooterEditorVisualCleaner
{
    static ShooterEditorVisualCleaner()
    {
        EditorApplication.delayCall += Apply;
    }

    [MenuItem("Tools/Shooter/Fix Game View Visual Artifacts")]
    public static void Apply()
    {
        HideGameViewAnnotations();
    }

    private static void HideGameViewAnnotations()
    {
        // Game View Gizmos can show light/audio/canvas icons over the weapon; hide those editor annotations.
        System.Type annotationUtility = System.Type.GetType("UnityEditor.AnnotationUtility,UnityEditor");
        if (annotationUtility == null)
        {
            return;
        }

        MethodInfo getAnnotations = annotationUtility.GetMethod("GetAnnotations", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        MethodInfo setIconEnabled = annotationUtility.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        MethodInfo setGizmoEnabled = annotationUtility.GetMethod("SetGizmoEnabled", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (getAnnotations == null)
        {
            return;
        }

        System.Array annotations = getAnnotations.Invoke(null, null) as System.Array;
        if (annotations == null)
        {
            return;
        }

        for (int i = 0; i < annotations.Length; i++)
        {
            object annotation = annotations.GetValue(i);
            if (annotation == null)
            {
                continue;
            }

            FieldInfo classIdField = annotation.GetType().GetField("classID");
            FieldInfo scriptClassField = annotation.GetType().GetField("scriptClass");
            if (classIdField == null || scriptClassField == null)
            {
                continue;
            }

            int classId = (int)classIdField.GetValue(annotation);
            string scriptClass = scriptClassField.GetValue(annotation) as string;
            if (!ShouldHideAnnotation(classId, scriptClass))
            {
                continue;
            }

            if (setIconEnabled != null)
            {
                setIconEnabled.Invoke(null, new object[] { classId, scriptClass, 0 });
            }

            if (setGizmoEnabled != null)
            {
                setGizmoEnabled.Invoke(null, new object[] { classId, scriptClass, 0 });
            }
        }

    }

    private static bool ShouldHideAnnotation(int classId, string scriptClass)
    {
        if (classId == 82 || classId == 108 || classId == 121 || classId == 198 || classId == 199 || classId == 223 || classId == 224)
        {
            return true;
        }

        if (scriptClass == null)
        {
            return false;
        }

        return scriptClass == "AudioSource"
            || scriptClass == "Light"
            || scriptClass == "LensFlare"
            || scriptClass == "ParticleSystem"
            || scriptClass == "ParticleSystemRenderer"
            || scriptClass == "Canvas"
            || scriptClass == "RectTransform";
    }
}
