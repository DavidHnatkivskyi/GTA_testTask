using UnityEngine;

namespace Workarounds
{
    public class ForceLandscape : MonoBehaviour
    {
        private void Awake()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }
}