using EcoLab.TimeManaging.Domain;
using UnityEngine;

namespace EcoLab
{
    public class MetadataManager : MonoBehaviour
    {
        public string UserId { get; private set; }

        public static MetadataManager Instance { get; private set; }

        private void Awake()
        {
            ErrorHandler.AssertExists(Instance);

            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        public void SetUserID(string userID)
        {
            UserId = userID;
        }
    }
}