using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace Iridescent.LightGroupControl
{

    [TrackColor(0.9454092f, 0.9779412f, 0.3883002f)]
    [TrackClipType(typeof(LightGroupControlClip))]
    [TrackBindingType(typeof(LightGroup))]
    public class LightGroupControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<LightGroupControlMixerBehaviour>.Create(graph, inputCount);
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            // Light trackBinding = director.GetGenericBinding(this) as Light;
            // if (lights.Count == null)
            // return;
            // foreach (var light in lights)
            // {
            //     driver.AddFromName<Light>(light.gameObject, "m_Color");
            //     driver.AddFromName<Light>(light.gameObject, "m_Intensity");
            //     driver.AddFromName<Light>(light.gameObject, "m_Range");
            //     driver.AddFromName<Light>(light.gameObject, "m_BounceIntensity");        
            // }

#endif
            // base.GatherProperties(director, driver);
        }
    }
}