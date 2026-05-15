using UnityEngine;

namespace Features.Choice.Components
{
    public class WrongChoiceComponent : ChoiceComponent
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string animationName;
        
        [SerializeField] private AudioSource audioSource;

        private bool _inProgress;

        private void OnMouseDown() => ShowWrongChoice();

        public void ShowWrongChoice()
        {
            if (InProgress())
                return;

            if (animator != null)
                animator.Play(animationName);

            if (audioSource != null)
                audioSource.Play();
        }

        private bool InProgress()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return audioSource.isPlaying || stateInfo.IsName(animationName);
        }
    }
}