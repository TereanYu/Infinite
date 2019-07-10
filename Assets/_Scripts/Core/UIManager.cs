#region Author
///-----------------------------------------------------------------
///   Namespace:		YU.ECS
///   Class:			UIManager
///   Author: 		    yutian
///-----------------------------------------------------------------
#endregion

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace YU.ECS {

    public class UIManager : MonoSingleton<UIManager>
    {
        public float m_textShowTime = 3f;
        public float m_textStayTime = 4f;
        public float m_textFadeTime = 3f;

        public Text bottomText;
        public Text mainTitle;
        public Text distanceText;
        public Text successText;
        public Text thanksText;


        public void HideStartUI()
        {
            mainTitle.DOFade(0f, 2f);
        }

        public void ShowBottomText(string content)
        {
            bottomText.text = content;
            bottomText.DOFade(1f, m_textShowTime);
            StartCoroutine(WaitToTextFade());
        }

        private IEnumerator WaitToTextFade()
        {
            yield return new WaitForSeconds(m_textStayTime);
            bottomText.DOFade(0f, m_textFadeTime);
        }


        public void UpdateDistance(float distance)
        {
            distanceText.transform.localScale = Vector3.one;
            distanceText.text = distance.ToString();
            distanceText.transform.DOPunchScale(Vector3.one*1.2f,0.8f,10,0);
        }

        public void ShowSuccessText()
        {
            successText.gameObject.SetActive(true);
            successText.DOFade(1f, 5f);
        }

        public void ShowThanksText()
        {
            successText.gameObject.SetActive(false);
            thanksText.gameObject.SetActive(true);
            thanksText.DOFade(1f, 5f);
        }
    }
}

