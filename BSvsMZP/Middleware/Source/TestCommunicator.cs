﻿using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;

namespace Middleware
{
	public class TestCommunicator
	{
		public bool shouldListen;
		public UdpClient udpServer;
		public System.Threading.Thread listenerThread;
		public List<Envelope> envelopeBuffer;
		private Object thisLock = new Object();


		public TestCommunicator()
		{
			shouldListen = false;
			envelopeBuffer = new List<Envelope>();
			udpServer = new UdpClient(new IPEndPoint (IPAddress.Any, 0));
			udpServer.Client.ReceiveTimeout = 10;
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

						Common.ByteList byteList = new Common.ByteList();
						byteList.CopyFromBytes(receiveBytes);

						Common.EndPoint msgEP = new Common.EndPoint ();
						msgEP.Address = BitConverter.ToInt32(IPAddress.Parse(receivedMessageEndPoint.Address.ToString()).GetAddressBytes(), 0);
						msgEP.Port = receivedMessageEndPoint.Port;

						Messages.Message msg = Messages.Message.Create(byteList);

						if (msg.MessageTypeId() == Messages.Message.MESSAGE_CLASS_IDS.JoinGame) {
							Messages.JoinGame joinMsg = (Messages.JoinGame)msg;
							Common.AgentInfo aInfo = new Common.AgentInfo();
							aInfo.AgentStatus = Common.AgentInfo.PossibleAgentStatus.InGame;
							aInfo.AgentType = joinMsg.AgentInfo.AgentType;
							aInfo.Id = (short)1;
							aInfo.Location = new Common.FieldLocation(50, 50, false);
							aInfo.Strength = 100;
							aInfo.Speed = .25;
							Messages.AckNak reply = new Messages.AckNak(Messages.Reply.PossibleStatus.Success, aInfo);
							reply.ConversationId = joinMsg.ConversationId;
							reply.MessageNr = joinMsg.MessageNr;
							reply.MessageNr.SeqNumber += 1;
							Envelope replyEnvelope = new Envelope(reply, msgEP);
							sendEnvelope(replyEnvelope);
						}

						lock(thisLock) {
							envelopeBuffer.Add(new Envelope(msg, msgEP));
						}
					}
					catch (Exception e) {
						//Console.WriteLine(e.ToString());
					}
				}

				udpServer.Close();
				udpServer = new UdpClient(new IPEndPoint (IPAddress.Any, 0));
				udpServer.Client.ReceiveTimeout = 10;

			});

			listenerThread.Start();
		}


		public void sendEnvelope(Envelope msg) {
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

