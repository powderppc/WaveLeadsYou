using UnityEngine;

namespace PowderPPC.Object.Others
{
    /// <summary>
    /// 爆風クラス
    /// アクティブ状態になると時間経過で爆風が広がる。
    /// 被弾判定がついている場合、プレイヤーは死ぬ。
    /// </summary>
    public class ExplosionController : MonoBehaviour
    {
        /// <summary>
        /// 爆風が広がっていく速度
        /// </summary>
        [SerializeField] float expandSpeed;
        /// <summary>
        /// 爆風の色が薄まっていく速度(1/value秒)
        /// </summary>
        [SerializeField] float colorLightSped;
        float timer;

        SpriteRenderer mySprite;

        private void Awake()
        {
            mySprite = GetComponent<SpriteRenderer>();
        }

        // Use this for initialization
        void Start()
        {
            timer = 0;
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;

            //時間経過とともに自信を拡大する
            var size = transform.localScale;
            size.x += expandSpeed * timer;
            size.y += expandSpeed * timer;
            transform.localScale = size;

            var color = mySprite.color;
            color.a = Mathf.Max(0, 1 - timer * colorLightSped);
            mySprite.color = color;

            if (color.a == 0)
            {
                EndExplode();
            }
        }

        /// <summary>
        /// 爆発演出を終わらせ、再度演出を出せるようにしておく
        /// </summary>
        void EndExplode()
        {
            gameObject.SetActive(false);
            timer = 0;
            transform.localScale = new Vector2(0.1f, 0.1f);
            var color = mySprite.color;
            color.a = 1;
            mySprite.color = color;
        }
    }
}