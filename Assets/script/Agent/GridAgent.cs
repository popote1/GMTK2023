using System.Collections;
using System.Collections.Generic;
using System.Linq;
using script;
using UnityEngine;

public class GridAgent : MonoBehaviour
{
    [SerializeField] protected Rigidbody Rigidbody;
    [SerializeField] protected float MaxMoveSpeed;
    [SerializeField] protected float MoveSpeed;
    [SerializeField] protected float Drag;
    [SerializeField] protected GridManager GridManager;
    [SerializeField] protected Animator _animator;
    public Subgrid Subgrid;
    public GameObject PSEmoteRedSquare;
    [Header("HeightOffSetting")] 
    public float HeightOffSetting;
    public LayerMask GroundLayer;
    
    public GameObject SelectionSprite;
    
    protected bool _isAttcking;
    protected bool _isSelected;
    
    public bool IsSelected
    {
        get => _isSelected;
        set {
            if (SelectionSprite) SelectionSprite.SetActive(value);
            _isSelected = value;
        }
    }
    
    public void Generate(GridManager gridManager) => GridManager = gridManager;
    
    protected virtual void Update() {
        if (_isAttcking) {
            ManageAttack();
        }
        else
        {
            if (Rigidbody.velocity.magnitude > 0.5f) transform.forward = Rigidbody.velocity;
            RaycastHit hit;
            if (Physics.Raycast(new Ray(transform.position, Vector3.down), out hit, 4, GroundLayer)) {
                transform.position = new Vector3(transform.position.x, hit.point.y + HeightOffSetting, transform.position.z);
            }
        }
        _animator.SetFloat("Velocity", Rigidbody.velocity.magnitude/MaxMoveSpeed);
    }
    protected virtual void FixedUpdate() {
        if (Subgrid!=null&&!_isAttcking) {
            Cell cell =Subgrid.GetCellFromWorldPos(transform.position);
            if (cell == null) {
                Cell currentPos = GridManager.GetCellFromWorldPos(transform.position);
                if (currentPos == null) {
                    PSEmoteRedSquare.SetActive(true);
                    Debug.LogWarning("Zombie out of the Game Zone", this);
                    return;
                }
                ManagerRecalculationOrExtraPathToSubGrid(currentPos);
                return;
            }

            if (Subgrid.NextSubGrid != null && Subgrid.TargetCells[0].Pos==cell.Pos) {
                Subgrid = Subgrid.NextSubGrid;
                Debug.Log("Is In Position");
                return;
            }
            PSEmoteRedSquare.SetActive(false);
            Rigidbody.AddForce(new Vector3(cell.DirectionTarget.x, 0, cell.DirectionTarget.y) * MoveSpeed);
            Rigidbody.velocity -= Rigidbody.velocity * Drag;
            //transform.position = new Vector3(transform.position.x, 0.5f,transform.position.z );
        }
    }
    
    protected virtual void ManagerRecalculationOrExtraPathToSubGrid(Cell currentPos) {
        List<Chunk> path =GridManager.GetAStartPath(currentPos.Chunk,
            GridManager.GetCellFromPos(Subgrid.TargetPos).Chunk);
        foreach (var neighbor in GridManager.GetNeighborsOfPath(path)) {
            if (path.Contains(neighbor)) continue;
            path.Add(neighbor);
        }
        Subgrid.AddChunksToSubGrid(path.ToArray());
    }
    
    protected virtual void ManageAttack(){}
}
