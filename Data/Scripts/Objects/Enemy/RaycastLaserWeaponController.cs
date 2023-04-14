using PowderPPC.Object.Player;
using UnityEngine;

namespace PowderPPC.Object.Enemy
{
    /// <summary>
    /// Raycastレーザーを常時撃ち続ける敵
    /// プレイヤーが触れると死亡するがブロックで防ぐことができる
    /// Raycastの仕組みを学ぶために作られた
    /// </summary>
    public class RaycastLaserWeaponController : MonoBehaviour
    {
        [SerializeField] RaycastLaserController laserController;
        [SerializeField] float maxLaserDistance;

        void Start()
        {
            laserController.Init(maxLaserDistance, transform.position, transform.rotation);
        }

        void Update()
        {
            var hit = laserController.DoRaycast();
            var piece = true;
            //衝突時
            if (hit.collider)
            {
                if (hit.collider.gameObject.TryGetComponent<PlayerController>(out var p))
                {
                    p.TryDying();
                }
                else if (hit.collider.gameObject.TryGetComponent<BombController>(out var bomb))
                {
                    //ボムに当たった場合は爆発
                    bomb.Explode();
                }
                else
                {
                    //非貫通オブジェクトの場合は照射先を衝突場所に合わせる
                    piece = false;
                }

            }
            //非衝突時
            else
            {

            }
            laserController.DrawRay(piece);
        }
    }
}