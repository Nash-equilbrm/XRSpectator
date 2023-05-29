using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshSimplifier : MonoBehaviour
{
    [SerializeField, Range(0f, 1f), Tooltip("The desired quality of the simplified mesh.")]
    private float quality = 0.5f;

    private void Start()
    {
        Simplify();
    }

    private void Simplify()
    {
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) 
            return;

        Mesh sourceMesh = meshFilter.sharedMesh;
        if (sourceMesh == null) 
            return;

        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        //meshSimplifier.Vertices = sourceMesh.vertices;

        //for (int i = 0; i < sourceMesh.subMeshCount; i++)
        //{
        //    meshSimplifier.AddSubMeshTriangles(sourceMesh.GetTriangles(i));
        //}
        meshSimplifier.Initialize(sourceMesh);

        meshSimplifier.SimplifyMesh(quality);

        var destMesh = meshSimplifier.ToMesh();
        GetComponent<MeshFilter>().sharedMesh = destMesh;

        //var newMesh = new Mesh();
        //newMesh.subMeshCount = meshSimplifier.SubMeshCount;
        //newMesh.vertices = meshSimplifier.Vertices;

        //for (int i = 0; i < meshSimplifier.SubMeshCount; i++)
        //{
        //    newMesh.SetTriangles(meshSimplifier.GetSubMeshTriangles(i), 0);
        //}

        //meshFilter.sharedMesh = newMesh;
    }
}