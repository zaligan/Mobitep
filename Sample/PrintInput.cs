using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintInput : MonoBehaviour
{
    private MouseInputReader m_mouseInputReader;

    /// <summary>
    /// EditorWindowで右に設定したデバイスの入力値
    /// </summary>
    private Vector2 m_rightInputValueVec;

    /// <summary>
    /// EditorWindowで左に設定したデバイスの入力値
    /// </summary>
    private Vector2 m_leftInputValueVec;

    // Start is called before the first frame update
    void Start()
    {
        // 同じGameObjectにアタッチされているMouseInputReaderを取得
        m_mouseInputReader = GetComponent<MouseInputReader>();
    }

    // Update is called once per frame
    void Update()
    {
        // 要素数が 0 より大きい場合のみ入力値を取得
        if (0 < m_mouseInputReader.GetRightInputValueListCount())
        {
            m_rightInputValueVec = m_mouseInputReader.GetRightInputValueList();
        }

        if (0 < m_mouseInputReader.GetLeftInputValueListCount())
        {
            m_leftInputValueVec = m_mouseInputReader.GetLeftInputValueList();
        }

        Debug.Log("Right: " + m_rightInputValueVec);
        Debug.Log("Left: " + m_leftInputValueVec);
    }
}
