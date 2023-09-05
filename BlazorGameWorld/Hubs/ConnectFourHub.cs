using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using BlazorGameWorld.Pages;
using BlazorGameWorld.Shared;
using blazorWords.Data;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver.Core.Connections;


namespace BlazorGameWorld.Hubs
{
	public class Client
	{
		public string Name { get; set; }
		public Client Opponent { get; set; }
		public bool IsPlaying { get; set; }
		public bool WaitingForMove { get; set; }
		public bool LookingForOpponent { get; set; }
		public string ConnectionId { get; set; }
	}
	public class ConnectFourHub : Hub<IGameClient>
	{
		private static ConcurrentBag<Client> _clientList = new ConcurrentBag<Client>();
		private static ConcurrentBag<ConnectFourGameState> _games = new ConcurrentBag<ConnectFourGameState>();

		//private static ConcurrentDictionary<Tuple<int, int>, ConnectFourGameState> _states = 
		//	new ConcurrentDictionary<Tuple<int, int>, ConnectFourGameState>();

		public async Task RegisterClient(string userName)
		{
			var client = _clientList.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
			if (client == null)
			{
				client = new Client {ConnectionId = Context.ConnectionId, Name = userName};
				_clientList.Add(client);
			}

			client.IsPlaying = false;
			await Clients.Client(Context.ConnectionId).Registered(userName);
		}

		public async Task FindOpponent()
		{
			var player = _clientList.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
			if (player == null) return;
			player.LookingForOpponent = true;

			var opponent = _clientList.FirstOrDefault(x => x.ConnectionId != Context.ConnectionId &&
			                                               x.LookingForOpponent &&
			                                               !x.IsPlaying);
			if (opponent == null)
			{
				await Clients.Client(Context.ConnectionId).WaitForOpponent();
			}
			else
			{
				player.IsPlaying = true;
				player.LookingForOpponent = false;
				player.Opponent = opponent;

				opponent.IsPlaying = true;
				opponent.LookingForOpponent = false;
				opponent.Opponent = player;

				// Notify both players that a game was found
				await Clients.Client(Context.ConnectionId).FoundOpponent(opponent.Name, 1);
				await Clients.Client(opponent.ConnectionId).FoundOpponent(player.Name, 2);

				await Clients.Client(Context.ConnectionId).UserTurn();
				await Clients.Client(opponent.ConnectionId).OpponentTurn();

				_games.Add(new ConnectFourGameState {Player1 = player, Player2 = opponent});

			}
		}

		public async Task UserMadeMove(byte col)
		{
			var game = _games.FirstOrDefault(x =>
				x.Player1.ConnectionId == Context.ConnectionId ||
				x.Player2.ConnectionId == Context.ConnectionId);
			if (game == null) return;

			var landingRow = game.PlayPiece(col);
			var cssClass = $"player{game.PlayerTurn} col{col} drop{landingRow}";
			await Clients.Client(game.Player1.ConnectionId).UpdateGame(game.CurrentTurn - 1,cssClass);
			await Clients.Client(game.Player2.ConnectionId).UpdateGame(game.CurrentTurn - 1, cssClass);
			
			// Need to update turn now, but how to check who made the move?
			if (game.PlayerTurn == 1)
			{
				await Clients.Client(game.Player1.ConnectionId).UserTurn();
				await Clients.Client(game.Player2.ConnectionId).OpponentTurn();
			}
			else if (game.PlayerTurn == 2)
			{
				await Clients.Client(game.Player1.ConnectionId).OpponentTurn();
				await Clients.Client(game.Player2.ConnectionId).UserTurn();
			}

		}

		//public override async Task OnConnectedAsync()
		//{
		//	Console.WriteLine("Client Connected:" + this.Context.ConnectionId);
		//	if (_clientList.Count == 0)
		//	{
		//		string otherUser = null;
		//		await Clients.Caller.GetConnectionId(this.Context.ConnectionId,otherUser);
		//	}
		//	else if (_clientList.Count == 1)
		//	{
		//		await Clients.Others.SendUserInformation(this.Context.ConnectionId);
		//	}
		//	else if (_clientList.Count > 1)
		//	{
		//		await Clients.Caller.ComeBackLater();
		//	}
		//}
		//public override async Task OnDisconnectedAsync(Exception exception)
		//{
		//	Console.WriteLine("Client Disconnected:" + Context.ConnectionId);
		//	if (_clientList.Count > 0)
		//	{
		//		string connectionId = Context.ConnectionId;
		//		string userName = _clientList.FirstOrDefault(entry => entry.Value == connectionId).Key;
		//		Console.WriteLine("Client Is Disconnected UserName:" + userName);
		//		if (userName != null)
		//		{
		//			_clientList.TryRemove(userName, out _);
		//		}
		//		if (_clientList.Count == 1 || _clientList.Count == 0)
		//		{
		//			await Clients.Others.RemoveUser(userName, connectionId);
		//		}
		//	}
		//	await base.OnDisconnectedAsync(exception);
		//}

		//public async Task Refresh()
		//{
		//	await Clients.All.RefreshGame();
		//}

		//public async Task SendUserInfo(string otherUserName, string senderConnectionId)
		//{
		//	await Clients.Client(senderConnectionId).GetUserInformation(otherUserName,senderConnectionId);
		//}

		//public async Task AddUser(string userName, string connectionId)
		//{
		//	_clientList.TryAdd(userName, connectionId);
		//	if (_clientList.Count == 1){
		//		await Clients.All.RefreshGame();
		//	}
		//	else if (_clientList.Count == 0)
		//	{
		//		await Clients.All.ReceiveUser(userName, connectionId);
		//	}
		//}

		//public async Task OpenClient(int counter, string imgId, string lblId)
		//{
		//	await Clients.Others.ReceiveOpen(counter, imgId, lblId);
		//}

		//public async Task SendPlacement(string userName, string connectionId)
		//{
		//	await Clients.Others.ReceiveMove(userName, connectionId);
		//}
	}
}
