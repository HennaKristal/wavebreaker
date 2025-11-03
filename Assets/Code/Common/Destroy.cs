using UnityEngine;

public class ExplosionDestroy : MonoBehaviour
{
    public void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
