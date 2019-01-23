using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class NetworkManager : MonoBehaviour
    {
        private ClientTCP ClientTCP = new ClientTCP();
        [SerializeField] private GameObject playerPref;
        private Dictionary<int, GameObject> playerList = new Dictionary<int, GameObject>();
        public int myIndex;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            UnityThread.initUnityThread();
        }

        // Start is called before the first frame update
        void Start()
        {
            ClientTCP.Connect();
        }

        public void InstantiatePlayer(int index)
        {
            GameObject temp = Instantiate(playerPref);
            temp.name = "Player: " + index;
            playerList.Add(index, temp);
        }
    }
}