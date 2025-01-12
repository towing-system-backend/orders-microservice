using System.Reflection;

namespace RabbitMQ.Contracts
{
    public abstract class DtoCreator<IRabbitMQMessage, IDto>
    {
        public static IDto GetDto(IRabbitMQMessage message)
        {
            var messageType = message!.GetType();
            var messageTypeName = messageType.Name;
            var messageNamespace = messageType.Namespace;
            var creatorTypeName = $"{messageNamespace}.{messageTypeName}DtoCreator";
            var messageAssembly = messageType.Assembly.FullName;
            var fullTypeName = $"{creatorTypeName}, {messageAssembly}";
            var type = Type.GetType(fullTypeName);
            if (type == null) throw new InvalidOperationException($"Type: {fullTypeName} not found.");

            var creator = Activator.CreateInstance(type);
            if (creator == null) throw new InvalidOperationException($"Failed to create an instance of:: {fullTypeName}");

            var createDtoMethod = type.GetMethod(nameof(CreateDto), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (createDtoMethod == null) throw new InvalidOperationException($"The static method CreateDto could not be found in the type: {fullTypeName}");

            var dto = (IDto)createDtoMethod.Invoke(creator, new object[] { message })!;

            return dto;
        }
        public abstract IDto CreateDto(IRabbitMQMessage message);
    }

}