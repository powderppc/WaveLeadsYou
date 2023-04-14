using UnityEngine;

namespace PowderPPC.Object.Others
{
    /// <summary>
    /// プレイヤーが取得可能なオブジェクトにアタッチするクラス
    /// Spriteのみを持つオブジェクトにアタッチし、親の当たり判定はそのままに自身を上下に揺らす。
    /// </summary>
    public class PickuppableObjController : MonoBehaviour
    {
        /// <summary>
        /// 振幅
        /// </summary>
        [SerializeField] float amplitude;
        /// <summary>
        /// 周期
        /// </summary>
        [SerializeField] float priod;

        Vector2 initPos;

        private float timer;
        private float period_rate { get { return timer / priod; } }

        /// <summary>
        /// 現在の振れ幅
        /// </summary>
        private float amplitudeNow => amplitude * Mathf.Sin(period_rate * Mathf.PI * 2);


        // Use this for initialization
        void Start()
        {
            initPos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;

            var pos = initPos;
            pos.y += amplitudeNow;
            transform.position = pos;
        }
    }
}