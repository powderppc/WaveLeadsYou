using UnityEngine;

namespace PowderPPC.Object.Block
{
    public class BlockWithPlayerController : BlockController
    {
        /// <summary>
        /// プレイヤーを子要素として所持しているかどうか
        /// </summary>
        public bool IsPlayerInChild { get; private set; }

        public void SetPlayer()
        {
            IsPlayerInChild = true;
        }

        public void RemovePlayer()
        {
            IsPlayerInChild = false;
        }

    }
}