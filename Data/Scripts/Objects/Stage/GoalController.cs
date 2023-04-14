using PowderPPC.Object.Block;
using PowderPPC.Object.Others;
using PowderPPC.Object.Player;
using PowderPPC.System.Sound;
using UnityEngine;

namespace PowderPPC.Object.Stage
{
    public class GoalController : MonoBehaviour
    {

        /// <summary>
        /// ゴールに鍵がかかっているかどうか
        /// </summary>
        private bool isLock;

        /// <summary>
        /// ゴールしたかどうか
        /// </summary>
        public bool IsGoaled { get; private set; }

        [SerializeField] SpriteRenderer lockSprite;

        [SerializeField] ExplosionController explodeObj;

        [SerializeField] SpriteRenderer mySprite;
        public SpriteRenderer Mysprite => mySprite;

        /// <summary>
        /// ゴール可能状態のパーティクル
        /// </summary>
        [SerializeField] ParticleSystem goalParticle;
        /// <summary>
        /// ロック状態のパーティクル
        /// </summary>
        [SerializeField] ParticleSystem lockedParticle;
        /// <summary>
        /// ロックを解除するときのパーティクル
        /// </summary>
        [SerializeField] ParticleSystem unlockParticle;
        void Start()
        {
            IsGoaled = false;
            UnLock(false);

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Lock()
        {
            isLock = true;
            lockSprite.gameObject.SetActive(true);
            lockedParticle.gameObject.SetActive(true);
            goalParticle.gameObject.SetActive(false);
        }

        public void UnLock(bool isUnlockEffectEmit = true)
        {
            isLock = false;
            lockSprite.gameObject.SetActive(false);
            lockedParticle.gameObject.SetActive(false);
            goalParticle.gameObject.SetActive(true);
            if (isUnlockEffectEmit)
            {
                unlockParticle.Emit(20);
                SoundManager.Instance.SoundUnlockGoal();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.TryGetComponent<PlayerController>(out var p))
            {
                TryGoal();
            }
            else if(collision.gameObject.TryGetComponent<BlockController>(out var b))
            {
                TryGoal();
            }
        }

        public void TryGoal()
        {
            if (isLock)
            {
                SoundManager.Instance.SoundGoalFailed();
                return;
            }

            if (IsGoaled)
            {
                return;
            }

            IsGoaled = true;
            explodeObj.gameObject.SetActive(true);
            SoundManager.Instance.SoundGoal();
            return;
        }
    }
}