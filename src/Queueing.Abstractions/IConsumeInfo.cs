namespace Queueing
{
    public interface IConsumeInfo
    {
        string Route { get; }
        byte[] Body { get; }
    }
}
