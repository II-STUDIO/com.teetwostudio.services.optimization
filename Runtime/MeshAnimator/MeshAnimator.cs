#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.MeshAnimationSystem
{
    [ExecuteInEditMode]
    public class MeshAnimator : MonoBehaviour
    {
        public MeshAnimatorController controller;

        public bool autoUpdate = true;

        private MeshFilter[] meshFilter;
        private MeshAnimationState state;
        private MeshAnimationState state_ref;
        private MeshConllection meshConllection_ref;
        private TransitionInfo transitionInfo_ref;
        private ParameterInfo parameter;

        private CountDown frameRateCountdown;
        private Condition condition_ref;

        private float frameFrequency;
        private bool isRegistedUpdate = false;

        private int stateIndex = 0;
        private int meshIndex = 0;
        private int meshIndexCount = 0;
        private int stateCount = 0;

        private Dictionary<string, int> sateIndexHash = new Dictionary<string, int>();

#if UNITY_EDITOR
        public Coroutine EditorPlayRountine { get; set; }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
                return;

            if (EditorPlayRountine == null)
                return;

            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
        }
#endif
        private void Awake()
        {
            stateIndex = 0;
            meshIndex = 0;
            state = null;

            if (!controller)
                return;

#if UNITY_EDITOR
            if (Application.isPlaying && EditorPlayRountine != null)
                StopCoroutine(EditorPlayRountine);
#endif

            stateIndex = controller.defaultStateIndex;
            frameFrequency = 1f / controller.fps;
            frameRateCountdown.onComplete = RefreshMesh;

            meshFilter = GetComponentsInChildren<MeshFilter>();

            stateCount = controller.states.Count;

            if (stateCount > 0)
            {
                state = controller.states[stateIndex];
                meshIndexCount = state.animation.meshesCollection[0].meshes.Count - 1;
            }

            RefreshMesh();
        }

        private void OnEnable()
        {
            if (!autoUpdate)
            {
                return;
            }

            isRegistedUpdate = true;
            frameRateCountdown.Start(frameFrequency);
            SystemBaseUpdater.Instance.AddUpdater(UpdateFrame);
        }

        private void OnDisable()
        {
            if (!autoUpdate && !isRegistedUpdate)
            {
                return;
            }

            isRegistedUpdate = false;
            frameRateCountdown.Clear();
            SystemBaseUpdater.Instance.RemoveUpdater(UpdateFrame);
        }

        public Mesh GetMesh(int stateIndex, int meshIndex)
        {
            if (stateCount == 0)
                return null;

            state_ref = controller.states[stateIndex];

            if (state_ref.animation.meshesCollection.Count == 0)
                return null;

            return state_ref.animation.meshesCollection[0].meshes[meshIndex];
        }

        public int StringToStateIndex(string animationName)
        {
            int count = controller.states.Count;

            for (int i = 0; i < count; i++)
            {
                if (animationName == controller.states[i].animation.name)
                    return i;
            }

            Debug.LogErrorFormat($"The animation name <{animationName}> is not exited");
            return -1;
        }

        public int AnimationToStateIndex(MeshAnimation animation)
        {
            return StringToStateIndex(animation.name);
        }

        public void PlayState(MeshAnimation animation)
        {
            PlayState(animation.name);
        }

        public void PlayState(string animationName)
        {
            if(sateIndexHash.TryGetValue(animationName, out int index))
            {
                PlayState(index);
                return;
            }

            int targetIndex = StringToStateIndex(animationName);
            if (targetIndex < 0)
            {
                Debug.LogErrorFormat($"The animation name <{animationName}> is not exited");
                return;
            }

            sateIndexHash.Add(animationName, targetIndex);

            PlayState(targetIndex);
        }

        public void PlayState(int targetState)
        {
            if (targetState == stateIndex)
                return;

            meshIndex = 0;

            stateIndex = targetState;

            state = controller.states[stateIndex];

            meshIndexCount = state.animation.meshesCollection[0].meshes.Count - 1;

            frameRateCountdown.Clear();

            RefreshMesh();
        }

        public void UpdateFrame(float deltaTime)
        {
            frameRateCountdown.Update(deltaTime);
        }

        private void RefreshMesh()
        {
            frameRateCountdown.Start(frameFrequency);

            if (state == null)
                return;

            int collectionCount = state.animation.meshesCollection.Count;

            if (collectionCount == 1)
                meshFilter[0].mesh = state.animation.meshesCollection[0].meshes[meshIndex];
            else
            {
                for(int i = 0; i < collectionCount; i++)
                {
                    meshFilter[i].mesh = state.animation.meshesCollection[i].meshes[meshIndex];
                }
            }

            if (meshIndex < meshIndexCount - 1)
                meshIndex++;
            else
            {
                if (state.animation.isLoop)
                {
                    meshIndex = 0;
                    return;
                }

                PlayState(GetNextStateIndex());
            }
        }

        private int GetNextStateIndex()
        {
            if (state.transitionInfos == null)
                return stateIndex;

            int infoCount = state.transitionInfos.Count;
            if (infoCount == 0)
                return stateIndex;

            if(infoCount == 1)
            {
                transitionInfo_ref = state.transitionInfos[0];
                return CheckValideCondition(transitionInfo_ref) ? transitionInfo_ref.targetStateIndex : stateIndex;
            }

            for(int i = 0; i < infoCount; i++)
            {
                transitionInfo_ref = state.transitionInfos[0];

                if (CheckValideCondition(transitionInfo_ref))
                    return transitionInfo_ref.targetStateIndex;
            }

            return stateIndex;
        }

        private bool CheckValideCondition(TransitionInfo transitionInfo)
        {
            int count = transitionInfo.conditions.Count;

            if (count > 0)
            {
                for(int i = 0; i < count; i++)
                {
                    condition_ref = transitionInfo.conditions[i];

                    if (condition_ref.IsValide(parameter))
                        return true;
                }

                return false;
            }

            return true;
        }
    }
}