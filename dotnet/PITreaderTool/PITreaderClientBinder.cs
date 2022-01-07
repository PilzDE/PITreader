using Pilz.PITreader.Client;
using System.CommandLine;
using System.CommandLine.Binding;

namespace Pilz.PITreader.Tool
{
    public class PITreaderClientBinder : BinderBase<PITreaderClient>
    {
        private readonly Option<string> host;
        private readonly Option<ushort> port;
        private readonly Option<bool> acceptAll;
        private readonly Option<string> thumbprint;
        private readonly Argument<string> apiToken;

        public PITreaderClientBinder(Option<string> host, Option<ushort> port, Option<bool> acceptAll, Option<string> thumbprint, Argument<string> apiToken)
        {
            this.host = host;
            this.port = port;
            this.acceptAll = acceptAll;
            this.thumbprint = thumbprint;
            this.apiToken = apiToken;
        }

        protected override PITreaderClient GetBoundValue(BindingContext bindingContext)
        {
            var client = new PITreaderClient(
                bindingContext.ParseResult.GetValueForOption(this.host),
                bindingContext.ParseResult.GetValueForOption(this.port),
                bindingContext.ParseResult.GetValueForOption(this.acceptAll) ? CertificateValidators.AcceptAll : CertificateValidators.AcceptThumbprintSha2(bindingContext.ParseResult.GetValueForOption(this.thumbprint)))
            {
                ApiToken = bindingContext.ParseResult.GetValueForArgument(this.apiToken)
            };

            return client;
        }
    }
}