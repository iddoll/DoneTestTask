using System.Collections;
using Features.Appearance.Components;
using UnityEngine;

namespace Experiments.Features.TransformAnimator.Components
{
    public class ObjectSelectAnimator : MonoBehaviour
    {
        [SerializeField] private ButtonPressEffect ButtonPressEffect;
        [SerializeField] private TransformAnimator TransformAnimator;
        [SerializeField] private AppearanceComponent[] objectsToDisable;
        [SerializeField] private float DisableDelay = 1f;
        [SerializeField] private float AfterSelectDelay = 0.2f;
        [SerializeField] private float AfterDisableSequenceDelay = 0.2f;
        [SerializeField] private float AfterTransformAnimationDelay = 0.2f;
        [SerializeField] private float cooldownTime = 3f;

        private bool _isSelected;
        private bool _isOnCooldown = false;


        public void Select()
        {
            StartCoroutine(SelectSequence());
        }

        IEnumerator SelectSequence()
        {
            if (!_isOnCooldown)
            {
                if (!_isSelected)
                {
                    StartCoroutine(CooldownRoutine());
                    ButtonPressEffect.Press();
                    yield return new WaitForSeconds(AfterSelectDelay);

                    StartCoroutine(DisableObjectsSequentially(_isSelected));
                    yield return new WaitForSeconds(AfterDisableSequenceDelay);
                
                    TransformAnimator.StartAnimation();
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    StartCoroutine(CooldownRoutine());
                    ButtonPressEffect.Press();
                    yield return new WaitForSeconds(AfterSelectDelay);
                
                    TransformAnimator.StartAnimation();
                    yield return new WaitForSeconds(AfterTransformAnimationDelay);
                
                    StartCoroutine(DisableObjectsSequentially(_isSelected));
                    yield return new WaitForSeconds(AfterDisableSequenceDelay);
                }
                yield return new WaitForSeconds(1);

                _isSelected = !_isSelected;
            }
        
        }
        IEnumerator DisableObjectsSequentially(bool stateFlag)
        {
            foreach (AppearanceComponent obj in objectsToDisable)
            {
                if (obj != null)
                {
                    if(_isSelected==false)obj.ScaleOut();
                    else obj.ScaleIn();
                    yield return new WaitForSeconds(DisableDelay);
                }
            }
        }
        private IEnumerator CooldownRoutine()
        {
            _isOnCooldown = true;
            yield return new WaitForSeconds(cooldownTime);
            _isOnCooldown = false;
        }
    }
}
