namespace Backsplashed.Interop.Services
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using MediatR;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using NJsonSchema.Generation;

    // MediatorInteropAdapter
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class WebMessageBinder
    {
        private readonly IMediator _mediator;

        public WebMessageBinder(IMediator mediator)
        {
            _mediator = mediator;
        }

        public string GenerateInteropSchema()
        {
            var settings = new JsonSchemaGeneratorSettings
            {
                SerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                },
                AlwaysAllowAdditionalObjectProperties = false,
                DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.Null
            };

            var generator = new JsonSchemaGenerator(settings);
            var baseType = typeof(IBaseRequest);
            var schemas = GetType().Assembly.GetTypes()
                .Where(p => baseType.IsAssignableFrom(p))
                .ToDictionary(type => type.FullName, type => generator.Generate(type));

            return JsonConvert.SerializeObject(schemas);
        }

        public async Task<string> ProcessInteropCommand(string commandJson)
        {
            var requestMessage = JsonConvert.DeserializeObject<WebMessageRequest>(commandJson);
            var messageType = Type.GetType(requestMessage.Type);
            if (messageType == null)
            {
                throw new InvalidOperationException($"'{requestMessage.Type}' is not a valid interop command type.");
            }

            var request = JsonConvert.DeserializeObject(requestMessage.JsonData, messageType);
            var response = await _mediator.Send(request!);

            if (string.IsNullOrWhiteSpace(requestMessage.Callback) || response == null)
            {
                return string.Empty;
            }

            var responseMessage = new WebMessageResponse
            {
                Callback = requestMessage.Callback,
                JsonData = JsonConvert.SerializeObject(response)
            };

            return JsonConvert.SerializeObject(responseMessage);
        }
    }
}