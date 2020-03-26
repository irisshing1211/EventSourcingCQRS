namespace EventSourcingCQRS.Models
{
    public class DbSettings : IDbSettings
    {
        public string CollectionName { get; set; }
        public string NoSqlConnString { get; set; }
        public string DatabaseName { get; set; }
        public string RelationalConnString { get; set; }
    }

    public interface IDbSettings
    {
        string CollectionName { get; set; }
        string NoSqlConnString { get; set; }
        string DatabaseName { get; set; }
        string RelationalConnString { get; set; }
    }
    
}
