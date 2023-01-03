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



        public Dictionary<Light, LightValue> lightValueDictionary = new Dictionary<Light, LightValue>();

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


            if (lightValueDictionary.Count != bindingLightValues.Count)
            {
                lightValueDictionary.Clear();
                foreach (var light in m_TrackBinding.lights)
                {
                    lightValueDictionary.Add(light, new LightValue());
                }
            }

            if (lightValueDictionary.Count != additionalLightDataDict.Count)
            {

                InitHDAdditionalLightData(m_TrackBinding.lights);          
            }

            foreach (var lightValue in lightValueDictionary)
            {
                lightValue.Value.Reset();
            }


            float totalWeight = 0f;
            float greatestWeight = 0f;
            int currentInputs = 0;

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



                for (int j = 0; j < lightValueDictionary.Count; j++)
                {
                    lightValueDictionary.ElementAt(j).Value.color += clipLightValues[j].color * inputWeight;
                    lightValueDictionary.ElementAt(j).Value.intensity += clipLightValues[j].intensity * inputWeight;
                    lightValueDictionary.ElementAt(j).Value.bounceIntensity +=
                        clipLightValues[j].bounceIntensity * inputWeight;
                    lightValueDictionary.ElementAt(j).Value.range += clipLightValues[j].range * inputWeight;
                }


                totalWeight += inputWeight;

                if (inputWeight > greatestWeight)
                {
                    greatestWeight = inputWeight;
                }

                if (!Mathf.Approximately(inputWeight, 0f))
                    currentInputs++;
            }


            foreach (var lightValue in lightValueDictionary)
            {
                lightValue.Key.color = lightValue.Value.color;
                lightValue.Key.intensity = lightValue.Value.intensity;
                lightValue.Key.bounceIntensity = lightValue.Value.bounceIntensity;
                lightValue.Key.range = lightValue.Value.range;
                
                #if USE_HDRP
                if (additionalLightDataDict.ContainsKey(lightValue.Key))
                {
                    additionalLightDataDict[lightValue.Key].intensity=lightValue.Value.intensity;
                    // additionalLightDataDict[lightValue.Key].range =lightValue.Value.range;
                }
                #endif
            }


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