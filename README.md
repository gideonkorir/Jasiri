# Jasiri
OpenTracing + OpenZipkin

This is an [OpenTracing](http://opentracing.io/) implementation for [Zipkin](http://zipkin.io/). The name **Jasiri** is Swahili for **Brave** which is the name of the official zipkin client in Java.

## How to use

```
    var sender = new HttpZipkinTraceSender(
        ZipkinHttpApi.V2("http://localhost:9411"), new HttpClient()
        );
    var reporter = new PeriodicReporter(sender, new FlushOptions()
    {
        MaxBufferSize = 100,
        FlushInterval = TimeSpan.FromSeconds(1),
        CancellationToken = CancellationToken.None
    });
    Trace.Tracer = new Tracer(new TraceOptions()
    {
        Reporter = reporter 
    }); //use everything else default

    using(var span = Trace.Tracer.BuildSpan("try out zipkin")
        .WithTag(Tags.SpanKind, Tags.SpanKindClient)
        .Start()
        )
    {
        DoSomethingAwesome();
    }

```