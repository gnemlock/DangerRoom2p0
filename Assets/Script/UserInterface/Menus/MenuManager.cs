using UnityEngine;

namespace UserInterface.Menus
{
	public class MenuManager : MonoBehaviour
    {
        public static MenuManager source { get; private set; }

        private float[] tempFloat;
        private int[] tempInt;
        private string[] tempString;
        private bool[] tempBool;

		protected void Start()
        {
            if(!source) source = this;
            else if(source != this) Destroy(this);
        }
    }
}

namespace UserInterface.Menus.Utility
{

}