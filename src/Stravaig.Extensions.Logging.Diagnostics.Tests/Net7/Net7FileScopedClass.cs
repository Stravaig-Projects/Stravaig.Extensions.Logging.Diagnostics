#if NET7_0_OR_GREATER

using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;
using Stravaig.Extensions.Logging.Diagnostics.Tests.Net7.Helpers;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.Net7;

[TestFixture]
public class Net7FileScopedClass
{
    [Test]
    public void CanLogsFromAFileScopedClassBeRetrievedFromTheProvider()
    {
        var provider = new TestCaptureLoggerProvider();
        var logFactory = LoggerFactory.Create(builder =>
        {
            builder.AddProvider(provider);
            builder.AddConsole();
        });

        var _ = new SomeOpenService(logFactory);
        
        // Since a file scoped type can only be referenced within the same file,
        // this means that:
        // var logs = provider.GetLogEntriesFor<SomeFileScopedService>();
        // won't compile, emitting the error CS0246 "The type or namespace name
        // 'SomeFileScopedService' could not be found(are you missing a using
        // directive or an assembly reference?)" instead

        // From https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-11.0/file-local-types#naming
        // The type's accessibility and name in metadata is implementation-defined.
        // The intention is to permit the compiler to adopt any future access-limitation
        // features in the runtime which are suited to the feature. It's expected
        // that in the initial implementation, an internal accessibility would be
        // used and an unspeakable generated name will be used which depends on
        // the file the type is declared in.
        // So the following lookup works, but may change if they change the implementation,
        // which means no easy way to provide a helper method to get specific logs.
        // Is it likely that logs categories will reference file scoped types?
        // Probably not. If someone puts an issue in for this, will revisit.
        var fileScopedType = typeof(SomeOpenService).Assembly.GetTypes()
            .First(t => t.FullName!.StartsWith("Stravaig.Extensions.Logging.Diagnostics.Tests.Net7.Helpers.<FileWithTwoServices>",
                            StringComparison.Ordinal) &&
                        t.FullName.EndsWith("__SomeFileScopedService"));
        var logs = provider.GetLogEntriesFor(fileScopedType);
        logs.Count.ShouldBe(1);
        logs[0].OriginalMessage.ShouldBe("This is from a file scoped class!");
    }
}

#endif