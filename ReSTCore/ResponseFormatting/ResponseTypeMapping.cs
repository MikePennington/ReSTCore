namespace ReSTCore.ResponseFormatting
{
    public class ResponseTypeMapping
    {
        public ResponseTypeMapping(string mimeType, ResponseFormatType responseFormatType)
        {
            MimeType = mimeType;
            ResponseFormatType = responseFormatType;
        }

        public string MimeType { get; set; }

        public ResponseFormatType ResponseFormatType { get; set; }
    }
}
