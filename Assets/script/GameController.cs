using System;
using Unity.Mathematics;
using UnityEngine;

namespace script
{
    public class GameController:MonoBehaviour
    {
        public Camera Camera;
        public GridManager GridManager;
        public ZombieAgent PrefabsZombieAgent;
        public GameObject Arrow;

        [Header("Strat Zombie")] 
        public Transform[] zombies;

        private void Start() {
            foreach (var z in zombies) {
                ZombieAgent zombie = Instantiate(PrefabsZombieAgent,  z.position+ new Vector3(0, 0.5f, 0),
                    Quaternion.identity);
                zombie.Generate(GridManager);
            }
        }
        
        public void Update() {
            if (Input.GetButtonDown("Fire1")) {
                RaycastHit hit;
                if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hit)) {
                    if (Arrow) Instantiate(Arrow, hit.point, quaternion.identity);
                    Cell cell = GridManager.GetCellFromWorldPos(hit.point);
                    GridManager.StartCalcFlowfield(cell);
                }
            }
            
            //if (Input.GetKeyDown(KeyCode.A)) {
            //    RaycastHit hit;
            //    if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hit)) {
            //        Cell cell = GridManager.GetCellFromWorldPos(hit.point);
            //        ZombieAgent zombie = Instantiate(PrefabsZombieAgent, hit.point + new Vector3(0, 0.5f, 0),
            //            Quaternion.identity);
            //        zombie.Generate(GridManager);
            //    }
            //}
        }
    }
}