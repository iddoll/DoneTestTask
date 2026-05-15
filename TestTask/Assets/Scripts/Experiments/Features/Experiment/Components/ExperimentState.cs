using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Features.Experiment.Components
{
    [System.Serializable]
    public class ExperimentState
    {
        [SerializeField] private string animatorStateName;
        [SerializeField] private List<AudioState> audioStates;
        
        public List<AudioState> AudioStates => audioStates;
        public string AnimatorStateName => animatorStateName;

        public void PlayAudios()
        {
            if (AudioStates.Count == 0)
                return;

            foreach (var audioState in audioStates) 
                PlayAudio(audioState);
        }

        private async Task PlayAudio(AudioState audioState)
        {
            await Task.Delay((int) audioState.SecondToPlay * 1000);
            
            audioState.AudioSource.Play();
        }

        public float GetPlayingLenght()
        {
            if (AudioStates.Count == 0)
                return 0f;

            var lastAudio = audioStates.LastOrDefault();
            if (lastAudio == null)
                return 0f;
            
            return lastAudio.SecondToPlay + lastAudio.AudioSource.clip.length;
        }
    }
}