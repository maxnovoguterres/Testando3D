using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class ClientHandleData : MonoBehaviour
    {
        public static ByteBuffer playerBuffer;
        private delegate void Packet_(byte[] data);
        private static Dictionary<long, Packet_> packets = new Dictionary<long, Packet_>();
        private static NetworkManager networkManager;
        private static long pLength;

        private void Awake()
        {
            networkManager = GetComponent<NetworkManager>();
            InitMassages();
        }

        public static void InitMassages()
        {
            Debug.Log("Initializing Network Messages...");
            packets.Add((long)ServerPackets.Welcome, Packet_Welcome);
            packets.Add((long)ServerPackets.JoinMap, Packet_JoinMap);
        }

        public static void HandleData(byte[] data)
        {
            byte[] buffer;
            buffer = (byte[])data.Clone();

            if (playerBuffer == null) playerBuffer = new ByteBuffer();
            playerBuffer.WriteBytes(buffer);

            if (playerBuffer.Count() == 0)
            {
                playerBuffer.Clear();
                return;
            }

            if (playerBuffer.Length() >= 8)
            {
                pLength = playerBuffer.ReadLong(false);
                if (pLength <= 0)
                {
                    playerBuffer.Clear();
                    return;
                }
            }
            //if (playerBuffer.Length() >= 8)
            //{
            //    pLength = playerBuffer.ReadLong(false);
            //    if (pLength <= 0)
            //    {
            //        playerBuffer.Clear();
            //        return;
            //    }
            //}

            while (pLength > 0 || pLength <= playerBuffer.Length() - 8)
            {
                if (pLength <= playerBuffer.Length() - 8)
                {
                    playerBuffer.ReadLong();
                    data = playerBuffer.ReadBytes((int)pLength);
                    HandleDataPackets(data);
                }
                pLength = 0;

                if (playerBuffer.Length() >= 8)
                {
                    pLength = playerBuffer.ReadLong(false);
                    if (pLength < 0)
                    {
                        playerBuffer.Clear();
                        return;
                    }
                }
            }
        }

        public static void HandleDataPackets(byte[] data)
        {
            long packetNum; ByteBuffer buffer; Packet_ packet;

            buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            packetNum = buffer.ReadLong();
            buffer = null;

            if (packetNum == 0) return;

            if (packets.TryGetValue(packetNum, out packet))
            {
                packet.Invoke(data);
            }
        }

        private static void Packet_Welcome(byte[] data)
        {
            long packetNum; ByteBuffer buffer;
            buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            packetNum = buffer.ReadLong();
            int index = buffer.ReadInteger();
            networkManager.index = index;
            networkManager.InstantiatePlayer(index);
            buffer = null;
        }

        private static void Packet_JoinMap(byte[] data)
        {
            long packetNum; ByteBuffer buffer;
            buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            packetNum = buffer.ReadLong();
            int index = buffer.ReadInteger();

            if (index == networkManager.index) return;

            networkManager.InstantiatePlayer(index);
            buffer = null;
        }
    }
}
