using UnityEngine;

namespace PowderPPC.Object.Player
{
    /// <summary>
    /// プレイヤーの入力操作周りを管理
    /// </summary>
    public class PlayerInputController
    {
        enum InputType
        {
            /// <summary>
            /// デフォルト状態
            /// </summary>
            Default,
            /// <summary>
            /// カジュアル（↓キーだけでヒップドロップ発動可能）
            /// </summary>
            Casual,
        }

        InputType type;

        public PlayerInputController()
        {
            type = InputType.Casual;
        }

        /// <summary>
        /// 左右移動による入力があるかどうかを確認し、ある場合は方向の符号値を返す
        /// 入力が無い場合は0
        /// </summary>
        /// <returns></returns>
        public float TryGetMoveSpeedRate()
        {
          
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                return -1;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                return +1;
            }
            return 0;
        }
        
        /// <summary>
        /// ジャンプボタンを押した瞬間か
        /// </summary>
        /// <returns></returns>
        public bool TryGetJumpDown()
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) ||
                   Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// ジャンプボタンを押し続けている状態か
        /// （ヒップドロップを入力しっぱなしで連続発動させないための処理なので入力方法によって制御キーが変わる）
        /// </summary>
        /// <returns></returns>
        public bool TryGetJump()
        {
            switch (type)
            {
                case InputType.Default:
                    if (//Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ||
                           Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Z))
                    {
                        return true;
                    }
                    break;
                case InputType.Casual:
                    if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }


            return false;
        }

        /// <summary>
        /// ヒップドロップ発動に必要なコマンドを入力しているかどうか
        /// </summary>
        /// <returns></returns>
        public bool TryGetHipdrop()
        {
            switch (type)
            {
                case InputType.Default:
                    //ジャンプボタンが入力中＆下ボタンが入力中
                    if ((Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.Space)) ||
                        (Input.GetKey(KeyCode.DownArrow) && (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space))))
                    {
                        return true;
                    }
                    break;
                    case InputType.Casual:
                    ////下ボタンが入力した瞬間か（仮）
                    //if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                    //下ボタンが入力中（仮）
                    if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }


            return false;
        }
    }
}