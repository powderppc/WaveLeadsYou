using UnityEngine;

namespace PowderPPC.Object.Enemy
{
    public class PieceLaserController : MonoBehaviour
    {
        [SerializeField] PieceLaserObjectController laser;

        void Start()
        {
            //レーザーを活性化
            laser.gameObject.SetActive(true);
        }

    }
}