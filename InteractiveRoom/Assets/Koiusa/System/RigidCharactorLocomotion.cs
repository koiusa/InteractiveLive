/// Reference
/// https://gametukurikata.com/program/rigidbodyandcollider
///
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using StarterAssets;

namespace Koiusa.InteractiveRoom
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Rigidbody))]
	public class RigidCharactorLocomotion : MonoBehaviour
	{
		private StarterAssetsInputs _input;

		private Animator _animator;
		private Vector3 velocity;
		[SerializeField]
		private float jumpPower = 5f;
		[Range(0.0f, 0.3f)]
		public float RotationSmoothTime = 0.12f;
		//�@�n�ʂɐڒn���Ă��邩�ǂ���
		[SerializeField]
		private bool isGrounded;
		//�@���͒l
		private Vector3 movingDirecion;
		private Vector3 roleDirecion;
		//�@��������
		[SerializeField]
		private float walkSpeed = 1.5f;
		[SerializeField]
		public float SprintSpeed = 5.335f;
		public float SpeedChangeRate = 10.0f;
		//�@rigidbody
		private Rigidbody _rigid;
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

		// player
		private float _speed;
		private float _animationBlend;
		private float _targetRotation = 0.0f;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// animation IDs
		private int _animIDSpeed;
		private int _animIDGrounded;
		private int _animIDJump;
		private int _animIDFreeFall;
		private int _animIDMotionSpeed;

		private bool _hasAnimator;
		private Vector3 _latestPos;
		private GameObject _mainCamera;
		private void AssignAnimationIDs()
		{
			_animIDSpeed = Animator.StringToHash("Speed");
			_animIDGrounded = Animator.StringToHash("Grounded");
			_animIDJump = Animator.StringToHash("Jump");
			_animIDFreeFall = Animator.StringToHash("FreeFall");
			_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
		}

		void Start()
		{
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
			AssignAnimationIDs();
			_hasAnimator = TryGetComponent(out _animator);
			_rigid = GetComponent<Rigidbody>();
			_input = GetComponent<StarterAssetsInputs>();
		}

		void Update()
		{
			movingDirecion = new Vector3(_input.move.x, 0f, _input.move.y);
			roleDirecion = new Vector3(0, _input.move.x, -_input.move.x);
		}

		void FixedUpdate()
		{
			//�@�L�����N�^�[���ڒn���Ă���ꍇ
			if (isGrounded)
			{
				if (_hasAnimator)
				{
					_animator.SetBool(_animIDJump, false);
					_animator.SetBool(_animIDFreeFall, false);
				}

				//�@�ڒn�����̂ňړ����x��0�ɂ���
				velocity = Vector3.zero;

				//�@�����L�[������������Ă���
				if (movingDirecion.magnitude > 0f)
				{
					//animator.SetFloat(_animIDSpeed, movingDirecion.magnitude);
					//transform.LookAt(rigid.position + movingDirecion.normalized);

					_rigid.AddTorque(roleDirecion.normalized);

					var stepRayPosition = _rigid.position + stepRayOffset;

					//�@�X�e�b�v�p�̃��C���n�ʂɐڐG���Ă��邩�ǂ���
					if (Physics.Linecast(stepRayPosition, stepRayPosition + _rigid.transform.forward * stepDistance, out var stepHit, layerMask))
					{
						//�@�i�s�����̒n�ʂ̊p�x���w��ȉ��A�܂��͏����i����艺�������ꍇ�̈ړ�����

						if (Vector3.Angle(_rigid.transform.up, stepHit.normal) <= slopeLimit
						|| (Vector3.Angle(_rigid.transform.up, stepHit.normal) > slopeLimit
							&& !Physics.Linecast(_rigid.position + new Vector3(0f, stepOffset, 0f), _rigid.position + new Vector3(0f, stepOffset, 0f) + _rigid.transform.forward * slopeDistance, layerMask))
						)
						{
							velocity = new Vector3(0f, (Quaternion.FromToRotation(Vector3.up, stepHit.normal) * _rigid.transform.forward * walkSpeed).y, 0f) + _rigid.transform.forward * walkSpeed;
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
						velocity = movingDirecion.normalized * walkSpeed;
					}

					//�@�L�[�̉���������������ꍇ�͈ړ����Ȃ�
				}
				else
				{
					_animator.SetFloat(_animIDSpeed, 0f);
				}
				//�@�W�����v
				if (_input.jump
					&& !_animator.GetCurrentAnimatorStateInfo(0).IsName("Jump")
					&& !_animator.IsInTransition(0)      //�@�J�ړr���ɃW�����v�����Ȃ�����
				)
				{
					_animator.SetTrigger(_animIDJump);
					//�@�W�����v������ڒn���Ă��Ȃ���Ԃɂ���
					isGrounded = false;
					_animator.SetBool(_animIDGrounded, isGrounded);
					//velocity.y = jumpPower;
					_rigid.AddForce(transform.up * jumpPower, ForceMode.Impulse);
				}
            }
            else
            {
				_input.jump = false;
            }
			//�@�L�����N�^�[���ړ������鏈��
			//rigid.MovePosition(rigid.position + velocity * Time.fixedDeltaTime);
			if (!_input.jump)
			{
				_rigid.AddForce(_rigid.rotation * velocity * SpeedChangeRate, ForceMode.Force);
			}

			Move();
		}

		private void OnCollisionEnter(Collision collision)
		{
			//�@�n�ʂɒ��n�������ǂ����̔���
			if (Physics.CheckSphere(_rigid.position, 0.3f, layerMask))
			{
				isGrounded = true;
				_animator.SetBool(_animIDGrounded, isGrounded);
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
				if (!Physics.Linecast(_rigid.position + Vector3.up * 0.2f, _rigid.position + Vector3.down * 0.3f, layerMask))
				{
					isGrounded = false;
					_animator.SetBool(_animIDGrounded, isGrounded);
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

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? SprintSpeed : walkSpeed;

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_rigid.velocity.x, 0.0f, _rigid.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}
			_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			////if (_input.move != Vector2.zero)
			////{
			////	_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
			////	float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

			////	// rotate to face input direction relative to camera position
			////	transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
			////}


			Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

			// move the player
			//_controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

			// update animator if using character
			if (_hasAnimator)
			{
				_animator.SetFloat(_animIDSpeed, _animationBlend);
				_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
			}
		}
	}
}