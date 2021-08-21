using UnityEngine;
using System.Collections;

namespace Koiusa.InteractiveRoom
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Rigidbody))]
	public class RigidCharactorLocomotion : MonoBehaviour
	{

		private Animator animator;
		private Vector3 velocity;
		[SerializeField]
		private float jumpPower = 5f;
		//�@�n�ʂɐڒn���Ă��邩�ǂ���
		[SerializeField]
		private bool isGrounded;
		//�@���͒l
		private Vector3 input;
		//�@��������
		[SerializeField]
		private float walkSpeed = 1.5f;
		//�@rigidbody
		private Rigidbody rigid;
		//�@���C���[�}�X�N
		[SerializeField]
		private LayerMask layerMask;
		//�@�O���ɒi�������邩���ׂ郌�C���΂��I�t�Z�b�g�ʒu
		[SerializeField]
		private Vector3 stepRayOffset = new Vector3(0f, 0.05f, 0f);
		//�@���C���΂�����
		[SerializeField]
		private float stepDistance = 0.5f;
		//�@�����i��
		[SerializeField]
		private float stepOffset = 0.3f;
		//�@�����p�x
		[SerializeField]
		private float slopeLimit = 65f;
		//�@�����i���̈ʒu�����΂����C�̋���
		[SerializeField]
		private float slopeDistance = 0.6f;

		void Start()
		{
			animator = GetComponent<Animator>();
			rigid = GetComponent<Rigidbody>();
		}

		void Update()
		{

		}

		void FixedUpdate()
		{
			//�@�L�����N�^�[���ڒn���Ă���ꍇ
			if (isGrounded)
			{
				//�@�ڒn�����̂ňړ����x��0�ɂ���
				velocity = Vector3.zero;
				input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

				//�@�����L�[������������Ă���
				if (input.magnitude > 0f)
				{
					animator.SetFloat("Speed", input.magnitude);
					transform.LookAt(rigid.position + input.normalized);

					var stepRayPosition = rigid.position + stepRayOffset;

					//�@�X�e�b�v�p�̃��C���n�ʂɐڐG���Ă��邩�ǂ���
					if (Physics.Linecast(stepRayPosition, stepRayPosition + rigid.transform.forward * stepDistance, out var stepHit, layerMask))
					{
						//�@�i�s�����̒n�ʂ̊p�x���w��ȉ��A�܂��͏����i����艺�������ꍇ�̈ړ�����

						if (Vector3.Angle(rigid.transform.up, stepHit.normal) <= slopeLimit
						|| (Vector3.Angle(rigid.transform.up, stepHit.normal) > slopeLimit
							&& !Physics.Linecast(rigid.position + new Vector3(0f, stepOffset, 0f), rigid.position + new Vector3(0f, stepOffset, 0f) + rigid.transform.forward * slopeDistance, layerMask))
						)
						{
							velocity = new Vector3(0f, (Quaternion.FromToRotation(Vector3.up, stepHit.normal) * rigid.transform.forward * walkSpeed).y, 0f) + rigid.transform.forward * walkSpeed;
						}
						else
						{
							//�@�w�肵�������ɓ��Ă͂܂�Ȃ��ꍇ�͑��x��0�ɂ���
							velocity = Vector3.zero;
						}

						Debug.Log(Vector3.Angle(Vector3.up, stepHit.normal));

						//�@�O���̕ǂɐڐG���Ă��Ȃ����
					}
					else
					{
						velocity = transform.forward * walkSpeed;
					}

					//�@�L�[�̉���������������ꍇ�͈ړ����Ȃ�
				}
				else
				{
					animator.SetFloat("Speed", 0f);
				}
				//�@�W�����v
				if (Input.GetButtonDown("Jump")
					&& !animator.GetCurrentAnimatorStateInfo(0).IsName("Jump")
					&& !animator.IsInTransition(0)      //�@�J�ړr���ɃW�����v�����Ȃ�����
				)
				{
					animator.SetTrigger("Jump");
					//�@�W�����v������ڒn���Ă��Ȃ���Ԃɂ���
					isGrounded = false;
					animator.SetBool("IsGrounded", isGrounded);
					velocity.y = jumpPower;
				}
			}
			//�@�L�����N�^�[���ړ������鏈��
			rigid.MovePosition(rigid.position + velocity * Time.fixedDeltaTime);
		}

		private void OnCollisionEnter(Collision collision)
		{
			//�@�n�ʂɒ��n�������ǂ����̔���
			if (Physics.CheckSphere(rigid.position, 0.3f, layerMask))
			{
				isGrounded = true;
				animator.SetBool("IsGrounded", isGrounded);
				velocity.y = 0f;
			}
		}

		private void OnCollisionExit(Collision collision)
		{
			//�@�n�ʂ���~�肽���̏���
			//�@�n�ʂƂ������C���[�̃Q�[���I�u�W�F�N�g���痣�ꂽ��
			if (1 << collision.gameObject.layer == layerMask)
			{
				//�@�������Ƀ��C���[���΂��n�ʂƂ��郌�C���[�ƐڐG���Ȃ���Βn�ʂ��痣�ꂽ�Ɣ��肷��
				if (!Physics.Linecast(rigid.position + Vector3.up * 0.2f, rigid.position + Vector3.down * 0.3f, layerMask))
				{
					isGrounded = false;
					animator.SetBool("IsGrounded", isGrounded);
				}
			}
		}

		private void OnDrawGizmos()
		{
			var stepRayPosition = transform.position + stepRayOffset;
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(stepRayPosition, stepRayPosition + transform.forward * stepDistance);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position + new Vector3(0f, stepOffset, 0f), transform.position + new Vector3(0f, stepOffset, 0f) + transform.forward * slopeDistance);
		}
	}
}