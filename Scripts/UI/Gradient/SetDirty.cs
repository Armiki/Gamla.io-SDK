using UnityEngine;
using UnityEngine.UI;

namespace Gamla.GUI
{
	public class SetDirty : MonoBehaviour
	{
		public Graphic m_graphic;
		
		void Reset()
		{
			m_graphic = GetComponent<Graphic>();
		}
		
		void Update()
		{
			m_graphic.SetVerticesDirty();
		}
	}
}