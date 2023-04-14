using PowderPPC.Object.Block;
using PowderPPC.Object.Others;
using PowderPPC.Object.Player;
using PowderPPC.System.Sound;
using UnityEngine;

namespace PowderPPC.Object.Enemy
{
    /// <summary>
    /// ボム
    /// プレイヤーやブロックなど他のオブジェクトが触れると全画面に爆風を起こし、プレイヤーが死ぬ
    /// </summary>
    public class BombController : MonoBehaviour
    {

        [SerializeField] ExplosionController explodeObj;

        [SerializeField] GameObject myBody;

        // Use this for initialization
        void Start()
        {
            explodeObj.gameObject.SetActive(false);

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// オブジェクトが触れた場合に発動
        /// 全画面に爆風を起こす
        /// </summary>
        public void Explode()
        {
            explodeObj.gameObject.SetActive(true);
            myBody.SetActive(false);
            var collider = GetComponent<CircleCollider2D>();
            collider.enabled = false;
            SoundManager.Instance.SoundExplodeBomb();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent<PlayerController>(out var _))
            {
                Explode();
            }
            else if (collision.gameObject.TryGetComponent<BlockController>(out var _))
            {
                Explode();
            }
        }

    }
}