using PowderPPC.Object.Player;
using UnityEngine;

namespace PowderPPC.Object.Stage
{
    public class GoalKeyController : MonoBehaviour
    {
        [SerializeField] GoalController goalObj;

        [SerializeField] ParticleSystem shineParticle;
        [SerializeField] ParticleSystem pickupParticle;

        [SerializeField] SpriteRenderer mySprite;
        [SerializeField] SpriteRenderer keyLaserSprite;

        bool isUnlocked;
        float timer;
        [SerializeField] float shotTime;

        // Use this for initialization
        void Start()
        {
            goalObj?.Lock();
            isUnlocked = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (isUnlocked)
            {
                timer += Time.deltaTime;
                var rate = timer / shotTime;

                if (rate > 1)
                {
                    keyLaserSprite.gameObject.SetActive(true);
                    return;
                }

                var color = keyLaserSprite.color;
                color.a = 1 - rate;
                keyLaserSprite.color = color;

                var size = keyLaserSprite.transform.localScale;
                size.y = 1 - rate;
                keyLaserSprite.transform.localScale = size;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.TryGetComponent<PlayerController>(out var p))
            {
                //プレイヤーが触れたらゴールをアンロック＆自身を非アクティブ化
                goalObj.UnLock();
                pickupParticle.Emit(10);

                shineParticle.Stop();
                var collider = GetComponent<BoxCollider2D>();
                collider.enabled = false;
                mySprite.enabled = false;

                ShotLaser();
                //gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 鍵取得時にゴールに向けてレーザーを放つ
        /// </summary>
        void ShotLaser()
        {
            isUnlocked = true;
            timer = 0;

            var diff = goalObj.transform.position - transform.position;
            var angle = Mathf.Atan2(diff.y, diff.x);
            var rota = angle * Mathf.Rad2Deg;
            var length = diff.magnitude;

            keyLaserSprite.transform.rotation = new Quaternion();
            keyLaserSprite.transform.Rotate(new Vector3(0, 0, rota));

            var size = keyLaserSprite.transform.localScale;
            size.x = length * 2;
            keyLaserSprite.transform.localScale = size;

            keyLaserSprite.gameObject.SetActive(true);

        }
    }
}