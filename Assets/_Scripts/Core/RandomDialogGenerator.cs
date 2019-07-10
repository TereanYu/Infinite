#region Author
///-----------------------------------------------------------------
///   Namespace:		YU.ECS
///   Class:			RandomDialogGenerator
///   Author: 		    yutian
///-----------------------------------------------------------------
#endregion

using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace YU.ECS
{
    public class RandomDialogGenerator : MonoBehaviour
    {
        private List<string> m_words = new List<string>();

        private StringBuilder m_dialog = new StringBuilder();

        private void Awake()
        {
            m_words.Add("我们已经分开，但是我们尊敬你的勇气，请跟随我们的碎片吧");
            m_words.Add("希望我们这个世界的坏结局能给你指引，请跟随我们的碎片");
            m_words.Add("你是谁，为什么和我长得一样？");
            m_words.Add("我曾经像你一样找过那个世界，可是那太远了，也许我的碎片能指引你方向");
            m_words.Add("等闲变却故人心，却道故人心易变");
            m_words.Add("去吧，去替我们找到那个幸福的世界，那将是我们唯一的安慰");
            m_words.Add("无穷的平行宇宙？听着大的令人感到寒冷，祝你成功，愿引力指引你");
        }

        private int index = 0;
        public void GenerateRandomWords(int distance)
        {

            if (m_words.Count > 0)
            {
                index = UnityEngine.Random.Range(0, m_words.Count);
                UIManager.Instance.ShowBottomText(m_words[index]);
                m_words.RemoveAt(index);
            }
            else
            {
                m_dialog.Clear();
                m_dialog.AppendFormat("请朝着碎片的方向，你离那个世界还隔着{0}个世界的距离", distance);
                UIManager.Instance.ShowBottomText(m_dialog.ToString());
            }
            
        }
    }
}


