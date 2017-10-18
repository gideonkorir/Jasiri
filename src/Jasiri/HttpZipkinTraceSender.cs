using OpenTracing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jasiri
{
    public class HttpZipkinTraceSender : ISender
    {
        readonly ZipkinApi zipkinApi;
        readonly HttpClient client;

        public HttpZipkinTraceSender(ZipkinApi zipkinApi, HttpClient client)
        {
            this.zipkinApi = zipkinApi ?? throw new ArgumentNullException(nameof(zipkinApi));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task SendAsync(IReadOnlyList<ISpan> spans, CancellationToken cancellationToken)
        {
            if (spans == null || spans.Count == 0)
                return;

            var serialized = zipkinApi.Serializer.Serialize(spans);
            var response = await client.PostAsync(
                zipkinApi.Uri, 
                new StringContent(serialized, Encoding.UTF8, zipkinApi.Serializer.MediaType)
                ).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}
