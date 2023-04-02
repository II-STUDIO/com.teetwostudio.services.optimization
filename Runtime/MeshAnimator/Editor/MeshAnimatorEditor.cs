#if UNITY_EDITOR
using Services.Utility;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Services.Optimization.MeshAnimationSystem
{
    [CustomEditor(typeof(MeshAnimator))]
    public class MeshAnimatorEditor : Editor
    {
        public MeshAnimator animator { get; private set; }
        private MeshAnimationState state;
        private MeshFilter[] meshFilter_temp;
        private MeshFilter[] meshFilter;
        private MeshFilter meshFilter_ref;
        private WaitForSeconds waitForFrameRate;
        private int meshIndex = 0;
        private int targetStateIndex = 0;

        private bool isSelecting = false;

        public override void OnInspectorGUI()
        {
            animator = target as MeshAnimator;

            if (!animator)
                return;

            animator.controller = EditorGUILayout.ObjectField("Controller", animator.controller, typeof(MeshAnimatorController), true) as MeshAnimatorController;
            animator.autoUpdate = EditorGUILayout.Toggle("Auto Update", animator.autoUpdate);

            serializedObject.ApplyModifiedProperties();

            if (!animator.controller)
                return;


            if (animator.controller.states.Count == 0)
                return;

            int meshCollectionCount = animator.controller.states[targetStateIndex].animation.meshesCollection.Count;

            meshFilter_temp = animator.GetComponentsInChildren<MeshFilter>();

            state = animator.controller.states[targetStateIndex];

            if(meshFilter == null)
                meshFilter = new MeshFilter[meshCollectionCount];

            if (meshFilter_temp.Length < meshCollectionCount)
            {
                for(int i = 0;i< meshFilter_temp.Length; i++)
                {
                    meshFilter[i] = meshFilter_temp[i];
                }

                for (int i = meshFilter_temp.Length; i < meshCollectionCount; i++)
                {
                    MeshConllection meshConllection = state.animation.meshesCollection[i];

                    GameObject gameObject = new GameObject(meshConllection.skinName);
                    gameObject.AddComponent<MeshRenderer>();

                    meshFilter_ref = gameObject.AddComponent<MeshFilter>();
                    meshFilter_ref.sharedMesh = meshConllection.meshes[0];
                    meshFilter_ref.transform.SetParent(animator.transform);
                    meshFilter_ref.transform.localPosition = Vector3.zero;
                    meshFilter_ref.transform.localEulerAngles = Vector3.zero;

                    meshFilter[i] = meshFilter_ref;
                }
            }
            else
            {
                meshFilter = meshFilter_temp;

                for (int i = 0; i < meshCollectionCount; i++)
                {
                    meshFilter_ref = meshFilter[i];

                    if (meshFilter_ref.sharedMesh != null)
                        continue;

                    MeshConllection meshConllection = state.animation.meshesCollection[i];

                    meshFilter_ref.sharedMesh = meshConllection.meshes[0];
                }
            }

            EditorGUILayout.BeginVertical(EditorKey.GroupBox);

            EditorView.HorizontalGroup(() =>
            {
                EditorGUILayout.LabelField("Preview : " + state.animation.name, GUI.skin.button);

                if (GUILayout.Button("Select", GUILayout.Width(50)))
                {
                    PreviewSelectionWindowEditor.Open(this);
                }
            });

            if (animator.EditorPlayRountine != null)
            {
                if (GUILayout.Button("Stop"))
                {
                    animator.runInEditMode = false;
                    animator.StopCoroutine(animator.EditorPlayRountine);
                    animator.EditorPlayRountine = null;

                    SetMesh(0);
                }
            }
            else
            {
                if (GUILayout.Button("Play"))
                {
                    waitForFrameRate = new WaitForSeconds(1f / animator.controller.fps);

                    animator.runInEditMode = true;
                    animator.EditorPlayRountine = animator.StartCoroutine(PlayRountine());

                    SetMesh(0);
                }
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
        }

        private IEnumerator PlayRountine()
        {
            while (true)
            {
                yield return waitForFrameRate;

                SetMesh(meshIndex);

                if (meshIndex < state.animation.meshesCollection[0].meshes.Count - 1)
                    meshIndex++;
                else
                    meshIndex = 0;
            }
        }

        private void SetMesh(int meshIndexTarget)
        {
            meshIndex = meshIndexTarget;

            int collectionCount = state.animation.meshesCollection.Count;

            if (collectionCount == 1)
                meshFilter[0].sharedMesh = state.animation.meshesCollection[0].meshes[meshIndex];
            else
            {
                for (int i = 0; i < collectionCount; i++)
                {
                    meshFilter[i].sharedMesh = state.animation.meshesCollection[i].meshes[meshIndex];
                }
            }
        }

        public void SelectPreviewIndex(int index)
        {
            targetStateIndex = index;
            state = animator.controller.states[targetStateIndex];

            SetMesh(0);
        }
    }
}
#endif