namespace ContentNetworkSystem.Models
{
    /// <summary>
    /// used to ensure scheduler service runs only once at same time
    /// </summary>
    public class Scheduler
    {
        public int ID { get; set; }
        public int? RequestId { get; set; }
    }
}
