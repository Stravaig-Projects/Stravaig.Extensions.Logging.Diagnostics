using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;
using Stravaig.Extensions.Logging.Diagnostics.Render;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.Renderer
{
    [TestFixture]
    public class LogEntryRendererExtensionsTests
    {
        [Test]
        public void EmptyArray_RendersNothing()
        {
            int writeToSinkCount = 0;
            int formatCount = 0;
            
            Array.Empty<LogEntry>().RenderLogs(_ => formatCount++.ToString(), _ => writeToSinkCount++);
            
            writeToSinkCount.ShouldBe(0);
            formatCount.ShouldBe(0);
        }

        [Test]
        public void SingleLog_RendersOneItem()
        {
            int writeToSinkCount = 0;
            int formatCount = 0;

            var logger = new TestCaptureLogger();
            logger.LogInformation("Some log.");

            logger.GetLogs()
                .RenderLogs(le => $"{++formatCount} {le.FormattedMessage}", s =>
                {
                    writeToSinkCount++;
                    s.ShouldBe("1 Some log.");
                });
            
            writeToSinkCount.ShouldBe(1);
            formatCount.ShouldBe(1);
        }
        
        [Test]
        public void ManyLogs_RendersItems()
        {
            int formatCount = 0;

            var logger = new TestCaptureLogger();
            logger.LogInformation("Some log.");
            logger.LogInformation("Some other log.");
            logger.LogInformation("A third log.");

            List<string> sink = new List<string>();

            logger.GetLogs()
                .RenderLogs(
                    le => $"{++formatCount} {le.FormattedMessage}",
                    s => sink.Add(s));
            
            sink.Count.ShouldBe(3);
            formatCount.ShouldBe(3);
            
            sink[0].ShouldBe("1 Some log.");
            sink[1].ShouldBe("2 Some other log.");
            sink[2].ShouldBe("3 A third log.");
        }
    }
}