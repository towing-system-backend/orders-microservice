namespace Application.Core
{
    public class ExceptionCatcher<T,U>(IService<T, U> service, Func<Exception, Exception> exceptionParser) : IService<T, U> 
    {
        private readonly IService<T, U> _service = service;
        private readonly Func<Exception, Exception> _exceptionParser = exceptionParser; 
        async public Task<Result<U>> Execute(T data)
        {
            try
            {
                var res = await _service.Execute(data);
                if (res.IsError) res.Unwrap();
                return res;
            }
            catch (Exception e) 
            { 
                throw _exceptionParser(e);
            }
        }
    }
}