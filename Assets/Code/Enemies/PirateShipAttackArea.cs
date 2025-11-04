using UnityEngine;

public class PirateShipAttackArea : MonoBehaviour
{
    private enum Side { Right, Left }
    [SerializeField] private PirateShipAI pirateShipAI;
    [SerializeField] private Side side = Side.Right;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) { return; }
        pirateShipAI.EnableSideGuns(side == Side.Right);
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) { return; }
        pirateShipAI.DisableSideGuns();
    }
}
