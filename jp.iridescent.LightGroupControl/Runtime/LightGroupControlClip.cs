using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Iridescent.LightGroupControl
{

    [Serializable]
    public class LightGroupControlClip : PlayableAsset, ITimelineClipAsset
    {
        public LightGroupControlBehaviour template = new LightGroupControlBehaviour();

        // public List<LightValue> lightValues;
        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<LightGroupControlBehaviour>.Create(graph, template);
            return playable;
        }
    }
}