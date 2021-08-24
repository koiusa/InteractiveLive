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
		//　地面に接地しているかどうか
		[SerializeField]
		private bool isGrounded;
		//　入力値
		private Vector3 movingDirecion;
		private Vector3 roleDirecion;
		//　歩く速さ
		[SerializeField]
		private float walkSpeed = 1.5f;
		[SerializeField]
		public float SprintSpeed = 5.335f;
		public float SpeedChangeRate = 10.0f;
		//　rigidbody
		private Rigidbody _rigid;
		//　レイヤーマスク
		[SerializeField]
		private LayerMask layerMask;
		//　前方に段差があるか調べるレイを飛ばすオフセット位置
		[SerializeField]
		private Vector3 stepRayOffset = new Vector3(0f, 0.05f, 0f);
		//　レイを飛ばす距離
		[SerializeField]
		private float stepDistance = 0.5f;
		//　昇れる段差
		[SerializeField]
		private float stepOffset = 0.3f;
		//　昇れる角度
		[SerializeField]
		private float slopeLimit = 65f;
		//　昇れる段差の位置から飛ばすレイの距離
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
			//　キャラクターが接地している場合
			if (isGrounded)
			{
				if (_hasAnimator)
				{
					_animator.SetBool(_animIDJump, false);
					_animator.SetBool(_animIDFreeFall, false);
				}

				//　接地したので移動速度を0にする
				velocity = Vector3.zero;

				//　方向キーが多少押されている
				if (movingDirecion.magnitude > 0f)
				{
					//animator.SetFloat(_animIDSpeed, movingDirecion.magnitude);
					//transform.LookAt(rigid.position + movingDirecion.normalized);

					_rigid.AddTorque(roleDirecion.normalized);

					var stepRayPosition = _rigid.position + stepRayOffset;

					//　ステップ用のレイが地面に接触しているかどうか
					if (Physics.Linecast(stepRayPosition, stepRayPosition + _rigid.transform.forward * stepDistance, out var stepHit, layerMask))
					{
						//　進行方向の地面の角度が指定以下、または昇れる段差より下だった場合の移動処理

						if (Vector3.Angle(_rigid.transform.up, stepHit.normal) <= slopeLimit
						|| (Vector3.Angle(_rigid.transform.up, stepHit.normal) > slopeLimit
							&& !Physics.Linecast(_rigid.position + new Vector3(0f, stepOffset, 0f), _rigid.position + new Vector3(0f, stepOffset, 0f) + _rigid.transform.forward * slopeDistance, layerMask))
						)
						{
							velocity = new Vector3(0f, (Quaternion.FromToRotation(Vector3.up, stepHit.normal) * _rigid.transform.forward * walkSpeed).y, 0f) + _rigid.transform.forward * walkSpeed;
						}
						else
						{
							//　指定した条件に当てはまらない場合は速度を0にする
							velocity = Vector3.zero;
						}

						Debug.Log(Vector3.Angle(Vector3.up, stepHit.normal));

						//　前方の壁に接触していなければ
					}
					else
					{
						velocity = movingDirecion.normalized * walkSpeed;
					}

					//　キーの押しが小さすぎる場合は移動しない
				}
				else
				{
					_animator.SetFloat(_animIDSpeed, 0f);
				}
				//　ジャンプ
				if (_input.jump
					&& !_animator.GetCurrentAnimatorStateInfo(0).IsName("Jump")
					&& !_animator.IsInTransition(0)      //　遷移途中にジャンプさせない条件
				)
				{
					_animator.SetTrigger(_animIDJump);
					//　ジャンプしたら接地していない状態にする
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
			//　キャラクターを移動させる処理
			//rigid.MovePosition(rigid.position + velocity * Time.fixedDeltaTime);
			if (!_input.jump)
			{
				_rigid.AddForce(_rigid.rotation * velocity * SpeedChangeRate, ForceMode.Force);
			}

			Move();
		}

		private void OnCollisionEnter(Collision collision)
		{
			//　地面に着地したかどうかの判定
			if (Physics.CheckSphere(_rigid.position, 0.3f, layerMask))
			{
				isGrounded = true;
				_animator.SetBool(_animIDGrounded, isGrounded);
				velocity.y = 0f;
			}
		}

		private void OnCollisionExit(Collision collision)
		{
			//　地面から降りた時の処理
			//　地面としたレイヤーのゲームオブジェクトから離れた時
			if (1 << collision.gameObject.layer == layerMask)
			{
				//　下向きにレイヤーを飛ばし地面とするレイヤーと接触しなければ地面から離れたと判定する
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