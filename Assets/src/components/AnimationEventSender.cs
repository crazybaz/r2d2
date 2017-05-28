using UnityEngine;

public class AnimationEventSender : MonoBehaviour
{
    public void AnimationEventHandler(string animationName)
    {
        gameObject.SendMessage(animationName, SendMessageOptions.DontRequireReceiver);
    }
}