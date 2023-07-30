using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public Player player;
    public void FootstepSound()
    {
        EventHandler.CallPlaySoundEvent(player.isRunning ? SoundName.FootStepHard : SoundName.FootStepSoft);
    }
}
