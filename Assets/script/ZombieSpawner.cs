using script;
using UnityEngine;

[SelectionBase]
public class ZombieSpawner : MonoBehaviour
{
    public ZombieAgent PrefabsZombi;
    public void Start() {
        if (GridManager.Instance == null) {
            Debug.LogWarning( "No Grid Manager Found For this Spawner", this);
            return;
        }

        ZombieAgent zombi =Instantiate(PrefabsZombi
            , transform.position+new Vector3(0, Metrics.ZombieSpawnOffset, 0)
            , transform.rotation);
        zombi.Generate(GridManager.Instance);
        Destroy(gameObject);
    }

    public void OnDrawGizmos() {
        if (EditorControlStatics.DisplayGizmos) {
            Gizmos.color = new Color(0, 0.5f, 1, 0.5f);
            Gizmos.DrawSphere(transform.position, 0.4f);
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }
    }
}

