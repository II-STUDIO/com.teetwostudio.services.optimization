#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using Services.Utility;

namespace Services.Optimization.MeshAnimationSystem
{
    public class MeshAnimationBaker : EditorWindow
    {
        private const string DefaultAssetName = "Untitled";
        private GameObject model;
        private MeshAnimation assetsAnimation;
        private List<MeshAnimation> createAnimations = new List<MeshAnimation>();
        private MeshAnimatorController assetController;
        private Animator animator;
        private SkinnedMeshRenderer[] skinnedMeshRenderersTotal;
        private EditorCoroutine coroutine;
        private bool[] activeSkinnedIndexs;

        private string path = "Assets/";
        private string assetName = DefaultAssetName;
        private int fps = 30;
        private bool optimize;
        private bool preview;

        private bool isReady = false;

        [MenuItem("IIStudio/Optimization/Mesh Animation Baker")]
        public static void Open()
        {
            EditorWindow window = GetWindow<MeshAnimationBaker>();
            window.titleContent = new GUIContent("Mesh Aniamtion Baker");
        }

        private void OnGUI()
        {
            GameObject newModel = EditorGUILayout.ObjectField("Character", model, typeof(GameObject), true) as GameObject;

            if(model != newModel)
            {
                model = newModel;

                if (model != null)
                {
                    animator = model.GetComponent<Animator>();

                    if (!animator)
                        animator = model.GetComponentInChildren<Animator>();

                    assetName = model.name;

                    skinnedMeshRenderersTotal = animator.GetComponentsInChildren<SkinnedMeshRenderer>();

                    isReady = false;

                    if (skinnedMeshRenderersTotal.Length == 0)
                        return;

                    activeSkinnedIndexs = new bool[skinnedMeshRenderersTotal.Length];
                    for(int i = 0; i < activeSkinnedIndexs.Length; i++)
                    {
                        activeSkinnedIndexs[i] = true;
                    }
                }
                else
                {
                    animator = null;
                    assetName = DefaultAssetName;
                }
            }

            EditorView.HorizontalGroup(() =>
            {
                path = EditorGUILayout.TextField("Save Path", path);

                if (GUILayout.Button("Select", GUILayout.Width(55)))
                {
                    string selectPath = EditorUtility.OpenFolderPanel("Animation Save", "", "");
                    path = string.IsNullOrEmpty(selectPath) ? path : selectPath.Substring(selectPath.LastIndexOf("Assets")) + "/";
                }
            });

            assetName = EditorGUILayout.TextField("Asset Name", assetName);
            fps = EditorGUILayout.IntSlider(fps, 15, 90);
            optimize = EditorGUILayout.Toggle("Optimize", optimize);
            preview = EditorGUILayout.Toggle("preview & Dry", preview);

            if (model == null || animator == null || animator.runtimeAnimatorController == null || skinnedMeshRenderersTotal.Length == 0)
                return;

            EditorGUILayout.BeginVertical(EditorKey.GroupBox);

            EditorGUILayout.LabelField("Select Active mesh", GUI.skin.button);

            for (int i = 0; i < activeSkinnedIndexs.Length; i++)
            {
                string skinnedName = skinnedMeshRenderersTotal[i].gameObject.name;
                activeSkinnedIndexs[i] = EditorGUILayout.Toggle(skinnedName, activeSkinnedIndexs[i]);
            }

            EditorGUILayout.EndVertical();

            if (GUILayout.Button(preview ? "Preview" : "Bake"))
            {
                if (animator == null)
                    return;

                if(isReady == false)
                {
                    coroutine = EditorCoroutineUtility.StartCoroutine(GenerateAnimation(true), this);
                    GenerateAnimation(true);
                    return;
                }

                EditorCoroutineUtility.StartCoroutine(GenerateAnimation(false), this);
                GenerateAnimation(false);
            }
        }

        private IEnumerator GenerateAnimation(bool isDry)
        {
            EditorUtility.ClearProgressBar();

            createAnimations.Clear();

            if (!preview && !isDry)
            {
                assetController = CreateInstance<MeshAnimatorController>();
                assetController.fps = fps;
            }

            AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
            Debug.Log($"Found {animationClips.Length} clips. Creating SO with name \"{assetName}\" with Animation FPS {fps}");
            int clipIndex = 1;

            string parentFolder = path + "Baked " + assetName + " Meshs/";

            float increment = 1f / fps;

            foreach (AnimationClip clip in animationClips)
            {
                Debug.Log($"Processing clip {clipIndex}: \"{clip.name}\". Length: {clip.length:N4}.");

                EditorUtility.DisplayProgressBar("Processing Animations", $"Processing animation {clip.name} ({clipIndex} / {animationClips.Length})", clipIndex / (float)animationClips.Length);

                assetsAnimation = CreateInstance<MeshAnimation>();

                assetsAnimation.name = clip.name;
                assetsAnimation.time = clip.length;
                animator.Play(clip.name);

                SkinnedMeshRenderer[] skinnedMeshRenderersTotal = animator.GetComponentsInChildren<SkinnedMeshRenderer>();

                Dictionary<string, int> currentCollection = new Dictionary<string, int>();
                int currentCollectionIndex = -1;

                for (float time = increment; time < clip.length; time += increment)
                {
                    Debug.Log($"Processing {clip.name} frame {(time):N4}");

                    animator.Update(increment);

                    if (preview || isDry)
                    {
                        yield return new WaitForSeconds(increment);
                    }

                    for(int i = 0; i < activeSkinnedIndexs.Length; i++)
                    {
                        if (!activeSkinnedIndexs[i])
                        {
                            continue;
                        }

                        SkinnedMeshRenderer renderSkinnedMsh = skinnedMeshRenderersTotal[i];
                        string skinnedName = renderSkinnedMsh.gameObject.name;

                        if (!preview && !isDry)
                        {
                            Mesh mesh = new Mesh();
                            renderSkinnedMsh.BakeMesh(mesh, true);

                            if (optimize)
                            {
                                mesh.Optimize(); // maybe saves
                            }

                            if (!AssetDatabase.IsValidFolder(parentFolder + clip.name + "/" + skinnedName))
                            {
                                Debug.Log("Path doesn't exist for clip. Creating folder: " + parentFolder + clip.name + "/" + skinnedName);
                                System.IO.Directory.CreateDirectory(parentFolder + clip.name + "/" + skinnedName);
                            }

                            AssetDatabase.CreateAsset(mesh, parentFolder + clip.name + "/" + skinnedName + $"/{skinnedName} {clip.name} - {time:N4}.asset");

                            if (currentCollection.ContainsKey(skinnedName))
                            {
                                assetsAnimation.meshesCollection[currentCollection[skinnedName]].meshes.Add(mesh);
                            }
                            else
                            {
                                currentCollectionIndex++;
                                currentCollection.Add(skinnedName, currentCollectionIndex);

                                assetsAnimation.meshesCollection.Add(new MeshConllection());
                                assetsAnimation.meshesCollection[currentCollectionIndex].skinName = skinnedName;
                                assetsAnimation.meshesCollection[currentCollectionIndex].meshes.Add(mesh);
                            }
                        }
                    }
                }

                Debug.Log($"Setting {clip.name} to have {assetsAnimation.meshesCollection.Count} meshes");

                if (!preview && !isDry)
                {
                    EditorUtility.SetDirty(assetsAnimation);
                    AssetDatabase.CreateAsset(assetsAnimation, path + clip.name + ".asset");

                    createAnimations.Add(assetsAnimation);
                }

                clipIndex++;
            }

            EditorUtility.ClearProgressBar();

            if (!preview && !isDry)
            {
                for(int i = 0; i < createAnimations.Count; i++)
                {
                    MeshAnimationState state = new MeshAnimationState();
                    state.animation = createAnimations[i];
                    assetController.states.Add(state);
                }

                EditorUtility.SetDirty(assetController);
                AssetDatabase.CreateAsset(assetController, path + assetName + " MeshAnimatorController.asset");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if(preview || isDry)
                isReady = true;

            if (isDry)
            {
                coroutine = EditorCoroutineUtility.StartCoroutine(GenerateAnimation(false), this);
                GenerateAnimation(false);
            }

            if (coroutine != null)
                EditorCoroutineUtility.StopCoroutine(coroutine);
        }
    }
}
#endif