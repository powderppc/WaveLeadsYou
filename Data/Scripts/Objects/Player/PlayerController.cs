using PowderPPC.Object.Block;
using PowderPPC.Object.Enemy;
using PowderPPC.System.Sound;
using System.Collections.Generic;
using UnityEngine;

namespace PowderPPC.Object.Player
{
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// プレイヤーの移動速度
        /// </summary>
        [SerializeField] float moveSpeed;
        float nowMoveSpeed;
        ///// <summary>
        ///// プレイヤーにかかる重力
        ///// </summary>
        float gravityScale;
        /// <summary>
        /// ジャンプ力
        /// </summary>
        [SerializeField] float jumpPower;

        /// <summary>
        /// コヨーテタイム
        /// </summary>
        [SerializeField] float coyoteTime;

        /// <summary>
        /// ヒップドロップ発動までの時間
        /// </summary>
        [SerializeField] float readyHipDropTime;

        /// <summary>
        /// ヒップドロップ後の硬直時間
        /// </summary>
        [SerializeField] float afterHipDropTime;

        /// <summary>
        /// ヒップドロップ発動時の落下速度
        /// </summary>
        [SerializeField] float dropSpeed;

        /// <summary>
        /// 標準のドロップ威力
        /// </summary>
        [SerializeField] float dropPowerDefault;

        /// <summary>
        /// 高いところから落下したときのドロップ威力
        /// </summary>
        [SerializeField] float dropPowerSuper;

        /// <summary>
        /// スーパーヒップドロップ発動に必要な時間
        /// </summary>
        [SerializeField] float superDropTime;
        Rigidbody2D myRigidbody;

        public Rigidbody2D MyRigidbody => myRigidbody;

        SpriteRenderer mySprite;
        enum PlayerState
        {
            /// <summary>
            /// 開始数フレームのみこの状態（当たり判定が消えることがある問題の暫定対応）
            /// </summary>
            OnWait,
            /// <summary>
            /// 地面に着いている状態でジャンプが可能
            /// </summary>
            OnGround,
            /// <summary>
            /// コヨーテタイム
            /// （ジャンプもヒップドロップも可能）
            /// </summary>
            DoCoyote,
            /// <summary>
            /// ジャンプ状態（ジャンプは出来ないがヒップドロップは可能）
            /// </summary>
            OnSky,
            /// <summary>
            /// ヒップドロップ開始時のアニメーション
            /// </summary>
            ReadyHipDrop,
            /// <summary>
            /// ヒップドロップ中（地面に着くまで操作不可）
            /// </summary>
            DoHipDrop,
            /// <summary>
            /// ヒップドロップ終了時の硬直
            /// </summary>
            AfterHipDrop,
            /// <summary>
            /// 死亡演出諸々のトリガーステート
            /// </summary>
            Dying,
            /// <summary>
            /// 死亡状態
            /// </summary>
            Death,
            /// <summary>
            /// ステージクリア演出諸々のトリガーステート
            /// （Cleared以外のステートに遷移しない）
            /// </summary>
            Clearing,
            /// <summary>
            /// クリア状態（他のステートに遷移しない）
            /// </summary>
            Cleared,
        }

        PlayerState state;

        /// <summary>
        /// 各ステートでの経過時間
        /// </summary>
        float stateTime;

        public bool IsDied => state == PlayerState.Death;

        /// <summary>
        /// 画面内にいるかどうか
        /// </summary>
        private bool isOnCamera = true;

        /// <summary>
        /// 自身の入力処理の管理場所
        /// </summary>
        PlayerInputController myInput;

        enum PlayerAngle
        {
            Right,
            Left,
        }
        PlayerAngle playerAngle;

        /// <summary>
        /// プレイヤーの向きの符号（1：← -1：→）
        /// </summary>
        public float PlayerAngleToSign => playerAngle == PlayerAngle.Right ? -1 : 1;

        /// <summary>
        /// 入力中にジャンプ、ヒップドロップが発生した場合はTrueになり、
        /// 一度キーを離さないとヒップドロップが発動できない
        /// </summary>
        bool isInputJumped;

        [SerializeField] ParticleSystem brokenParticle;
        [SerializeField] List<ParticleSystem> hipdropImpactParticles;
        [SerializeField] ParticleSystem superHipdropTrailParticle;

        private void Awake()
        {
            myRigidbody = GetComponent<Rigidbody2D>();
            mySprite=GetComponent<SpriteRenderer>();
        }

        // Use this for initialization
        void Start()
        {
            state= PlayerState.OnWait;
            stateTime = 0;
            playerAngle= PlayerAngle.Right;
            gravityScale = myRigidbody.gravityScale;
            superHipdropTrailParticle.Stop();

            myInput = new PlayerInputController();
        }

        // Update is called once per frame
        void Update()
        {
            stateTime += Time.deltaTime;
            transform.rotation = new Quaternion();

            //移動の入力
            InputMove();

            //ジャンプ
            TryJump();

            //ヒップドロップ
            TryHipDrop();

            //画面外判定
            CameraOutUpdate();

            //固有ステート
            switch (state)
            {
                case PlayerState.OnWait:
                    if (stateTime > 0.2f)
                    {
                        myRigidbody.velocity = Vector3.zero;
                        ChangeState(PlayerState.OnSky);
                    }
                    break;
                case PlayerState.OnGround:
                    break;
                case PlayerState.DoCoyote:
                    UpdateCoyote();
                    break;
                case PlayerState.OnSky:
                    break;
                case PlayerState.ReadyHipDrop:
                    ReadyHipDrop();
                    break;
                case PlayerState.DoHipDrop:
                    DoHipDrop();
                    break;
                case PlayerState.AfterHipDrop:
                    AfterHipDrop();
                    break;
                case PlayerState.Dying:
                    DyingUpdate();
                    break;
                case PlayerState.Death:
                    DeathUpdate();
                    break;
                case PlayerState.Clearing:
                    ClearingUpdate();
                    break;
                case PlayerState.Cleared:
                    ClearedUpdate();
                    break;
                default:
                    break;
            }

        }

        private void FixedUpdate()
        {
            //移動入力の反映
            FixedMoveUpdateByInput();

        }

        private void InputMove()
        {
            switch (state)
            {
                case PlayerState.OnGround:
                case PlayerState.DoCoyote:
                case PlayerState.OnSky:
                    nowMoveSpeed = myInput.TryGetMoveSpeedRate();
                    
                    nowMoveSpeed *= moveSpeed;// * Time.deltaTime;

                    //向き変更
                    if (nowMoveSpeed < 0)
                    {
                        playerAngle = PlayerAngle.Left;
                    }
                    else if (nowMoveSpeed > 0)
                    {
                        playerAngle = PlayerAngle.Right;
                    }

                    break;
                default:
                    break;
            }

        }

        private void FixedMoveUpdateByInput()
        {
            switch (state)
            {
                case PlayerState.OnGround:
                case PlayerState.DoCoyote:
                case PlayerState.OnSky:
                    var v = myRigidbody.velocity;
                    v.x = nowMoveSpeed;
                    myRigidbody.velocity = v;

                    break;
                default:
                    break;
            }
        }

        private void UpdateCoyote()
        {
            if (stateTime > coyoteTime)
            {
                ChangeState(PlayerState.OnSky);
            }
        }

        private void TryJump()
        {
            switch (state)
            {
                case PlayerState.OnGround:
                case PlayerState.DoCoyote:
                    if (myInput.TryGetJumpDown())
                    {
                        DoJump();
                    }
                    break;
                default:
                    break;
            }
            //ジャンプボタンを一度離しているか（ヒップドロップ発動に必要）
            if (!myInput.TryGetJump())
            {
                isInputJumped = false;

            }
        }

        /// <summary>
        /// Y軸の速度をリセットしたうえでジャンプ力を付与する
        /// </summary>
        private void DoJump()
        {
            var v = myRigidbody.velocity;
            v.y = jumpPower;
            myRigidbody.velocity = v;
            ChangeState(PlayerState.OnSky);
            isInputJumped = true;

            SoundManager.Instance.SoundJump();
        }

        private void TryHipDrop()
        {
            switch (state)
            {
                case PlayerState.DoCoyote:
                case PlayerState.OnSky:
                    //ずっと押しっぱなしの場合は何もしない
                    if (isInputJumped)
                    {
                        return;
                    }
                    if (myInput.TryGetHipdrop())
                    {
                        ChangeState(PlayerState.ReadyHipDrop);
                        isInputJumped = true;
                        SoundManager.Instance.SoundHipdrop();
                    }

                    break;
                default:
                    break;
            }
        }

        private void ReadyHipDrop()
        {
            myRigidbody.velocity = Vector3.zero;
            myRigidbody.gravityScale = 0;

            var rota = Mathf.Min(1, stateTime / readyHipDropTime) * 360 * PlayerAngleToSign;
            transform.rotation = new Quaternion();
            transform.Rotate(0, 0, rota);

            if (stateTime > readyHipDropTime)
            {
                ToHipDrop();
            }
        }

        private void ToHipDrop()
        {
            ChangeState(PlayerState.DoHipDrop);
            var v = myRigidbody.velocity;
            v.y = -dropSpeed;
            myRigidbody.velocity = v;
            myRigidbody.gravityScale = gravityScale;

        }

        private void DoHipDrop()
        {
            //一定時間落下し続けたらスーパーヒップドロップ状態に移行
            if (stateTime > superDropTime && !superHipdropTrailParticle.isPlaying)
            {
                superHipdropTrailParticle.Emit(1);
                superHipdropTrailParticle.Play();
                SoundManager.Instance.SoundToSuperHipdrop();
            }
        }

        /// <summary>
        /// ヒップドロップによって地形に触れたときの処理
        /// </summary>
        private void DoneDrop()
        {
            ChangeState(PlayerState.AfterHipDrop);
            //エフェクト生成
            foreach(var p in hipdropImpactParticles)
            {
                p.Emit(10);
            }
            superHipdropTrailParticle.Stop();
            //着地音作成
            SoundManager.Instance.SoundDoneHipdrop();
        }
        private void AfterHipDrop()
        {
            if (stateTime > afterHipDropTime)
            {
                ChangeState(PlayerState.OnGround);
            }
        }

        /// <summary>
        /// クリア判定になっていない場合は死亡状態にする
        /// </summary>
        public void TryDying()
        {
            switch (state)
            {
                //特定のステートの場合は何もしない
                case PlayerState.Dying:
                case PlayerState.Death:
                case PlayerState.Clearing:
                case PlayerState.Cleared:
                    return;
                default:
                    break;
            }
            ChangeState(PlayerState.Dying);

        }

        private void DyingUpdate()
        {
            ChangeState(PlayerState.Death);
            myRigidbody.velocity = Vector2.zero;
            myRigidbody.bodyType = RigidbodyType2D.Kinematic;
            var collider = GetComponent<BoxCollider2D>();
            collider.enabled = false;
            brokenParticle.Emit(30);
            SoundManager.Instance.SoundHitPlayer();
        }

        private void DeathUpdate()
        {
            var color = mySprite.color;
            color.a = Mathf.Max(0, 1 - stateTime * 4);
            mySprite.color = color;

        }

        private void ClearingUpdate()
        {
            ChangeState(PlayerState.Cleared);
        }

        private void ClearedUpdate()
        {
            myRigidbody.velocity = Vector2.zero;
            myRigidbody.gravityScale = 0;

            var rota = stateTime / readyHipDropTime * 360 * PlayerAngleToSign;
            transform.rotation = new Quaternion();
            transform.Rotate(0, 0, rota);
        }

        /// <summary>
        /// 自身のステートを更新する
        /// </summary>
        /// <param name="state"></param>
        /// <param name="isOverride">Trueの時、同じステートだった場合も実行する</param>
        private void ChangeState(PlayerState state, bool isOverride = false)
        {
            var old = this.state;
            //上書き設定ではない、かつ同じステートだった場合は何もしない
            if(!isOverride && old == state)
            {
                return;
            }

            this.state = state;
            stateTime = 0;

        }

        public void ForceGoal()
        {
            ChangeState(PlayerState.Clearing);
        }

        /// <summary>
        /// プレイヤーの接触状態を地上判定に変更する（Trueの場合成功）
        /// </summary>
        public bool TryChangeStateOnGroundByBlock()
        {
            switch (this.state)
            {
                case PlayerState.OnGround:
                case PlayerState.AfterHipDrop:
                    return true;
                //case PlayerState.DoCoyote:
                case PlayerState.OnSky:
                    ChangeState(PlayerState.OnGround);
                    return true;
                default:
                    break;
            }
            return false;
        }

        /// <summary>
        /// プレイヤーの接触状態を空中判定に変更する（Trueの場合成功）
        /// </summary>
        public bool TryChangeStateOnSkyByBlock()
        {
            switch (this.state)
            {
                case PlayerState.OnGround:
                    ChangeState(PlayerState.DoCoyote);
                    return true;
                case PlayerState.OnSky:
                case PlayerState.DoCoyote:
                    return true;
                default:
                    break;
            }
            return false;
        }

        public bool IsPlayerOnSky => state == PlayerState.OnSky;

        // 衝突判定
        #region

        private void OnCollisionEnter2D(Collision2D collision)
        {

            //敵などに触れた場合は死亡する
            if (collision.gameObject.TryGetComponent<DangerObjController>(out var _))
            {
                TryDying();
            }
            //RaycastLaserなどは敵側で判定処理させる

        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<BlockController>(out var obj))
            {
                //空中ニュートラル状態で地形に接触した場合は地上判定とする
                //BlockManagerで管理

                //ヒップドロップモーションの状態で地形に接触した場合
                //ヒップドロップ完了処理とブロックに波を与える処理
                if (state == PlayerState.DoHipDrop)
                {
                    var power = stateTime > superDropTime ? dropPowerSuper : dropPowerDefault;
                    var param = new VibrationParameter() { amplitude = power };
                    obj.AddForce(param);

                    DoneDrop();
                }

            }
            if (collision.gameObject.TryGetComponent<BGMVolumeChanger>(out var bgm))
            {
                TryChangeStateOnGroundByBlock();
                //ヒップドロップモーションの状態で地形に接触した場合
                //ヒップドロップ完了処理とブロックに波を与える処理
                if (state == PlayerState.DoHipDrop)
                {
                    bgm.ChangeVolume();
                    DoneDrop();
                }
            }
            if (collision.gameObject.TryGetComponent<SEVolumeChanger>(out var se))
            {
                TryChangeStateOnGroundByBlock();
                //ヒップドロップモーションの状態で地形に接触した場合
                //ヒップドロップ完了処理とブロックに波を与える処理
                if (state == PlayerState.DoHipDrop)
                {
                    se.ChangeVolume();
                    DoneDrop();
                }
            }

        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<BGMVolumeChanger>(out var bgm))
            {
                TryChangeStateOnSkyByBlock();
            }
            if (collision.gameObject.TryGetComponent<SEVolumeChanger>(out var se))
            {
                TryChangeStateOnSkyByBlock();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //敵などに触れた場合は死亡する
            if (collision.gameObject.TryGetComponent<DangerObjController>(out var _))
            {
                TryDying();
            }
            //RaycastLaserなどは敵側で判定処理させる
        }

        /// <summary>
        /// 画面外落下による死亡判定
        /// </summary>
        private void CameraOutUpdate()
        {
            if (!isOnCamera)
            {
                TryDying();
            }
            isOnCamera = false;
        }

        #endregion

        //画面内か判定
        void OnWillRenderObject()
        {
            if (Camera.current.name == "Main Camera")
            {
                isOnCamera = true;
            }

        }
    }
}