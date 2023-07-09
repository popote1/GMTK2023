using System;
using UnityEngine;

namespace script
{
    public class GameController:MonoBehaviour
    {
        public Camera Camera;
        public GridManager GridManager;
        public ZombieAgent PrefabsZombieAgent;
        
        public void Update() {
            if (Input.GetButtonDown("Fire1")) {
                RaycastHit hit;
                if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hit)) {
                    Debug.Log("Hit");
                    GridManager.ColorCell(hit.point);
                    Cell cell = GridManager.GetCellFromWorldPos(hit.point);
                    GridManager.StartCalcFlowfield(cell);
                }
            }
            
            if (Input.GetKeyDown(KeyCode.A)) {
                RaycastHit hit;
                if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hit)) {
                    Cell cell = GridManager.GetCellFromWorldPos(hit.point);
                    ZombieAgent zombie = Instantiate(PrefabsZombieAgent, hit.point + new Vector3(0, 0.5f, 0),
                        Quaternion.identity);
                    zombie.Generate(GridManager);
                }
            }
        }
    }
}