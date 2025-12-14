using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

// MaterialArrayData 클래스에 CustomEditor 적용
[CustomEditor(typeof(MaterialArrayData))]
public class MaterialArrayDataEditor : Editor
{
    private SerializedProperty pairsProp; // List<MeshMaterialPair> 속성

    private void OnEnable()
    {
        // "pairs" 필드를 찾아 연결합니다.
        pairsProp = serializedObject.FindProperty("pairs");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // List 자체를 유니티 스타일로 그립니다.
        EditorGUILayout.PropertyField(pairsProp, new GUIContent("Mesh & Material Pairs"), true);

        // --- 각 요소(Pair)의 내용 커스텀 그리기 ---

        for (int i = 0; i < pairsProp.arraySize; i++)
        {
            SerializedProperty pairProp = pairsProp.GetArrayElementAtIndex(i);

            // Mesh와 Materials 속성을 가져옵니다.
            SerializedProperty meshProp = pairProp.FindPropertyRelative("mesh");
            SerializedProperty materialsProp = pairProp.FindPropertyRelative("materials");

            // 헤더: Mesh 이름과 Material 개수를 표시하여 명확하게 구분합니다.
            string meshName = meshProp.objectReferenceValue ? meshProp.objectReferenceValue.name : "None";
            string header = $"Pair {i} | Mesh: {meshName} (Materials: {materialsProp.arraySize})";

            // Foldout 영역을 그립니다.
            // (isExpanded는 SerializedProperty를 통해 지속성을 가집니다)
            materialsProp.isExpanded = EditorGUILayout.Foldout(materialsProp.isExpanded, header, true);

            if (materialsProp.isExpanded)
            {
                EditorGUI.indentLevel++;

                // Mesh 필드 그리기
                EditorGUILayout.PropertyField(meshProp, new GUIContent("Mesh Object"));

                // Materials 배열 필드 그리기
                EditorGUILayout.PropertyField(materialsProp, new GUIContent("Materials"), true);

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space(5);
        }

        // 변경 사항 저장 및 적용
        serializedObject.ApplyModifiedProperties();
    }
}