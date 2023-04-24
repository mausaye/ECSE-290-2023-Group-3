using UnityEngine;
using System.Collections;
using TMPro;

namespace TMPro.Examples
{
    public class TeleType : MonoBehaviour
    {
        private string[] labels = new string[]
        {
            "It is currently finals week at CWRU...",
            "You are awaken by your phone buzzing...",
            "You crawl out of bed to reach it.",
            "Your game design group had scheduled a meeting for this morning, but you overslept.",
            "You need to get to Kelvin Smith Library ASAP to finish your Game Design Project!", 
            "You have to hurry now!",
            "It snowed 8 inches last night... be careful!"
        };

        private TMP_Text m_textMeshPro;

        void Awake()
        {
            m_textMeshPro = GetComponent<TMP_Text>();
            m_textMeshPro.enableWordWrapping = true;
            m_textMeshPro.alignment = TextAlignmentOptions.Top;
        }

        IEnumerator Start()
        {
            for (int i = 0; i < labels.Length; i++)
            {
                m_textMeshPro.text = labels[i];
                m_textMeshPro.ForceMeshUpdate();

                int totalVisibleCharacters = m_textMeshPro.textInfo.characterCount;
                int counter = 0;
                int visibleCount = 0;

                while (visibleCount < totalVisibleCharacters)
                {
                    visibleCount = counter % (totalVisibleCharacters + 1);
                    m_textMeshPro.maxVisibleCharacters = visibleCount;
                    counter += 1;
                    yield return new WaitForSeconds(0.05f);
                }

                yield return new WaitForSeconds(1.2f);
            }
        }
    }
}