using System;
using System.Collections;
using System.Collections.Generic;

namespace BSvsMZP
{
	public class Listener
	{

		private Communicator comm;
		private MessageQueue msgQueue;
		private System.Threading.Thread listenerThread;
		private bool shouldListen;


		public Listener(Communicator comm, MessageQueue msgQueue)
		{
			this.comm = comm;
			this.msgQueue = msgQueue;
			shouldListen = false;
		}


		public void startListening() {
			shouldListen = true;
			listenForMessages();
		}


		public void stopListening () {
			shouldListen = false;
		}



		private void listenForMessages () {
			listenerThread = new System.Threading.Thread (delegate() {

				int messagesMoved = 0;
				while (shouldListen) {
					messagesMoved = 0;
				
					Hashtable messagesFromComm = comm.getMessages();
					foreach (DictionaryEntry entry in messagesFromComm)
					{
						switch(entry.Key.ToString())
						{
						case "Request":
							foreach(Messages.Request msg in (List<Messages.Request>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].Request.Add(msg);
								} else {
									msgQueue.newConvMessages.Request.Add(msg);
								}
							}
							break;
						case "GameAnnouncement":
							foreach(Messages.GameAnnouncement msg in (List<Messages.GameAnnouncement>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].GameAnnouncement.Add(msg);
								} else {
									msgQueue.newConvMessages.GameAnnouncement.Add(msg);
								}
							}
							break;
						case "JoinGame":
							foreach(Messages.JoinGame msg in (List<Messages.JoinGame>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].JoinGame.Add(msg);
								} else {
									msgQueue.newConvMessages.JoinGame.Add(msg);
								}
							}
							break;
						case "AddComponent":
							foreach(Messages.AddComponent msg in (List<Messages.AddComponent>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].AddComponent.Add(msg);
								} else {
									msgQueue.newConvMessages.AddComponent.Add(msg);
								}
							}
							break;
						case "RemoveComponent":
							foreach(Messages.RemoveComponent msg in (List<Messages.RemoveComponent>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].RemoveComponent.Add(msg);
								} else {
									msgQueue.newConvMessages.RemoveComponent.Add(msg);
								}
							}
							break;
						case "StartGame":
							foreach(Messages.StartGame msg in (List<Messages.StartGame>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].StartGame.Add(msg);
								} else {
									msgQueue.newConvMessages.StartGame.Add(msg);
								}
							}
							break;
						case "EndGame":
							foreach(Messages.EndGame msg in (List<Messages.EndGame>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].EndGame.Add(msg);
								} else {
									msgQueue.newConvMessages.EndGame.Add(msg);
								}
							}
							break;
						case "GetResource":
							foreach(Messages.GetResource msg in (List<Messages.GetResource>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].GetResource.Add(msg);
								} else {
									msgQueue.newConvMessages.GetResource.Add(msg);
								}
							}
							break;
						case "TickDelivery":
							foreach(Messages.TickDelivery msg in (List<Messages.TickDelivery>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].TickDelivery.Add(msg);
								} else {
									msgQueue.newConvMessages.TickDelivery.Add(msg);
								}
							}
							break;
						case "ValidateTick":
							foreach(Messages.ValidateTick msg in (List<Messages.ValidateTick>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].ValidateTick.Add(msg);
								} else {
									msgQueue.newConvMessages.ValidateTick.Add(msg);
								}
							}
							break;
						case "Move":
							foreach(Messages.Move msg in (List<Messages.Move>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].Move.Add(msg);
								} else {
									msgQueue.newConvMessages.Move.Add(msg);
								}
							}
							break;
						case "ThrowBomb":
							foreach(Messages.ThrowBomb msg in (List<Messages.ThrowBomb>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].ThrowBomb.Add(msg);
								} else {
									msgQueue.newConvMessages.ThrowBomb.Add(msg);
								}
							}
							break;
						case "Eat":
							foreach(Messages.Eat msg in (List<Messages.Eat>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].Eat.Add(msg);
								} else {
									msgQueue.newConvMessages.Eat.Add(msg);
								}
							}
							break;
						case "Collaborate":
							foreach(Messages.Collaborate msg in (List<Messages.Collaborate>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].Collaborate.Add(msg);
								} else {
									msgQueue.newConvMessages.Collaborate.Add(msg);
								}
							}
							break;
						case "GetStatus":
							foreach(Messages.GetStatus msg in (List<Messages.GetStatus>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].GetStatus.Add(msg);
								} else {
									msgQueue.newConvMessages.GetStatus.Add(msg);
								}
							}
							break;
						case "Reply":
							foreach(Messages.Reply msg in (List<Messages.Reply>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].Reply.Add(msg);
								} else {
									msgQueue.newConvMessages.Reply.Add(msg);
								}
							}
							break;
						case "AckNak":
							foreach(Messages.AckNak msg in (List<Messages.AckNak>)entry.Value) {
								messagesMoved++;
								string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
								if (msgQueue.convInProgressMessages.ContainsKey(convKey)){
									msgQueue.convInProgressMessages[convKey].AckNak.Add(msg);
								} else {
									msgQueue.newConvMessages.AckNak.Add(msg);
								}
							}
							break;
						}

					}


					if (messagesMoved == 0) {
						// Be nice, sleep for a awhile...
						System.Threading.Thread.Sleep(50);
					}
				}
			});

			listenerThread.Start();
		}

	}
}

