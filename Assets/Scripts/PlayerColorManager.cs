using System.Collections.Generic;
using UnityEngine;

namespace PlayerColorManager
{

    public class PlayerColorManager : MonoBehaviour

    {
        public static void setPlayerColor(int colorIndex, Renderer renderer, List<Material> materialsList)
        {
            var newMaterial = materialsList[colorIndex];
            renderer.material = newMaterial;
            if (newMaterial == null) { Debug.Log("fuckery "+colorIndex); }

        }
    }
}
