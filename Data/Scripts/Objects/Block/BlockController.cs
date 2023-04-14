using PowderPPC.Object.Player;
using PowderPPC.System.Sound;
using System.Collections.Generic;
using UnityEngine;

namespace PowderPPC.Object.Block
{
    public class BlockController : MonoBehaviour
    {
        public LinkedList<VibrationController> VibrationList { get; private set; }

        BlockController leftBlock;
        BlockController rightBlock;

        private LinkedList<VibrationController> deletevibrationlist;

        private Rigidbody2D myRigidbody;
        private BoxCollider2D myCollider;
        private Vector2 defaultColliderSize;
        private Vector2 defaultColliderOffset;
        private Vector2 defaultPos;

        /// <summary>
        /// プレイヤーと接触判定を持っているかどうか
        /// 接触している場合Trueで、FixedUpdateのタイミングでFalseになる
        /// IsPlayerInChildのとき、一定距離以上離れてなければ解除されない
        /// </summary>
        public bool IsStayPlayer { get; private set; }

        /// <summary>
        /// プレイヤーと接触時に自身が上昇状態にあるかどうか
        /// プレイヤーと接触時に上昇状態ならオンになり、非接触時以降に上昇状態でなくなればオフ
        /// </summary>
        public bool IsStayPlayerWithHighPos { get; private set; }
        /// <summary>
        /// 現在の振幅
        /// </summary>
        float amplitudeSum;
        /// <summary>
        /// 直前のフレームの振幅
        /// </summary>
        float prevAmplitudeSum;
        /// <summary>
        /// 自身が頂上付近にいるかどうか
        /// </summary>
        bool isTopPos => amplitudeSum > 0 && Mathf.Abs(amplitudeSum - prevAmplitudeSum) < 0.05f;
        /// <summary>
        /// 自身が上昇状態か
        /// </summary>
        bool isUppingPos => (amplitudeSum > prevAmplitudeSum) || isTopPos;

        private void Awake()
        {
            myRigidbody = GetComponent<Rigidbody2D>();
            myCollider = GetComponent<BoxCollider2D>();
        }

        public void MyInit()
        {
            defaultColliderSize = myCollider.size;
            defaultColliderOffset = myCollider.offset;
            defaultPos = transform.position;
            amplitudeSum = 0;
            prevAmplitudeSum = 0;
        }

        public void MyTimerUpdate(float t)
        {
            foreach (var v in VibrationList)
            {
                v.MyUpdate(t);
            }
        }

        /// <summary>
        /// 自身の振動を伝導させる更新処理
        /// </summary>
        public void MyTransmitUpdate()
        {

            deletevibrationlist = new LinkedList<VibrationController>();

            foreach (var v in VibrationList)
            {

                //伝導処理
                if (v.CanTransmission)
                {
                    DoTranmit(v);
                    v.DoAfterTransmission();
                }

                //振動が終わったやつの削除処理
                if (v.IsEndVibration)
                {
                    deletevibrationlist.AddLast(v);
                }
            }
            //削除項目があった場合は削除する
            foreach (var v in deletevibrationlist)
            {
                VibrationList.Remove(v);
            }
        }

        /// <summary>
        /// 伝導結果を自身の座標に反映させる処理
        /// 最後に結果の絶対値を返す
        /// </summary>
        public float MyPosUpdate()
        {
            prevAmplitudeSum = amplitudeSum;
            amplitudeSum = 0f;
            foreach (var v in VibrationList)
            {
                amplitudeSum += v.AmplitudeNow;
            }

            UpdatePos(amplitudeSum);
            UpdateColliderSize();

            //プレイヤーと非接触かつ自身が下降状態の時に解除
            if(!IsStayPlayer && !isUppingPos)
            {
                IsStayPlayerWithHighPos = false;
            }

            return Mathf.Abs(amplitudeSum);
        }

        /// <summary>
        /// 座標の更新
        /// </summary>
        /// <param name="y"></param>
        private void UpdatePos(float y)
        {
            var pos = defaultPos;
            pos.y += y;
            myRigidbody.MovePosition(pos);
        }

        /// <summary>
        /// 高波が発生したときなどにブロックの間に隙間ができるので判定を下に伸ばす
        /// </summary>
        private void UpdateColliderSize()
        {
            var posCenter = transform.position;
            var yLeft = leftBlock?.transform.position.y ?? posCenter.y;
            var yRight = rightBlock?.transform.position.y ?? posCenter.y;

            var minY = Mathf.Min(yLeft, yRight);
            //両隣の座標が自身より低かった場合は判定を伸ばす
            if (posCenter.y > minY)
            {
                var diff = posCenter.y - minY;
                var size = myCollider.size;
                size.y = diff * 2 + defaultColliderSize.y;
                myCollider.size = size;

                var offset = myCollider.offset;
                offset.y = -size.y / 2;
                myCollider.offset = offset;
            }
            else
            {
                myCollider.size = defaultColliderSize;
                myCollider.offset = defaultColliderOffset;
            }
        }
        /// <summary>
        /// 隣のブロックに振動を伝導
        /// </summary>
        /// <param name="vibration"></param>
        private void DoTranmit(VibrationController vibration)
        {
            switch (vibration.MyForce)
            {
                case VibrationController.ForceFrom.Center:
                    //両端に伝導する
                    leftBlock?.VibrationList.AddLast(new VibrationController(vibration, VibrationController.ForceFrom.Right));
                    rightBlock?.VibrationList.AddLast(new VibrationController(vibration, VibrationController.ForceFrom.Left));
                    break;
                case VibrationController.ForceFrom.Right:
                    //左に伝導する
                    leftBlock?.VibrationList.AddLast(new VibrationController(vibration, VibrationController.ForceFrom.Right));
                    break;
                case VibrationController.ForceFrom.Left:
                    //右に伝導する
                    rightBlock?.VibrationList.AddLast(new VibrationController(vibration, VibrationController.ForceFrom.Left));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// ブロックの初期化処理（連結処理）
        /// </summary>
        /// <param name="leftObj"></param>
        /// <param name="rightObj"></param>
        public void Init(BlockController leftObj, BlockController rightObj)
        {
            leftBlock = leftObj;
            rightBlock = rightObj;
            VibrationList = new LinkedList<VibrationController>();
        }

        /// <summary>
        /// 外部から自身に振動を追加する
        /// </summary>
        /// <param name="param">コンストラクタで事前にパラメータを決めておく</param>
        public void AddForce(VibrationParameter param)
        {
            var vibration = new VibrationController(
                param.amplitude,
                param.period,
                param.transmissionTime
                );
            VibrationList.AddLast(vibration);

            SoundManager.Instance.SoundHitBlock();
        }



        //衝突判定
        #region 
        private void OnCollisionStay2D(Collision2D collision)
        {
            if(collision.gameObject.TryGetComponent<PlayerController>(out var p))
            {
                IsStayPlayer = true;
                IsStayPlayerWithHighPos = isUppingPos;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<PlayerController>(out var p))
            {
                IsStayPlayer = false;
            }
        }

        #endregion
    }
}

