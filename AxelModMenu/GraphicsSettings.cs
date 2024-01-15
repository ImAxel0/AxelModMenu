using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace AxelModMenu;

public class GraphicsSettings
{
    public static void SetTaaSharpness(float value)
    {
        LocalPlayer.MainCam.GetComponent<HDAdditionalCameraData>().taaSharpenStrength = value;
    }

    public static void SetTaaAntiFlicker(float value)
    {
        LocalPlayer.MainCam.GetComponent<HDAdditionalCameraData>().taaAntiFlicker = value;
    }
}
