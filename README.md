# Jasiri
OpenTracing + OpenZipkin

This is an [OpenTracing](http://opentracing.io/) compatibile implementation of [Zipkin](http://zipkin.io/) based on the Zipkin V2 api. The name **Jasiri** is Swahili for **Brave** which is the name of the official zipkin client in Java.

## How to use without OpenTracing

Add a reference to Jasiri nuget package

```
    var sender = new HttpZipkinTraceSender(
        ZipkinHttpApi.V2("http://localhost:9411"), new HttpClient()
        );
    var registry = new InMemoryPropagationRegistry();
    registry.Register("b3", new B3Propagator()); //you can register multiple propagators for different transports
    var tracer = new Tracer(new TraceOptions()
    {
        Reporter = new PeriodicReporter(sender, new FlushOptions()
        {
            FlushInterval = TimeSpan.FromSeconds(1),
            MaxBufferSize = 100
        }),
        PropagationRegistry = registry,
        Endpoint = Endpoint.GetHostEndpoint()
    });
    Trace.Tracer = tracer;

    //in another piece of code
    using(var span = Trace.NewSpan("try out jasiri").Tag("tag1", "value1").Start())
    {
        DoSomethingAwesome();
    }
```

## How to use OpenTracing

Add a reference to Jasiri.OpenTracing package

```
    //create tracer as specified above
    var tracer = TracerAsSpecifiedAbove();
    OpenTracing.Trace.Tracer = new OTTracer(tracer);
    using(var span = OpenTracing.Trace.BuildSpan("open_jasiri")
        .WithTag("Jasiri", "IsCool")
        .WithTag(Tags.SpanKind, Tags.SpanKindServer)
        .Start())
    {
    }
```