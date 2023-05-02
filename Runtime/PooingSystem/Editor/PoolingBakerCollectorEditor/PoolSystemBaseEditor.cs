#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using Services.Utility;

namespace Services.Optimization.PoolingSystem.ScriptEditor
{
    [CustomEditor(typeof(PoolSystemBaseHandler))]
    public class PoolSystemBaseEditor : Editor
    {
        PoolSystemBaseHandler _systemBaseHandler;
        Dictionary<int, AnimBool> _animBools = new Dictionary<int, AnimBool>();

        float _miniBtn = 30f;

        public override void OnInspectorGUI()
        {
            _systemBaseHandler = target as PoolSystemBaseHandler;
            if (!_systemBaseHandler) return;

             EditorView.VerticalGroup(EditorKey.GroupBox, () =>
             {
                 PoolEditorCache.objectPreview = (ObjectPreview)EditorGUILayout.EnumPopup("Preview Object",PoolEditorCache.objectPreview);
                 EditorGUILayout.PropertyField(serializedObject.FindProperty("_initMethod"));
                 EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxCapacity"));
                 EditorGUILayout.PropertyField(serializedObject.FindProperty("_container"));
             });

            //Check Confix
            EditorView.VerticalGroup(EditorKey.GroupBox, () =>
            {
                int count = _systemBaseHandler.CreateLayers.Count;
                for (int i = 0; i < count; i++)
                {
                    if (!_animBools.ContainsKey(i)) _animBools.Add(i, new AnimBool());
                    DrawLayerItem(i);
                }

                EditorView.HorizontalLine();

                if (GUILayout.Button("New Layer"))
                {
                    CreateLayer newLayer = new CreateLayer();
                    newLayer.SetupValue();
                    _systemBaseHandler.CreateLayers.Add(newLayer);
                }
            });

            serializedObject.ApplyModifiedProperties();
            Undo.RecordObject(_systemBaseHandler, "BakerCollector");

            if(GUI.changed) EditorUtility.SetDirty(_systemBaseHandler);

            Repaint();
        }

        void DrawLayerItem(int index)
        {
            if (index > _systemBaseHandler.CreateLayers.Count - 1) 
                return;

            var layer = _systemBaseHandler.CreateLayers[index];
            bool isContineus = true;

            EditorView.FoldoutGroup(_animBools[index], layer.layerName, () =>
            {
                if (!isContineus)
                    return;

                EditorView.VerticalGroup(EditorKey.GroupBox, () =>
                {
                    layer.layerName = EditorGUILayout.TextField("Layer Name", layer.layerName);
                    layer.source = (CreateSource)EditorGUILayout.EnumPopup("Soruce Loader", layer.source);

                    if(layer.source == CreateSource.Defualt)
                    {
                        for (int i = 0; i < layer.poolingCreaters.Count; i++)
                        {
                            DrawItem(index, i);
                        }

                        EditorView.HorizontalLine();

                        if (GUILayout.Button("+"))
                        {
                            layer.poolingCreaters.Add(new PoolerCreater());
                        }

                    }
                    else if(layer.source == CreateSource.LoadPath)
                    {
                        EditorView.VerticalGroup(EditorKey.GroupBox, () =>
                        {
                            layer.sourcePath = EditorGUILayout.TextField("Assets Path", layer.sourcePath);
                        });
                    }

                });

                _systemBaseHandler.CreateLayers[index] = layer;

            }, () => 
            { 
                if(GUILayout.Button("X", GUILayout.Width(_miniBtn)))
                {
                    _systemBaseHandler.CreateLayers.RemoveAt(index);
                    isContineus = false;
                }
            });
        }

        void DrawItem(int layerIndex, int createrIndex)
        {
            var layer = _systemBaseHandler.CreateLayers[layerIndex];
            var creater = layer.poolingCreaters[createrIndex];
            var id = creater.profile ? creater.profile.ID : "Profile is null";

            var isConfix = !PoolEditorCache.AddObjectCacheSucces(creater.profile);
            bool isContinues = true;

            EditorView.VerticalGroup(EditorKey.GroupBox, () =>
            {
                EditorView.HorizontalGroup(GUI.skin.box, () =>
                {
                    EditorGUILayout.LabelField("ID : " + id);
                    if (GUILayout.Button("-",GUILayout.Width(_miniBtn)))
                    {
                        layer.poolingCreaters.RemoveAt(createrIndex);
                        isContinues = false;
                        return;
                    }
                });

                if (!isContinues)
                    return;

                creater.profile = (PoolProfile)EditorGUILayout.ObjectField("Profile", creater.profile, typeof(PoolProfile), true);

                if (creater.profile && creater.profile.Prefab && PoolEditorCache.objectPreview == ObjectPreview.On)
                {
                    EditorView.HorizontalLine();
                    EditorView.DrawPrefviewObject(creater.profile.Prefab.gameObject, 80);
                }

                if(isConfix)
                    EditorGUILayout.HelpBox("ID of this profile are same with other profile id please check profiles id", MessageType.Error);

                layer.poolingCreaters[createrIndex] = creater;
                _systemBaseHandler.CreateLayers[layerIndex] = layer;
            });
        }
    }
}
#endif
