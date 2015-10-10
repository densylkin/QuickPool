using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(ObjectsPool))]
public class ObjectsPoolEditor : Editor
{
    private ObjectsPool m_Target { get { return (ObjectsPool)target; } }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(m_Target, "poolmanager");

        PoolManagerSettings();
        Toolbar();

        EditorUtility.SetDirty(m_Target);
    }

    private void PoolManagerSettings()
    {
        GUILayout.BeginVertical(GUI.skin.box);

        m_Target.debugMessages = EditorGUILayout.Toggle("Show debug messages? ", m_Target.debugMessages);
        m_Target.spawnDespawnMessages = EditorGUILayout.Toggle("Send spawn/despawn messages to Objects? ", m_Target.spawnDespawnMessages);

        if (Application.isPlaying)
        {
            foreach (var pool in m_Target.pools)
            {
                GUILayout.Label(pool.poolName + "_spawned count: " + pool.spawnedCount + "/" + pool.totalCount);
            }
        }

        GUILayout.EndVertical();
    }

    private void Toolbar()
    {
        GUILayout.Space(10f);
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        GUILayout.Space(10f);
        m_Target.foldout = EditorGUILayout.Foldout(m_Target.foldout, "Pools (" + m_Target.pools.Count + ")");

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Expand All", EditorStyles.toolbarButton, GUILayout.Width(65)))
            m_Target.pools.ForEach(pool => pool.foldout = true);

        if (GUILayout.Button("Collapse All", EditorStyles.toolbarButton, GUILayout.Width(71)))
            m_Target.pools.ForEach(pool => pool.foldout = false);

        GUI.color = Color.green;
        if (GUILayout.Button("Add", EditorStyles.toolbarButton, GUILayout.Width(30)))
            m_Target.pools.Add(Pool.CreateNewPool());
        GUI.color = Color.white;

        GUILayout.EndHorizontal();

        if(m_Target.foldout)
        {
            for (int i = 0; i < m_Target.pools.Count; i++ )
            {
                Pool pool = m_Target.pools[i];

                GUILayout.BeginHorizontal();
                GUILayout.Space(10f);
                GUILayout.BeginVertical();
                PoolArea(pool);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }
    }

    private void PoolArea(Pool pool)
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Space(10f);

        pool.foldout = EditorGUILayout.Foldout(pool.foldout, pool.poolName);
        GUILayout.FlexibleSpace();

        if (Application.isPlaying)
            GUILayout.Label("Spawned: " + pool.spawnedCount + "/" + pool.totalCount);

        //if (GUILayout.Button("Despawn All", EditorStyles.toolbarButton, GUILayout.Width(80)))

        if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(80)))
            pool.ClearAndDestroy();

        if (GUILayout.Button("Preinstantiate", EditorStyles.toolbarButton, GUILayout.Width(80)))
            pool.PreInstantiate();

        GUI.color = Color.red;
        if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(15)))
        {
            pool.ClearAndDestroy();
            if (pool.m_Root != null)
                GameObject.DestroyImmediate(pool.m_Root);
            m_Target.pools.Remove(pool);
        }

        GUI.color = Color.white;

        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();

        if (pool.foldout)
        {
            pool.Prefab = EditorGUILayout.ObjectField("Prefab: ", pool.Prefab, typeof(GameObject), false) as GameObject;
            pool.size = EditorGUILayout.IntField("Pool size: ", pool.size);
            pool.allowMore = EditorGUILayout.Toggle("Allow more: ", pool.allowMore);
            pool.debugMessages = EditorGUILayout.Toggle("Debug messages: ", pool.debugMessages);
        }

        GUILayout.EndVertical();
    }
}
