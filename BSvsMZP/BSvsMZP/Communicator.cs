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
		private int port;
		private bool shouldListen;
		private UdpClient udpServer;
		private IPEndPoint RemoteIpEndPoint;
		private System.Threading.Thread listenerThread;
		private Hashtable messagesHashTable;
		private Object thisLock = new Object();

		public Communicator(int port)
		{
			this.port = port;
			shouldListen = false;
			setupUDP();
			setupMessageHashTable();
		}

		private void setupUDP () {
			RemoteIpEndPoint = new IPEndPoint (IPAddress.Any, port);
			udpServer = new UdpClient (RemoteIpEndPoint);
		}

		public void changePort(int port) {
			this.port = port;
			if (shouldListen) {
				stopListening();
				startListening();
			} else {
				setupUDP();
			}
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

				try{
					while(shouldListen){

						Byte[] receiveBytes = udpServer.Receive(ref RemoteIpEndPoint); 

						Common.ByteList bList = new Common.ByteList();
						bList.CopyFromBytes(receiveBytes);
						addMessageToHash(bList);





						/*
						// Uses the IPEndPoint object to determine which of these two hosts responded.
						Console.WriteLine("This is the message you received ");
							//returnData.ToString());
						Console.WriteLine("This message was sent from " +
							RemoteIpEndPoint.Address.ToString() +
							" on their port number " +
							RemoteIpEndPoint.Port.ToString());
						*/
					}
				}
				catch (Exception e) {
					Console.WriteLine(e.ToString());
				}
				Console.WriteLine("End listening...");
			});

			listenerThread.Start();
		}


		private void addMessageToHash (Common.ByteList byteList) {

			System.Threading.Thread adderThread = new System.Threading.Thread(delegate(){
				string messageClassId = ((Messages.Message.MESSAGE_CLASS_IDS)byteList.PeekInt16()).ToString();
				string str = "Messages." + messageClassId + ",Messages";
				var type = Type.GetType(str);
				if (type != null) {
					var msg = Activator.CreateInstance(type);
					msg.GetType().GetMethod("Decode").Invoke(msg, new System.Object[]{byteList});
					lock (thisLock) {
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


	}
}

