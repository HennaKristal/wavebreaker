using UnityEngine;

[CreateAssetMenu(menuName = "Create New Settings")]
public class Settings : ScriptableObject
{
    public bool screenShakeEnabled = true;
    public bool motionBlurEnabled = true;
    public bool filmGrainEnabled = true;
    public bool showKeyboardControls = true;
    public bool showControllerControls = true;
}
