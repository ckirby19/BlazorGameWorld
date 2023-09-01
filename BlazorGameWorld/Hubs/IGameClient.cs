namespace BlazorGameWorld.Hubs
{
	public interface IGameClient
	{
		Task SendUserInformation(string connectionID);
		Task GetConnectionId(string connectionID, string otherName);
		Task ComeBackLater();
		Task GetUserInformation(string otherUserName, string senderConnectionID);
		Task RefreshGame();
		Task ReceiveMove(string userName, string connectionID);
		Task ReceiveUser(string userName, string connectionID);
		Task ReceiveOpen(int counter, string imgId, string lblId);
		Task RemoveUser(string userName, string connectionID);

	}
}
