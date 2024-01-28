using UnityEngine;

namespace Game.Scripts.Behaviours
{
    public class AudienceCharacterBehaviour : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private int totalAnimCount = 2;

        public void Start()
        {
            int randomAnimIndex = Random.Range(1, totalAnimCount + 1);
            animator.SetTrigger("Anim" + randomAnimIndex);
            
            if(randomAnimIndex == 1)
                transform.Rotate(0, 180f, 0f);
        }
    }
}
