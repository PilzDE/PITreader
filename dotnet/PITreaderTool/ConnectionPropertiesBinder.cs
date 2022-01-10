using System.CommandLine;
using System.CommandLine.Binding;

namespace Pilz.PITreader.Tool
{
    internal class ConnectionPropertiesBinder : BinderBase<ConnectionProperties>
    {
        private readonly Option<string> host;
        private readonly Option<ushort> port;
        private readonly Option<bool> acceptAll;
        private readonly Option<string> thumbprint;
        private readonly Argument<string> apiToken;

        public ConnectionPropertiesBinder(Option<string> host, Option<ushort> port, Option<bool> acceptAll, Option<string> thumbprint, Argument<string> apiToken)
        {
            this.host = host;
            this.port = port;
            this.acceptAll = acceptAll;
            this.thumbprint = thumbprint;
            this.apiToken = apiToken;
        }

        protected override ConnectionProperties GetBoundValue(BindingContext bindingContext)
        {
            var properties = new ConnectionProperties
            {
                Host = bindingContext.ParseResult.GetValueForOption(this.host),
                Port = bindingContext.ParseResult.GetValueForOption(this.port),
                AcceptAll = bindingContext.ParseResult.GetValueForOption(this.acceptAll),
                Thumbprint = bindingContext.ParseResult.GetValueForOption(this.thumbprint),
                ApiToken = bindingContext.ParseResult.GetValueForArgument(this.apiToken)
            };

            return properties;
        }
    }
}