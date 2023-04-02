#if UNITY_EDITOR
using UnityEditor;
using Services.Utility;

namespace Services.Optimization.MeshAnimationSystem
{
    [CustomEditor(typeof(MeshAnimatorController))]
    public class MeshAnimatorControllerAssetEditor : Editor
    {
        private MeshAnimatorController info;
        private MeshAnimationState state;

        public override void OnInspectorGUI()
        {
            info = target as MeshAnimatorController;
            if (!info)
                return;

            EditorView.VerticalGroup(EditorKey.GroupBox, () =>
            {
                EditorGUILayout.LabelField("FPS : " + info.fps);
                EditorGUILayout.LabelField("State Count : " + info.states.Count);
            });

            EditorView.VerticalGroup(EditorKey.GroupBox, () =>
            {
                int animationCount = info.states.Count;
                for (int i = 0; i < animationCount; i++)
                {
                    state = info.states[i];
                    EditorGUILayout.LabelField(
                        "Name : " + state.animation.name +
                        " , Mesh count : " + state.animation.meshesCollection.Count + 
                        " , Time : " + state.animation.time);
                }
            });
        }
    }
}
#endif