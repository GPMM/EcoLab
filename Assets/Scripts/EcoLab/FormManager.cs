using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EcoLab
{
    public class FormManager : MonoBehaviour
    {
        #region Serialized variables
        [SerializeField]
        private InputField userIdField;
        #endregion Serialized variables

        #region Methods
        private void Start()
        {
            // PROTO: get all inputfields into a Key/Value dictionary
            //GetComponentsInChildren<InputField>();

            ErrorHandler.AssertNull(userIdField);
        }

        public void Submit(string sceneName)
        {
            MetadataManager.Instance.SetUserID(userIdField.text);

            SceneManager.LoadScene(sceneName);
        }
        #endregion Methods
    }
}