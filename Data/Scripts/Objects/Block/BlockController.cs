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
        /// �v���C���[�ƐڐG����������Ă��邩�ǂ���
        /// �ڐG���Ă���ꍇTrue�ŁAFixedUpdate�̃^�C�~���O��False�ɂȂ�
        /// IsPlayerInChild�̂Ƃ��A��苗���ȏ㗣��ĂȂ���Ή�������Ȃ�
        /// </summary>
        public bool IsStayPlayer { get; private set; }

        /// <summary>
        /// �v���C���[�ƐڐG���Ɏ��g���㏸��Ԃɂ��邩�ǂ���
        /// �v���C���[�ƐڐG���ɏ㏸��ԂȂ�I���ɂȂ�A��ڐG���ȍ~�ɏ㏸��ԂłȂ��Ȃ�΃I�t
        /// </summary>
        public bool IsStayPlayerWithHighPos { get; private set; }
        /// <summary>
        /// ���݂̐U��
        /// </summary>
        float amplitudeSum;
        /// <summary>
        /// ���O�̃t���[���̐U��
        /// </summary>
        float prevAmplitudeSum;
        /// <summary>
        /// ���g������t�߂ɂ��邩�ǂ���
        /// </summary>
        bool isTopPos => amplitudeSum > 0 && Mathf.Abs(amplitudeSum - prevAmplitudeSum) < 0.05f;
        /// <summary>
        /// ���g���㏸��Ԃ�
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
        /// ���g�̐U����`��������X�V����
        /// </summary>
        public void MyTransmitUpdate()
        {

            deletevibrationlist = new LinkedList<VibrationController>();

            foreach (var v in VibrationList)
            {

                //�`������
                if (v.CanTransmission)
                {
                    DoTranmit(v);
                    v.DoAfterTransmission();
                }

                //�U�����I�������̍폜����
                if (v.IsEndVibration)
                {
                    deletevibrationlist.AddLast(v);
                }
            }
            //�폜���ڂ��������ꍇ�͍폜����
            foreach (var v in deletevibrationlist)
            {
                VibrationList.Remove(v);
            }
        }

        /// <summary>
        /// �`�����ʂ����g�̍��W�ɔ��f�����鏈��
        /// �Ō�Ɍ��ʂ̐�Βl��Ԃ�
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

            //�v���C���[�Ɣ�ڐG�����g�����~��Ԃ̎��ɉ���
            if(!IsStayPlayer && !isUppingPos)
            {
                IsStayPlayerWithHighPos = false;
            }

            return Mathf.Abs(amplitudeSum);
        }

        /// <summary>
        /// ���W�̍X�V
        /// </summary>
        /// <param name="y"></param>
        private void UpdatePos(float y)
        {
            var pos = defaultPos;
            pos.y += y;
            myRigidbody.MovePosition(pos);
        }

        /// <summary>
        /// ���g�����������Ƃ��ȂǂɃu���b�N�̊ԂɌ��Ԃ��ł���̂Ŕ�������ɐL�΂�
        /// </summary>
        private void UpdateColliderSize()
        {
            var posCenter = transform.position;
            var yLeft = leftBlock?.transform.position.y ?? posCenter.y;
            var yRight = rightBlock?.transform.position.y ?? posCenter.y;

            var minY = Mathf.Min(yLeft, yRight);
            //���ׂ̍��W�����g���Ⴉ�����ꍇ�͔����L�΂�
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
        /// �ׂ̃u���b�N�ɐU����`��
        /// </summary>
        /// <param name="vibration"></param>
        private void DoTranmit(VibrationController vibration)
        {
            switch (vibration.MyForce)
            {
                case VibrationController.ForceFrom.Center:
                    //���[�ɓ`������
                    leftBlock?.VibrationList.AddLast(new VibrationController(vibration, VibrationController.ForceFrom.Right));
                    rightBlock?.VibrationList.AddLast(new VibrationController(vibration, VibrationController.ForceFrom.Left));
                    break;
                case VibrationController.ForceFrom.Right:
                    //���ɓ`������
                    leftBlock?.VibrationList.AddLast(new VibrationController(vibration, VibrationController.ForceFrom.Right));
                    break;
                case VibrationController.ForceFrom.Left:
                    //�E�ɓ`������
                    rightBlock?.VibrationList.AddLast(new VibrationController(vibration, VibrationController.ForceFrom.Left));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// �u���b�N�̏����������i�A�������j
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
        /// �O�����玩�g�ɐU����ǉ�����
        /// </summary>
        /// <param name="param">�R���X�g���N�^�Ŏ��O�Ƀp�����[�^�����߂Ă���</param>
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



        //�Փ˔���
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

