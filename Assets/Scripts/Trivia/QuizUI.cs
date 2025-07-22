using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;

public class QuizUI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_question = null;
    [SerializeField] private List<OptionButton> m_buttonList = null;

    private Coroutine loadingCoroutine;

    public void Construct(Question q, Action<OptionButton> callback)
    {
        if (loadingCoroutine != null)
            StopCoroutine(loadingCoroutine);

        loadingCoroutine = StartCoroutine(LoadLocalizedQuestion(q, callback));
    }

    private IEnumerator LoadLocalizedQuestion(Question q, Action<OptionButton> callback)
    {
        var localizedQuestion = new LocalizedString
        {
            TableReference = "Quiz",   
            TableEntryReference = q.localizationKey
        };
        var handleQ = localizedQuestion.GetLocalizedStringAsync();
        yield return handleQ;
        m_question.text = handleQ.Result;

        for (int i = 0; i < m_buttonList.Count; i++)
        {
            OptionButton button = m_buttonList[i];
            if (i < q.options.Count)
            {
                Option option = q.options[i];

                var localizedOption = new LocalizedString
                {
                    TableReference = "Quiz",
                    TableEntryReference = option.localizationKey
                };
                var handleO = localizedOption.GetLocalizedStringAsync();
                yield return handleO;

                option.text = handleO.Result;
                button.Construct(option, callback);
            }
            else
            {

                m_buttonList[i].gameObject.SetActive(false);
            }
        }
    }
}