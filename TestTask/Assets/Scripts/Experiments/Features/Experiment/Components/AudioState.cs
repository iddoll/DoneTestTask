using System.Threading.Tasks;
using UnityEngine;

namespace Features.Experiment.Components
{
    [System.Serializable]
    public class AudioState
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float secondToPlay;
        
        public AudioSource AudioSource => audioSource;
        public float SecondToPlay => secondToPlay;

        public async void Play()
        {
            if (audioSource == null)
                return;
            
            await Task.Delay((int)secondToPlay * 1000);
            audioSource.Play();
        }

        public float GetPlayingLenght()
        {
            if (audioSource == null)
                return 0f;
            
            return secondToPlay + audioSource.clip.length;
        }
    }
}