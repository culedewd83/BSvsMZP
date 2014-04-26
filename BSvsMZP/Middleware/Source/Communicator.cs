using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;

namespace Middleware
{
	public class Communicator
	{
		public bool shouldListen;
		public UdpClient udpServer;
		public System.Threading.Thread listenerThread;
		public List<Envelope> envelopeBuffer;
		private Object thisLock = new Object();


		public Communicator()
		{
			shouldListen = false;
			envelopeBuffer = new List<Envelope>();
			udpServer = new UdpClient(new IPEndPoint (IPAddress.Any, 0));
			udpServer.Client.ReceiveTimeout = 1000;
		}
			

		public void startListening() {
			shouldListen = true;
			listenForMessages();
		}


		public void stopListening () {
			shouldListen = false;
		}
			

		private void listenForMessages () {
			listenerThread = new System.Threading.Thread(delegate(){

				IPEndPoint receivedMessageEndPoint = new IPEndPoint(IPAddress.Any, 0);

				while(shouldListen){
					try{
						Byte[] receiveBytes = udpServer.Receive(ref receivedMessageEndPoint); 
						Console.WriteLine("packet received");
						Common.ByteList byteList = new Common.ByteList();
						byteList.CopyFromBytes(receiveBytes);

						Common.EndPoint msgEP = new Common.EndPoint ();
						msgEP.Address = BitConverter.ToInt32(IPAddress.Parse(receivedMessageEndPoint.Address.ToString()).GetAddressBytes(), 0);
						msgEP.Port = receivedMessageEndPoint.Port;

						lock(thisLock) {
							Messages.Message msg = Messages.Message.Create(byteList);
							envelopeBuffer.Add(new Envelope(msg, msgEP));

							Console.WriteLine("received a message: " + msg.MessageTypeId());
						}
					}
					catch (Exception e) {
						//Console.WriteLine(e.ToString());
					}
				}

				udpServer.Close();
				udpServer = new UdpClient(new IPEndPoint (IPAddress.Any, 0));
				udpServer.Client.ReceiveTimeout = 1000;
				
			});

			listenerThread.Start();
		}
			

		public void sendEnvelope(Envelope msg) {
			Console.WriteLine("message sent: " + msg.message.MessageTypeId());
			Common.ByteList byteList = new Common.ByteList ();
			msg.message.Encode(byteList);
			byte[] bytes = byteList.ToBytes();
			string ipAddress = new IPAddress(BitConverter.GetBytes(msg.endPoint.Address)).ToString();
			udpServer.Send(bytes, bytes.Length, ipAddress, msg.endPoint.Port);
		}


		public List<Envelope> getEnvelopes() {
			List<Envelope> toReturn;
			lock (thisLock) {
				toReturn = envelopeBuffer;
				envelopeBuffer = new List<Envelope>();
			}
			return toReturn;
		}

		public int getPort() {
			return ((IPEndPoint)udpServer.Client.LocalEndPoint).Port;
		}
	}
}

