using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace naichilab
{
	/// <summary>
	/// �V�[���J�ڎ��̃t�F�[�h�C���E�A�E�g�𐧌䂷�邽�߂̃N���X .
	/// </summary>
	public class FadeManager : MonoBehaviour
	{

		#region Singleton

		private static FadeManager instance;

		public static FadeManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = (FadeManager)FindObjectOfType(typeof(FadeManager));

					if (instance == null)
					{
						Debug.LogError(typeof(FadeManager) + "is nothing");
					}
				}

				return instance;
			}
		}

		#endregion Singleton

		/// <summary>
		/// �f�o�b�O���[�h .
		/// </summary>
		public bool DebugMode = true;
		/// <summary>�t�F�[�h���̓����x</summary>
		private float fadeAlpha = 0;
		/// <summary>�t�F�[�h�����ǂ���</summary>
		private bool isFading = false;
		/// <summary>�t�F�[�h�F</summary>
		public Color fadeColor = Color.black;


		public void Awake()
		{
			if (this != Instance)
			{
				Destroy(this.gameObject);
				return;
			}

			DontDestroyOnLoad(this.gameObject);
		}

		public void OnGUI()
		{

			// Fade .
			if (this.isFading)
			{
				//�F�Ɠ����x���X�V���Ĕ��e�N�X�`����`�� .
				this.fadeColor.a = this.fadeAlpha;
				GUI.color = this.fadeColor;
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
			}

			if (this.DebugMode)
			{
				if (!this.isFading)
				{
					//Scene�ꗗ���쐬 .
					//(UnityEditor���O��Ԃ��g��Ȃ��Ǝ����擾�ł��Ȃ������̂Ō��߂����ō쐬) .
					List<string> scenes = new List<string>();
					scenes.Add("StartIdleScene");
					scenes.Add ("InteractiveRoom");
					//scenes.Add ("SomeScene2");


					//Scene������Ȃ� .
					if (scenes.Count == 0)
					{
						GUI.Box(new Rect(10, 10, 200, 50), "Fade Manager(Debug Mode)");
						GUI.Label(new Rect(20, 35, 180, 20), "Scene not found.");
						return;
					}


					GUI.Box(new Rect(10, 10, 300, 50 + scenes.Count * 25), "Fade Manager(Debug Mode)");
					GUI.Label(new Rect(20, 30, 280, 20), "Current Scene : " + SceneManager.GetActiveScene().name);

					int i = 0;
					foreach (string sceneName in scenes)
					{
						if (GUI.Button(new Rect(20, 55 + i * 25, 100, 20), "Load Level"))
						{
							LoadScene(sceneName, 1.0f);
						}
						GUI.Label(new Rect(125, 55 + i * 25, 1000, 20), sceneName);
						i++;
					}
				}
			}



		}

		/// <summary>
		/// ��ʑJ�� .
		/// </summary>
		/// <param name='scene'>�V�[����</param>
		/// <param name='interval'>�Ó]�ɂ����鎞��(�b)</param>
		public void LoadScene(string scene, float interval)
		{
			StartCoroutine(TransScene(scene, interval));
		}

		/// <summary>
		/// �V�[���J�ڗp�R���[�`�� .
		/// </summary>
		/// <param name='scene'>�V�[����</param>
		/// <param name='interval'>�Ó]�ɂ����鎞��(�b)</param>
		private IEnumerator TransScene(string scene, float interval)
		{
			//���񂾂�Â� .
			this.isFading = true;
			float time = 0;
			while (time <= interval)
			{
				this.fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
				time += Time.deltaTime;
				yield return 0;
			}

			//�V�[���ؑ� .
			SceneManager.LoadScene(scene);

			//���񂾂񖾂邭 .
			time = 0;
			while (time <= interval)
			{
				this.fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
				time += Time.deltaTime;
				yield return 0;
			}

			this.isFading = false;
		}
	}
}