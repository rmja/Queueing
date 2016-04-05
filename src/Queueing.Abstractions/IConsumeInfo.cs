namespace Queueing
{
    public interface IConsumeInfo
    {
        string Exchange { get; }
        string Route { get; }
        byte[] Body { get; }
    }
}
