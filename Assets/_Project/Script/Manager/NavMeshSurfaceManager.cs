using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshSurfaceManager : MonoBehaviour
{
    private NavMeshSurface[] _navMeshSurfaces;

    void Awake()
    {
        _navMeshSurfaces = GetComponents<NavMeshSurface>();
        if (InteractableSceneManager.Instance.SetNavMeshSurface(this))
        {
            Debug.Log("NavMeshSurfaceManager registrato");
        }
    }

    public void ReBuildNavMesh()
    {
        foreach (NavMeshSurface navMeshSurface in _navMeshSurfaces)
        {
            navMeshSurface.BuildNavMesh();
        }
    }
}
