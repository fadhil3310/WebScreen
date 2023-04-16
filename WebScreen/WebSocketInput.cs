using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EmbedIO;
using EmbedIO.WebSockets;
using System.Text.Json;

namespace WebScreen
{
		internal class WebSocketInput : WebSocketModule
		{
				public EventHandler<List<InputMessage>> OnInput;

				public WebSocketInput(string urlPath) : base(urlPath, true)
				{
				}

				protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
				{
						// Format: Type X Y, Example: 1 5 2
						// Type: 1 = Down, 2 = Up
						//Array.Reverse(buffer);
						string message = Encoding.Default.GetString(buffer);
						//MessageBox.Show(message);
						List<InputMessage> inputMessage = JsonSerializer.Deserialize<List<InputMessage>>(message);
						
						OnInput.Invoke(this, inputMessage);
						return Task.CompletedTask;
				}
		}

		public class InputMessage : EventArgs
		{
				public int Type { get; set; }
				public uint ID { get; set; }
				public int X { get; set; }
				public int Y { get; set; }

				public InputMessage(int type, uint id, int x, int y)
				{
						Type = type;
						ID = id;
						X = x;
						Y = y;
				}
		}
}
