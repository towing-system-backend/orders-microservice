namespace Application.Core
{
    public class GuidGenerator : IdService<string>
    {
        public string GenerateId()
        {
             return Guid.NewGuid().ToString();
        }
    }
}