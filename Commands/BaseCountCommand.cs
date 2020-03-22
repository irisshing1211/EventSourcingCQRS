namespace EventSourcingCQRS.Commands
{
    public class BaseCountCommand
    {
        public string Id { get; set; }
        public int NewValue { get; set; }
    }
}
