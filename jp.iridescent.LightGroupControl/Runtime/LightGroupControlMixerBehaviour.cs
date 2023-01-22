using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if USE_HDRP
using UnityEngine.Rendering.HighDefinition;
#endif

namespace Iridescent.LightGroupControl
{


    internal struct LightParams
    {
        public Color DefaultColor;
        public float DefaultIntensity;
        public float DefaultBounceIntensity;
        public float DefaultRange;
    }

    public class LightGroupControlMixerBehaviour : PlayableBehaviour
    {



        public List<LightValue> tmpLightValues = new List<LightValue>();

#if USE_HDRP
        private Dictionary<Light,HDAdditionalLightData> additionalLightDataDict = new Dictionary<Light, HDAdditionalLightData>();

#endif
        public LightGroup m_TrackBinding;
        // public List<Light> lights = new List<Light>();
        // private Dictionary<Light,LightParams> lightParamsList = new Dictionary<Light, LightParams>();

        // Light m_TrackBinding;
        bool m_FirstFrameHappened;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            m_TrackBinding = playerData as LightGroup;
            //
            if (m_TrackBinding == null)
                return;
            var bindingLightValues = m_TrackBinding.lightValues;
            if (!m_FirstFrameHappened)
            {
      
                InitHDAdditionalLightData(m_TrackBinding.lights);
                m_FirstFrameHappened = true;
            }

            int inputCount = playable.GetInputCount();




            float totalWeight = 0f;
            float greatestWeight = 0f;
            int currentInputs = 0;


            if (tmpLightValues.Count != bindingLightValues.Count)
            {
                if (tmpLightValues.Count > bindingLightValues.Count)
                {
                    tmpLightValues.RemoveRange(bindingLightValues.Count, tmpLightValues.Count - bindingLightValues.Count);
                }
                else
                {
                    for (int i = tmpLightValues.Count; i < bindingLightValues.Count; i++)
                    {
                        tmpLightValues.Add(new LightValue());
                    }
                }
            }

            foreach (var tmpLightValue in tmpLightValues)
            {
                tmpLightValue.Reset();
            }
            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<LightGroupControlBehaviour> inputPlayable =
                    (ScriptPlayable<LightGroupControlBehaviour>)playable.GetInput(i);
                LightGroupControlBehaviour input = inputPlayable.GetBehaviour();

                var clipLightValues = input.lightValues;


                if (clipLightValues.Count > bindingLightValues.Count)
                {
                    clipLightValues.RemoveRange(bindingLightValues.Count,
                        clipLightValues.Count - bindingLightValues.Count);
                }
                else if (clipLightValues.Count < bindingLightValues.Count)
                {
                    clipLightValues.AddRange(bindingLightValues.GetRange(clipLightValues.Count,
                        bindingLightValues.Count - clipLightValues.Count));
                }

                var lightIndex = 0;
                foreach (var clipLightValue in clipLightValues)
                {
                    clipLightValue.lightName = m_TrackBinding.lights[lightIndex].gameObject.name;
                    lightIndex++;
                }
                
                for (int j = 0; j < tmpLightValues.Count; j++)
                {
                    tmpLightValues[j].color += clipLightValues[j].color * inputWeight;
                    tmpLightValues[j].intensity += clipLightValues[j].intensity * inputWeight;
                    // tmpLightValues[j].bounceIntensity +=
                    //     clipLightValues[j].bounceIntensity * inputWeight;
                    tmpLightValues[j].range += clipLightValues[j].range * inputWeight;
                }
                
                totalWeight += inputWeight;

                if (inputWeight > greatestWeight)
                {
                    greatestWeight = inputWeight;
                }

                if (!Mathf.Approximately(inputWeight, 0f))
                    currentInputs++;
            }

            var index = 0;
            foreach (var tmpLightValue in tmpLightValues)
            {

                if (index < m_TrackBinding.lightValues.Count)
                {
                    m_TrackBinding.lightValues[index].color = tmpLightValue.color;
                    m_TrackBinding.lightValues[index].intensity = tmpLightValue.intensity;
                    m_TrackBinding.lightValues[index].bounceIntensity = tmpLightValue.bounceIntensity;
                    m_TrackBinding.lightValues[index].range = tmpLightValue.range;
                }
                index++;

            }
            
            
            m_TrackBinding.ApplyLightValues();
            m_TrackBinding.TransferSyncGroup();
           
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            m_FirstFrameHappened = false;

            if (m_TrackBinding == null)
                return;

            // foreach (var lightParams in lightParamsList)
            // {
            //     var light = lightParams.Key;
            //     light.color = lightParams.Value.DefaultColor;
            //     light.intensity = lightParams.Value.DefaultIntensity;
            //     light.bounceIntensity = lightParams.Value.DefaultBounceIntensity;
            //     light.range = lightParams.Value.DefaultRange;
            //     
            // }

        }

       
        private void InitHDAdditionalLightData(List<Light> lights)
        {
#if USE_HDRP     
            additionalLightDataDict.Clear();
            foreach (var light in lights)
            {
                if (!additionalLightDataDict.ContainsKey(light))
                {
                    additionalLightDataDict.Add(light, light.GetComponent<HDAdditionalLightData>());
                }
            }
#endif

        }
    }
    
}