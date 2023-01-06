/*
    お絵描きソフト風の実装追加
    
    参考：
    https://qiita.com/maple-bitter/items/290ba820cffb8c97834f
 */



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PaintController : MonoBehaviour
{    
    [SerializeField]
    private RawImage m_image = null;

    private Texture2D m_texture = null;
    public Texture2D GetPaintedTexture()
    {
        return m_texture;
    }

    [SerializeField]
    private int m_width = 12;

    [SerializeField]
    private int m_height = 12;

    private Vector2 m_prePos;
    private Vector2 m_TouchPos;

    private float m_clickTime, m_preClickTime;

    private Vector2 m_offsetPosLeftBottom;


    // 線を描画
    public void OnDrag(BaseEventData arg)
    {

        PointerEventData _event = arg as PointerEventData; //タッチの情報取得

        // 押されているときの処理
        m_TouchPos = _event.position; // 現在のポインタの座標
        m_clickTime = _event.clickTime; // 最後にクリックイベントが送信された時間を取得

        float disTime = m_clickTime - m_preClickTime; // 前回のクリックイベントとの時差

        var dir = m_prePos - m_TouchPos; // 直前のタッチ座標との差
        if (disTime > 0.01) dir = new Vector2(0, 0);    // 0.1秒以上間があいたらタッチ座標の差を0にする。

        var dist = (int)dir.magnitude;  // タッチ座標ベクトルの絶対値

        dir = dir.normalized;   // 正規化

        //指定のペンの太さ(ピクセル)で、前回のタッチ座標から今回のタッチ座標まで塗りつぶす
        for (int d = 0; d < dist; ++d)
        {
            var p_pos = m_TouchPos + dir * d; // paint pos
            SetWhitePixelsToTexture(p_pos);
        }

        m_texture.Apply();
        m_prePos = m_TouchPos;
        m_preClickTime = m_clickTime;
    }

    // 点を描画
    public void OnPointerClick(BaseEventData arg)
    {
        PointerEventData _event = arg as PointerEventData; //タッチの情報取得

        // 押されているときの処理
        m_TouchPos = _event.position; //現在のポインタの座標

        SetWhitePixelsToTexture(m_TouchPos);
        
        m_texture.Apply();
    }

    void SetWhitePixelsToTexture(Vector2 _pos)
    {
        var lPos = _pos - m_offsetPosLeftBottom;
        lPos.y -= m_height / 2.0f;  // ペンの太さ（ピクセル）反映のため
        lPos.x -= m_width / 2.0f;   // ペンの太さ（ピクセル）反映のため

        for (int h = 0; h < m_height; ++h)
        {
            int y = (int)(lPos.y + h);
            if (y < 0 || y > m_texture.height) continue; //タッチ座標がテクスチャの外の場合、描画処理を行わない

            for (int w = 0; w < m_width; ++w)
            {
                int x = (int)(lPos.x + w);
                if (x >= 0 && x <= m_texture.width)
                {
                    m_texture.SetPixel(x, y, Color.white); //線を描画
                }
            }
        }
    }




    void Start()
    {
        ResetTexture();
    }

    public void ResetTexture()
    {
        var rectTransform = m_image.gameObject.GetComponent<RectTransform>();
        m_texture = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height, TextureFormat.RGBA32, false);
        SetBlackTexture((int)rectTransform.rect.width, (int)rectTransform.rect.height);
        m_image.texture = m_texture;
        m_offsetPosLeftBottom = rectTransform.offsetMin;
    }

    void SetBlackTexture(int width, int height)
    {
        for(int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                m_texture.SetPixel(w, h, Color.black);
            }
        }
        m_texture.Apply();      
    }
}
