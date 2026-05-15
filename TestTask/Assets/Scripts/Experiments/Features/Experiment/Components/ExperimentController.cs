using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Features.Localization.Components;
using Features.Localization.Models.Enums;
using UnityEngine;

namespace Features.Experiment.Components
{
    public class ExperimentController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private ContinueButton continueButton;
        [SerializeField] private List<ExperimentState> states;

        private int _currentStateIndex;

        private void Awake()
        {
            SubscribeEvents();
        }

        private void OnDestroy() => UnsubscribeEvents();

        private void Start()
        {
            RunNextState();
        }

        private async void RunNextState()
        {
            if (_currentStateIndex >= states.Count)
            {
                Debug.LogError($"Current state index is bigger than states amount");
                return;
            }

            ExperimentState currentState = states[_currentStateIndex];
            currentState.PlayAudios();
            animator.Play(currentState.AnimatorStateName);

            float audioLenght = currentState.GetPlayingLenght();
            
            await WaitForCompletion(audioLenght);

            _currentStateIndex++;
            continueButton.EnableButton();
        }
        
        private async Task WaitForCompletion(float audioLenght)
        {
            await Task.Yield();
            
            float stateDuration = GetCurrentStateDuration();
            float maxDuration = Mathf.Max(audioLenght, stateDuration);

            await Task.Delay((int)(maxDuration * 1000));
        }
        
        private float GetCurrentStateDuration()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.length;
        }
        
        private void SubscribeEvents() => continueButton.OnContinueButtonClick += RunNextState;

        private void UnsubscribeEvents() => continueButton.OnContinueButtonClick -= RunNextState;
    }
}