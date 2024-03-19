using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMove : MonoBehaviour
{
    /// <summary>
    /// 右に設定したマウスで動かすオブジェクト
    /// </summary>
    [SerializeField] GameObject R;

    /// <summary>
    /// 左に設定したマウスで動かすオブジェクト
    /// </summary>
    [SerializeField] GameObject L;

    /// <summary>
    /// オブジェクト R と L の移動速度
    /// </summary>
    public float MoveSpeed = 0.1f;

    /// <summary>
    /// 未加工の入力を読み込むクラス
    /// </summary>
    private MouseInputReader m_mouseInputReader;

    /// <summary>
    /// 右のマウスの移動ベクトル
    /// </summary>
    private Vector2 m_rightInputValueVec;

    /// <summary>
    /// 左のマウスの移動ベクトル
    /// </summary>
    private Vector2 m_leftInputValueVec;

    // Start is called before the first frame update
    void Start()
    {
        m_mouseInputReader = GetComponent<MouseInputReader>();
    }

    // Update is called once per frame
    void Update()
    {
        // 入力があれば読み込む
        if (0 < m_mouseInputReader.GetRightInputValueListCount())
        {
            m_rightInputValueVec = m_mouseInputReader.PopRightInputValueList() * MoveSpeed;
        }
        else
        {
            m_rightInputValueVec = Vector2.zero;
        }

        if (0 < m_mouseInputReader.GetLeftInputValueListCount())
        {
            m_leftInputValueVec = m_mouseInputReader.PopLeftInputValueList() * MoveSpeed;
        }
        else
        {
            m_leftInputValueVec = Vector2.zero;
        }

        // オブジェクトを移動させる
        R.transform.position += new Vector3(m_rightInputValueVec.x, 0, -m_rightInputValueVec.y);
        L.transform.position += new Vector3(m_leftInputValueVec.x, 0, -m_leftInputValueVec.y);

        Debug.Log("Right: " + m_rightInputValueVec);
        Debug.Log("Left: " + m_leftInputValueVec);
    }
}
