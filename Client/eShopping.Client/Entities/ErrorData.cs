namespace eShopping.Client.Entities
{
    public class ErrorData
    {
        public ErrorData(string uri, int error, string message, string debugMessage)
        {
            this.Uri = uri;
            this.Error = error;
            this.Message = message;
            this.DebugMessage = debugMessage;
        }
        public DateTime DateTimeCreated { get; set; } = DateTime.Now;
        public int Id { get; set; }
        public string Uri { get; set; }
        public int Error { get; set; }
        public string Message { get; set; }
        public string DebugMessage { get; set; }
    }
}
