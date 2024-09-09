namespace Marsion
{
    public class ServerGameData : IServerGameData
    {
        public IServerPlayer[] Players => throw new System.NotImplementedException();

        public ServerGameData(int count)
        {

        }
    }
}