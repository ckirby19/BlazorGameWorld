using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorGameWorld.Pages;
using blazorWords.Data;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver.Core.Connections;


namespace BlazorGameWorld.Hubs
{
	public class ConnectFourHub : Hub<IGameClient>
	{
		private ConcurrentDictionary<string, string> _clientList = new ConcurrentDictionary<string, string>();

		public override async Task OnConnectedAsync()
		{
			Console.WriteLine("Client Connected:" + this.Context.ConnectionId);
			if (_clientList.Count == 0)
			{
				string otherName = null;
				await Clients.Caller.GetConnectionId(this.Context.ConnectionId,otherName);
			}
			else if (_clientList.Count == 1)
			{
				await Clients.Others.SendUserInformation(this.Context.ConnectionId);
			}
			else if (_clientList.Count > 1)
			{
				await Clients.Caller.ComeBackLater();
			}
		}
		public override async Task OnDisconnectedAsync(Exception exception)
		{
			Console.WriteLine("Client Disconnected:" + Context.ConnectionId);
			if (_clientList.Count > 0)
			{
				string connectionId = Context.ConnectionId;
				string userName = _clientList.FirstOrDefault(entry => entry.Value == connectionId).Key;
				Console.WriteLine("Client Is Disconnected UserName:" + userName);
				if (userName != null)
				{
					_clientList.TryRemove(userName, out _);
				}
				if (_clientList.Count == 1 || _clientList.Count == 0)
				{
					await Clients.Others.RemoveUser(userName, connectionId);
				}
			}
			await base.OnDisconnectedAsync(exception);
		}

		public async Task Refresh()
		{
			await Clients.All.RefreshGame();
		}

		public async Task SendUserInfo(string otherUserName, string senderConnectionID)
		{
			await Clients.Client(senderConnectionID).GetUserInformation(otherUserName,senderConnectionID);
		}

		public async Task AddUser(string userName, string connectionID)
		{
			_clientList.TryAdd(userName, connectionID);
			if (_clientList.Count == 1){
				await Clients.All.RefreshGame();
			}
			else if (_clientList.Count == 0)
			{
				await Clients.All.ReceiveUser(userName, connectionID);
			}
		}

		public async Task OpenClient(int counter, string imgId, string lblId)
		{
			await Clients.Others.ReceiveOpen(counter, imgId, lblId);
		}

		public async Task SendPlacement(string userName, string connectionID)
		{
			await Clients.Others.ReceiveMove(userName, connectionID);
		}
	}
}
