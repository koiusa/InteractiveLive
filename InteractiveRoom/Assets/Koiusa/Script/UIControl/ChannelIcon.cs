using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Windows;

public class ChannelIcon : MonoBehaviour
{
    private Sprite sprite;
    private RectTransform rectTransform;
    private Image image;
    [SerializeField]
    void Awake()
    {
        Debug.Log("ChannelIcon");
        rectTransform = gameObject.GetComponent<RectTransform>();
        image = gameObject.GetComponent<Image>();
    }

    public void CreateThumbnail(string AuthorUrl)
    {
        StartCoroutine(GetImage(AuthorUrl));
    }

    IEnumerator GetImage(string URI)
    {
        if (string.IsNullOrEmpty(URI)){
            yield break;
        }
        using (var request = UnityWebRequestTexture.GetTexture(URI))
        {
            //�摜���擾�ł���܂ő҂�
            Debug.Log("www.SendWebRequest()");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("www.SendWebRequest.Result.Success");
                var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                image.sprite = sprite;
                rectTransform.transform.localPosition = Vector3.zero;
                rectTransform.transform.rotation = Quaternion.identity;
                rectTransform.transform.localScale = Vector3.one * 100; 
            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }

    /// <summary>
    /// �L���b�V������e�N�X�`�������[�h
    /// </summary>
    private Sprite GetTexture(string savePath)
    {
        // �t�@�C�����Ȃ���� null
        if (!File.Exists(savePath))
            return null;

        // �ۑ����Ă���e�N�X�`���̃��[�h
        var texture = new Texture2D(0, 0);
        texture.LoadImage(File.ReadAllBytes(savePath));
        // Sprite �ɕϊ�
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    /// <summary>
    /// �t�@�C�������[�h
    /// �t�@�C��������΃��[�h���Ȃ�
    /// </summary>
    private IEnumerator LoadAndSaveTexture(string uri, string savePath)
    {
        // �t�@�C��������̂ŏI��
        if (Directory.Exists(savePath))
            yield break;

        var dir = System.IO.Path.GetDirectoryName(savePath);
        // �f�B���N�g�����Ȃ���΍쐬����
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        using (var request = UnityWebRequestTexture.GetTexture(uri))
        {
            request.disposeUploadHandlerOnDispose = false;
            // �t�@�C����ۑ�
            request.downloadHandler = new DownloadHandlerFile(savePath);
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
        }
    }

}
