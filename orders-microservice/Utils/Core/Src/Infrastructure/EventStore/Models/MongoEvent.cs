namespace Application.Core
{
    public class MongoEvent(string stream, string type, string data, DateTime ocurredDate)
    {
        public string Stream = stream;
        public string Type = type;
        public string Data = data;
        public DateTime OcurredDate = ocurredDate;
    }
}

