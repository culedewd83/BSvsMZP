using System;
using System.Collections.Generic;

namespace Middleware
{
	public class MessagesLists
	{

		public List<Messages.Request> Request;						// 100
		//public List<Messages.GameAnnouncement> GameAnnouncement; 	// 101
		public List<Messages.JoinGame> JoinGame;					// 102
		public List<Messages.AddComponent> AddComponent;			// 103
		public List<Messages.RemoveComponent> RemoveComponent;		// 104
		public List<Messages.StartGame> StartGame;					// 105
		public List<Messages.EndGame> EndGame;						// 106
		public List<Messages.GetResource> GetResource;				// 107
		public List<Messages.TickDelivery> TickDelivery;			// 108
		public List<Messages.ValidateTick> ValidateTick;			// 109
		public List<Messages.Move> Move;							// 110
		public List<Messages.ThrowBomb> ThrowBomb;					// 111
		public List<Messages.Eat> Eat;								// 112
		//public List<Messages.ChangeStrength> ChangeStrength;		// 113
		public List<Messages.Collaborate> Collaborate;				// 114
		public List<Messages.GetStatus> GetStatus;					// 115
		public List<Messages.Reply> Reply;							// 200
		public List<Messages.AckNak> AckNak;						// 201

		public MessagesLists()
		{
			Request = new List<Messages.Request>();
			//GameAnnouncement = new List<Messages.GameAnnouncement>();
			JoinGame = new List<Messages.JoinGame>();
			AddComponent = new List<Messages.AddComponent>();
			RemoveComponent = new List<Messages.RemoveComponent>();
			StartGame = new List<Messages.StartGame>();
			EndGame = new List<Messages.EndGame>();
			GetResource = new List<Messages.GetResource>();
			TickDelivery = new List<Messages.TickDelivery>();
			ValidateTick = new List<Messages.ValidateTick>();
			Move = new List<Messages.Move>();
			ThrowBomb = new List<Messages.ThrowBomb>();
			Eat = new List<Messages.Eat>();
			//ChangeStrength = new List<Messages.ChangeStrength>();
			Collaborate = new List<Messages.Collaborate>();
			GetStatus = new List<Messages.GetStatus>();
			Reply = new List<Messages.Reply>();
			AckNak = new List<Messages.AckNak>();
		}
	}
}

