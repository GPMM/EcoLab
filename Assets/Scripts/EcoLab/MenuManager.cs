using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EcoLab
{
    public class MenuManager : MonoBehaviour
    {
        #region Serialized variables
        [SerializeField]
        private GameObject startingMenu;
        #endregion

        #region Local variables
        private List<GameObject> menus = new List<GameObject>();
        private GameObject activeMenu = null;
        #endregion Properties

        #region Methods
        private void Awake()
        {
            activeMenu = startingMenu;

            // Adds every panel under this MenuContainer as a Menu.
            foreach (Transform child in transform)
            {
                GameObject menu = child.gameObject;

                menus.Add(menu);

                // Sets the menu as active only if it is the starting menu, otherwise sets it to inactive.
                menu.SetActive(menu == startingMenu);
            }
        }

        public void UIMoveTo(GameObject targetMenu)
        {
            if (menus.Exists(x => x.gameObject == targetMenu))
            {
                activeMenu.SetActive(false);

                targetMenu.SetActive(true);

                activeMenu = targetMenu;
            }
        }
        #endregion Methods
    }
}
