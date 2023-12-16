namespace AuthService.Business.Exceptions
{
    public class HttpStatusException : Exception
    {
        public int Status { get; private set; }

        public HttpStatusException(int status, string msg) : base(msg)
        {
            Status = status;
        }
    }
}
