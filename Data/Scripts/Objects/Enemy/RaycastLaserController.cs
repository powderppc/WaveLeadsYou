using UnityEngine;

namespace PowderPPC.Object.Enemy
{
    /// <summary>
    /// Raycastレーザーの照射を管理するクラス
    /// 外部からの入力によって照射を行い、衝突の判定をする
    /// </summary>
    public class RaycastLaserController : MonoBehaviour
    {
        /// <summary>
        /// 描画するレーザー本体
        /// </summary>
        [SerializeField] LaserObjController laserObj;
        /// <summary>
        /// 描画するレーザー先端
        /// </summary>
        [SerializeField] LaserObjController laserObjFront;
        float maxDistance;
        float laserLength;
        /// <summary>
        /// レーザーの描画距離
        /// </summary>
        public float LaserLenght => laserLength;

        /// <summary>
        /// レーザーのBoxサイズ
        /// </summary>
        Vector2 laserSize;
        /// <summary>
        /// レーザーの照射始点
        /// </summary>
        Vector2 origin;
        /// <summary>
        /// レーザーの照射方向
        /// </summary>
        Vector2 direction;

        RaycastHit2D hit;

        /// <summary>
        /// 照射距離と照射位置の初期化
        /// </summary>
        /// <param name="maxDistance"></param>
        /// <param name="origin"></param>
        /// <param name="rotation"></param>
        public void Init(float maxDistance, Vector2 origin, Quaternion rotation)
        {
            laserSize = laserObj.transform.localScale;
            this.maxDistance = maxDistance;
            InitPos(origin, rotation);
        }

        /// <summary>
        /// 照射位置の初期化
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="rotation"></param>
        public void InitPos(Vector2 origin, Quaternion rotation)
        {
            this.origin = origin;
            var directionRad = rotation.eulerAngles.z * Mathf.Deg2Rad;
            direction = new Vector2(Mathf.Cos(directionRad), Mathf.Sin(directionRad));
        }

        /// <summary>
        /// Raycastを飛ばす
        /// Update関数内で飛ばす
        /// </summary>
        /// <returns></returns>
        public RaycastHit2D DoRaycast()
        {
            //Rayを照射
            hit = Physics2D.BoxCast(origin, laserSize, 0, direction, maxDistance);
            return hit;

        }

        /// <summary>
        /// レーザーの描画処理
        /// 衝突してない場合は射程範囲を描画
        /// 衝突時にisPieceがFalseなら衝突位置までを描画
        /// </summary>
        /// <param name="isPiece">Trueの時、衝突時でもレーザーを貫通させる</param>
        public void DrawRay(bool isPiece)
        {
            laserLength = maxDistance;

            if (hit.collider && !isPiece)
            {
                laserLength = (hit.point - origin).magnitude;
            }


            //衝突時の状態を考慮したレーザーの描画処理
            var pos = transform.localPosition;
            var scale = laserSize;
            scale.x = laserLength - pos.x;
            laserObj.transform.localScale = scale;

            DrawLaserFront(isPiece);
        }

        /// <summary>
        /// レーザーの描画処理
        /// 衝突してない場合は射程範囲を描画
        /// 衝突時にisPieceがFalseなら衝突位置までを描画
        /// 描画幅を調整したい場合はこっち
        /// </summary>
        /// <param name="isPiece"></param>
        /// <param name="widthRate">0-1</param>
        public void DrawRay(bool isPiece, float widthRate)
        {
            laserLength = maxDistance;

            if (hit.collider && !isPiece)
            {
                laserLength = (hit.point - origin).magnitude;
            }


            //衝突時の状態を考慮したレーザーの描画処理
            var pos = transform.localPosition;
            var scale = laserSize;
            scale.x = laserLength - pos.x;
            scale.y = widthRate;
            laserObj.transform.localScale = scale;

            DrawLaserFront(isPiece);
        }

        void DrawLaserFront(bool isPiece)
        {
            if (hit.collider && !isPiece)
            {
                laserObjFront.gameObject.SetActive(true);
                var scale = laserObj.transform.localScale;
                var pos = new Vector2(scale.x, 0);
                laserObjFront.transform.localPosition = pos;
            }
            else
            {
                laserObjFront.gameObject.SetActive(false);
            }

        }

        public Vector2 PosFront => laserObjFront.transform.position;
    }
}