using UnityEngine;
using UnityEngine.Events;

public class TouchAudioAnimatorConnector : MonoBehaviour
{
    public UnityEvent<AnimationEvent> FinishLastClipEvent = new UnityEvent<AnimationEvent>();

    private void OnFinishLastClipCompleted(AnimationEvent clip)
    {
        FinishLastClipEvent?.Invoke(clip);
    }
}