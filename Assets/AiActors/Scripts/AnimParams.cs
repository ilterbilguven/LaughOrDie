using UnityEngine;

namespace AiActors.Scripts
{
    public static class AnimParams
    {
        public static readonly int Speed = Animator.StringToHash("speed");
        public static readonly int Right = Animator.StringToHash("right");
        public static readonly int Forward = Animator.StringToHash("forward");
    }
}