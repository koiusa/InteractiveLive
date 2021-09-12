using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// 一定時間ごとにLaserを撃つMonoBehaviourクラス
/// </summary>
public class VFXLaserPlayer : MonoBehaviour
{
    [SerializeField] private VisualEffect _visualEffect; // レーザーVisualEffect
    [SerializeField] private Transform laserTarget; // レーザーが狙い撃ちする対象
    [SerializeField] private float interval = 4f; // レーザーを撃つ間隔

    private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            _visualEffect.Reinit();
            _visualEffect.Play();
        }
    }

    void Update()
    {
        // Ray情報の作成
        var origin = _visualEffect.GetVector3("LaserSrcPosition"); // レーザー始点
        var direction = (laserTarget.position - origin).normalized; // レーザー向きベクトル
        var ray = new Ray(origin, direction);

        // Raycast実行
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Rayが当たった位置をレーザー終点に設定
            _visualEffect.SetVector3("LaserDstPosition", hit.point);

            // 法線情報の設定
          //  _visualEffect.SetVector3("Collision Normal", hit.normal);
        }
        else
        {
            // レーザー終点の設定
            _visualEffect.SetVector3("LaserDstPosition", laserTarget.position);

            // 法線情報の設定
           // _visualEffect.SetVector3("Collision Normal", new Vector3(0f, 1f, 0f));
        }
    }
}