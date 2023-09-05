namespace BlazorGameWorld.Hubs
{
	public interface IGameClient
	{
		Task SendUserInformation(string connectionId);
		Task GetConnectionId(string connectionId, string otherUser);
		Task ComeBackLater();
		Task GetUserInformation(string otherUserName, string senderConnectionId);
		Task RefreshGame();
		Task ReceiveMove(string userName, string connectionId);
		Task ReceiveUser(string userName, string connectionId);
		Task Registered(string userName);
		Task FindOpponent();
		Task WaitForOpponent();
		Task FoundOpponent(string opponentName, int playerNum);
		Task ReceiveOpen(int counter, string imgId, string lblId);
		Task RemoveUser(string userName, string connectionId);

	}
}
