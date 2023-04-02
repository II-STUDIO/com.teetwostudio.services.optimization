#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;

namespace Services.Optimization.MeshAnimationSystem
{
    public class AssetHandlerEditor
    {
        [OnOpenAsset()]
        public static bool OpenEditor(int instanceId, int line)
        {
            MeshAnimatorController controller = EditorUtility.InstanceIDToObject(instanceId) as MeshAnimatorController;

            if (controller)
            {
                MeshAniamtorControllerEditorWindow.Open(controller);
                return true;
            }

            return false;
        }
    }
}
#endif