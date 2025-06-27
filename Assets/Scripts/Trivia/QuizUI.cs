using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizUI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_question = null;
    [SerializeField] private List<OptionButton> m_buttonList = null;

    public void Construct(Question q, Action<OptionButton> callback)
    {
        m_question.text = q.text;
        
        for (int n= 0; n < m_buttonList.Count ; n++)
        {
            m_buttonList[n].Construct(q.options[n], callback);
        }
    }
    
}
