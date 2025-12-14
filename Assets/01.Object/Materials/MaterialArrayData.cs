using UnityEngine;
using System.Collections.Generic;
using System;

// [Serializable] 속성을 가진 래퍼 클래스.
// 이 클래스가 인스펙터의 "칸 하나(Pair)" 역할을 하며, Mesh와 Material 배열을 묶습니다.
[Serializable]
public class MeshMaterialPair
{
    // 칸(Pair) 당 하나의 Mesh
    public Mesh mesh;

    // 칸(Pair) 당 N개의 Material
    public Material[] materials;
}

// 사용자 요청에 따라 클래스 이름과 메뉴 이름을 유지합니다.
[CreateAssetMenu(fileName = "MaterialArrayData", menuName = "Custom Data/Material Array")]
public class MaterialArrayData : ScriptableObject
{
    // Mesh와 Material 배열을 묶은 Pair 리스트.
    // List<MeshMaterialPair>가 "칸 하나에 Mesh, Material N개"의 배열 역할을 합니다.
    public List<MeshMaterialPair> pairs = new List<MeshMaterialPair>();

    // --- (옵션) 접근자 예시 ---

    /// <summary>
    /// 지정된 인덱스의 Mesh를 가져옵니다.
    /// </summary>
    public Mesh GetMesh(int pairIndex)
    {
        if (pairIndex >= 0 && pairIndex < pairs.Count)
        {
            return pairs[pairIndex].mesh;
        }
        Debug.LogError($"Pair index out of range: {pairIndex}");
        return null;
    }

    /// <summary>
    /// 지정된 인덱스의 Material 배열에서 특정 Material을 가져옵니다.
    /// </summary>
    public Material GetMaterial(int pairIndex, int materialIndex)
    {
        if (pairIndex >= 0 && pairIndex < pairs.Count &&
            materialIndex >= 0 && materialIndex < pairs[pairIndex].materials.Length)
        {
            return pairs[pairIndex].materials[materialIndex];
        }
        Debug.LogError($"Material index out of range: [{pairIndex}, {materialIndex}]");
        return null;
    }
}