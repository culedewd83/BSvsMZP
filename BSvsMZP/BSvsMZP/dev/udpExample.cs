using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Common;

namespace BSvsMZP
{
	public class udpExample
	{


		public udpExample()
		{
		}


		public void receiveMessage (int port) {
			System.Threading.Thread t1 = new System.Threading.Thread
			                             (delegate()
				{

					try{

						Console.WriteLine("Begin listening on port: " + port + "...");

						IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);

						UdpClient udpServer = new UdpClient (RemoteIpEndPoint);


						Byte[] receiveBytes = udpServer.Receive(ref RemoteIpEndPoint); 
						string returnData = Encoding.ASCII.GetString(receiveBytes);

						// Uses the IPEndPoint object to determine which of these two hosts responded.
						Console.WriteLine("This is the message you received " +
							returnData.ToString());
						Console.WriteLine("This message was sent from " +
							RemoteIpEndPoint.Address.ToString() +
							" on their port number " +
							RemoteIpEndPoint.Port.ToString());
					}
					catch (Exception e ) {
						Console.WriteLine(e.ToString());
					}
					Console.WriteLine("End listening...");
				});
			t1.Start();
		}



		public void sendMessage(string remoteAddress, int remotePort, string messageStr)
		{
			try{
				UdpClient udpClient = new UdpClient (0);

				udpClient.Connect(remoteAddress, remotePort);
				Byte[] messageBytes = Encoding.ASCII.GetBytes(messageStr);
				//udpClient.Send(messageBytes, messageBytes.Length);

				Messages.JoinGame test = new Messages.JoinGame();
				test.ANumber = "A01537812";
				ByteList bytes = new ByteList();
				test.Encode(bytes);
				messageBytes = bytes.ToBytes();
				Console.WriteLine("num of bytes: " + messageBytes.Length);
				udpClient.Send(messageBytes, messageBytes.Length);

				Console.WriteLine("Message sent");
			}  
			catch (Exception e ) {
				Console.WriteLine(e.ToString());
			}
		}


	}
}

