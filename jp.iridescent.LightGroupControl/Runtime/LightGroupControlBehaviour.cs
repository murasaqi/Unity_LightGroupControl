using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Iridescent.LightGroupControl
{
[Serializable]
public class LightGroupControlBehaviour : PlayableBehaviour
{
    // public Color color = Color.white;
    // public float intensity = 1f;
    // public float bounceIntensity = 1f;
    // public float range = 10f;
    public List<LightValue> lightValues = new List<LightValue>();
}
}