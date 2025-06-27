using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class QuizDB : MonoBehaviour
{
    [SerializeField] private List<Question> m_questionList = null;
    [SerializeField] private List<Question> m_backup;


    private void Awake()
    {
        m_backup = m_questionList.ToList();
    }

    public Question GetRandom(bool remove = true)
    {
       if(m_questionList.Count == 0)
        RestoreBackup();

        int index = Random.Range(0, m_questionList.Count);

        if (!remove)
            return m_questionList[index];
        
        Debug.Log(index);

        Question q = m_questionList[index];
        m_questionList.RemoveAt(index);

        return q;
        
    }

    private void RestoreBackup()
    {
        m_questionList = m_backup.ToList();
    }
    
}
