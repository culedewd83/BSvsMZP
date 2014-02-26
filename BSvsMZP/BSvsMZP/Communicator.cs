using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace BSvsMZP
{
	public class Communicator
	{
		private int commPort;
		private bool shouldListen;
		private UdpClient udpServer;
		private IPEndPoint commEndPoint;
		private System.Threading.Thread listenerThread;
		private Hashtable messagesHashTable;
		private Dictionary<string, Common.EndPoint> cachedEndPoints;
		private Object thisLock = new Object();



		public Communicator(int commPort)
		{
			this.commPort = commPort;
			shouldListen = false;
			setupUDP();
			setupMessageHashTable();
			cachedEndPoints = new Dictionary<string, Common.EndPoint> ();
		}



		public Communicator()
		{
			commPort = 0;
			shouldListen = false;
			setupUDP();
			setupMessageHashTable();
			cachedEndPoints = new Dictionary<string, Common.EndPoint> ();
		}



		private void setupUDP () {
			commEndPoint = new IPEndPoint (IPAddress.Any, commPort);
			udpServer = new UdpClient (commEndPoint);
			commPort = getIncommingPort();

		}



		public bool changePort(int port) {

			int oldPortNum = commPort;
			commPort = port;

			try{
				if (shouldListen) {
					stopListening();
					startListening();
				} else {
					setupUDP();
				}
			}
			catch (Exception e) {
				Console.WriteLine(e.ToString());
				commPort = oldPortNum;
				return false;
			}

			return true;
		}



		public bool changePort() {

			int oldPortNum = commPort;
			commPort = 0;

			try{
				if (shouldListen) {
					stopListening();
					startListening();
				} else {
					setupUDP();
				}
			}
			catch (Exception e) {
				Console.WriteLine(e.ToString());
				commPort = oldPortNum;
				return false;
			}

			return true;
		}



		public void startListening() {
			shouldListen = true;
			listenForMessages();
		}



		public void stopListening () {
			shouldListen = false;
			udpServer.Close();
			setupUDP();
		}



		private void listenForMessages () {
			listenerThread = new System.Threading.Thread(delegate(){

				IPEndPoint receivedMessageEndPoint = new IPEndPoint(IPAddress.Any, 0);

				try{
					while(shouldListen){

						Byte[] receiveBytes = udpServer.Receive(ref receivedMessageEndPoint); 

						Common.ByteList bList = new Common.ByteList();
						bList.CopyFromBytes(receiveBytes);

						Common.EndPoint msgEP = new Common.EndPoint ();
						msgEP.Address = BitConverter.ToInt32(IPAddress.Parse(receivedMessageEndPoint.Address.ToString()).GetAddressBytes(), 0);
						msgEP.Port = receivedMessageEndPoint.Port;

						addMessageToHash(bList, msgEP);

					}
				}
				catch (Exception e) {
					Console.WriteLine(e.ToString());
				}
			});

			listenerThread.Start();
		}


		private void addMessageToHash (Common.ByteList byteList, Common.EndPoint msgEP) {

			System.Threading.Thread adderThread = new System.Threading.Thread(delegate(){
				string messageClassId = ((Messages.Message.MESSAGE_CLASS_IDS)byteList.PeekInt16()).ToString();
				string str = "Messages." + messageClassId + ",Messages";
				var type = Type.GetType(str);
				if (type != null) {
					var msg = Activator.CreateInstance(type);
					msg.GetType().GetMethod("Decode").Invoke(msg, new System.Object[]{byteList});
					Messages.MessageNumber convID = (Messages.MessageNumber)msg.GetType().GetProperty("ConversationId").GetValue(msg, null);
					string convStr = "" + convID.ProcessId + "," + convID.SeqNumber;
					lock (thisLock) {
						if (!cachedEndPoints.ContainsKey(convStr)) {
							cachedEndPoints.Add(convStr, msgEP);
						}
						messagesHashTable[messageClassId].GetType().GetMethod("Add").Invoke(messagesHashTable[messageClassId], new System.Object[]{msg});
					}
				}
			});

			adderThread.Start();
		}



		private void setupMessageHashTable () {

			messagesHashTable = new Hashtable();

			foreach (string s in Enum.GetNames(typeof(Messages.Message.MESSAGE_CLASS_IDS))) {
				string str = "Messages." + s + ",Messages";
				var type = Type.GetType(str);
				if (type != null) {
					Type genericListType = typeof(List<>).MakeGenericType(type);
					var msgList = Activator.CreateInstance(genericListType);
					messagesHashTable [s] = msgList;
				}
			}
		}



		public void sendMessage(byte[] msg, Common.EndPoint endPoint, Messages.MessageNumber convID) {

			string convStr = "" + convID.ProcessId + "," + convID.SeqNumber;

			lock (thisLock) {
				if (!cachedEndPoints.ContainsKey(convStr)) {
					cachedEndPoints.Add(convStr, endPoint);
				}
			}
			string ipAddress = new IPAddress(BitConverter.GetBytes(endPoint.Address)).ToString();
			udpServer.Send(msg, msg.Length, ipAddress, endPoint.Port);
		}



		public void sendMessage(byte[] msg, Messages.MessageNumber convID) {

			string convStr = "" + convID.ProcessId + "," + convID.SeqNumber;

			if (cachedEndPoints.ContainsKey(convStr)) {

				Common.EndPoint endPoint = cachedEndPoints [convStr];
				string ipAddress = new IPAddress(BitConverter.GetBytes(endPoint.Address)).ToString();
				udpServer.Send(msg, msg.Length, ipAddress, endPoint.Port);
			}
		}



		public int getIncommingPort() {
			return ((IPEndPoint)udpServer.Client.LocalEndPoint).Port;
		}



		public Hashtable getMessages() {

			Hashtable toReturn;

			lock (thisLock) {
				toReturn = (Hashtable)messagesHashTable.Clone();
				setupMessageHashTable();
			}

			return toReturn;
		}

	}
}

