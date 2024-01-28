using UnityEngine;

namespace Game.Scripts.Behaviours
{
    public class AudienceCharacterBehaviour : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private int totalAnimCount = 5;
        private static int animIndex = 2;

        public void Start()
        {
            //int randomAnimIndex = Random.Range(1, totalAnimCount + 1);
            animator.SetTrigger("Anim" + (animIndex + 1));
            animator.Update(Random.Range(0, 18));

            animIndex = (animIndex + 1) % totalAnimCount;

            if (animIndex == 1)
                transform.Rotate(0, 180f, 0f);
        }
    }
}