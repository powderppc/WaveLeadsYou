using PowderPPC.Object.Block;
using PowderPPC.Object.Player;
using PowderPPC.Object.Stage;
using PowderPPC.System.Sound;
using UnityEngine;

namespace PowderPPC.Object.Enemy
{
    /// <summary>
    /// 一定周期でRaycastを飛ばしてプレイヤーやブロックに衝撃を与える
    /// </summary>
    public class GunController : MonoBehaviour
    {
        [SerializeField] RaycastLaserController laserController;
        [SerializeField] float maxLaserDistance;

        /// <summary>
        /// 周期ずらし用の初回の追加待機時間
        /// </summary>
        [SerializeField] float initWaitTime;
        /// <summary>
        /// 待機時間
        /// </summary>
        [SerializeField] float waitTime;
        /// <summary>
        /// 照射猶予時間（レーザーは出力する）
        /// </summary>
        [SerializeField] float noticeTime;

        /// <summary>
        /// ブロックに与える衝撃の強さ
        /// </summary>
        [SerializeField] float power;

        [SerializeField] ParticleSystem shotOrbitParticle;

        [SerializeField] ParticleSystem shotImpactParticle;

        enum GunState
        {
            Wait,
            Notice,
            Shot,
        }
        GunState state;
        float stateTimer;

        private void Start()
        {
            laserController.Init(maxLaserDistance, transform.position, transform.rotation);
            ChangeState(GunState.Wait);
            //初回の周期ずらし
            stateTimer -= initWaitTime;
            //レーザーの非アクティブ化
            laserController.gameObject.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
            stateTimer += Time.deltaTime;

            switch (state)
            {
                case GunState.Wait:
                    WaitUpdate();
                    break;
                case GunState.Notice:
                    NoticeUpdate();
                    break;
                case GunState.Shot:
                    Shot();
                    break;
                default:
                    break;
            }

        }


        private void WaitUpdate()
        {
            if (stateTimer > waitTime)
            {
                ChangeState(GunState.Notice, stateTimer - waitTime);
                //レーザーのアクティブ化
                laserController.gameObject.SetActive(true);
                SoundManager.Instance.SoundChargeGun();
            }
        }

        private void NoticeUpdate()
        {
            var hit = laserController.DoRaycast();
            var piece = true;
            //衝突時
            if (hit.collider)
            {
                //衝突場所にエフェクト置きたい

                if (hit.collider.gameObject.TryGetComponent<PlayerController>(out var p))
                {
                    piece = false;
                }
                else
                {
                    //非貫通オブジェクトの場合は照射先を衝突場所に合わせる
                    piece = false;
                }

            }
            var rate = Mathf.Max(0, 1 - stateTimer / noticeTime);

            //予告線を照射する
            laserController.DrawRay(piece, rate * 0.5f);

            if (stateTimer > noticeTime)
            {
                ChangeState(GunState.Shot, stateTimer - noticeTime);
            }
        }

        private void Shot()
        {
            var hit = laserController.DoRaycast();
            //衝突時
            if (hit.collider)
            {
                if (hit.collider.gameObject.TryGetComponent<PlayerController>(out var p))
                {
                    //プレイヤーに当たった場合は死亡判定
                    p.TryDying();
                }
                else if (hit.collider.gameObject.TryGetComponent<BlockController>(out var b))
                {
                    //ブロックに当たった場合は波を生成
                    var rotation = transform.rotation;
                    var directionRad = rotation.eulerAngles.z * Mathf.Deg2Rad;

                    var param = new VibrationParameter()
                    {
                        amplitude = -power * Mathf.Sin(directionRad)
                    };
                    b.AddForce(param);
                }
                else if (hit.collider.gameObject.TryGetComponent<GoalController>(out var goal))
                {
                    //ゴールに当たった場合はゴール判定にする
                    goal.TryGoal();
                }
                else if (hit.collider.gameObject.TryGetComponent<BombController>(out var bomb))
                {
                    //ボムに当たった場合は爆発
                    bomb.Explode();
                }

                //当たった場所にパーティクル生成
                shotImpactParticle.gameObject.transform.position = laserController.PosFront;
                shotImpactParticle.Emit(10);
            }
            //レーザーの非アクティブ化
            laserController.gameObject.SetActive(false);
            ChangeState(GunState.Wait, stateTimer);

            //効果音再生
            SoundManager.Instance.SoundShotGun();

            ////Particle生成(うまく動いてない)
            //var scale = shotParticle.gameObject.transform.localScale;
            //scale.y = laserController.LaserLenght;
            //shotParticle.gameObject.transform.localScale = scale;
            //var pos = shotParticle.gameObject.transform.position;
            //pos.x += scale.y / 4f;
            //shotParticle.gameObject.transform.position = pos;
            //shotParticle.Emit(10);
        }

        void ChangeState(GunState state, float overTime = 0)
        {
            this.state = state;
            stateTimer = overTime;
        }
    }
}