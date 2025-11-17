using UnityEngine;

public class PirateShipAttackArea : MonoBehaviour
{
    private enum Side { Right, Left }
    [SerializeField] private PirateShipAI pirateShipAI;
    [SerializeField] private Side side = Side.Right;
    private int targetCount = 0;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Ally")) { return; }

        targetCount++;

        if (targetCount == 1)
        {
            pirateShipAI.EnableSideGuns(side == Side.Right);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Ally")) { return; }

        targetCount--;

        if (targetCount < 0) targetCount = 0;

        if (targetCount == 0)
        {
            pirateShipAI.DisableSideGuns();
        }
    }
}
