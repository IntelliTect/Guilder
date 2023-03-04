﻿using Guilder.Client.Services;
using Guilder.Shared;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guilder.Client.Tests.Services;

public class TimelineTests
{
    [Fact]
    public void ReInitialize_SetsLowerBoundOneSlotBefore()
    {
        var mocker = new AutoMocker();
        mocker.GetMock<IClock>().Setup(x => x.GetCurrentInstant()).Returns(new LocalDateTime(2022, 3, 16, 10, 8).InUtc().ToInstant());
        mocker.GetMock<ICurrentTimeZone>().Setup(x => x.TimeZone).Returns(DateTimeZoneProviders.Tzdb["Etc/UTC"]);

        var timeline = mocker.CreateInstance<TimelineStore>();

        timeline.ReInitialize();

        Assert.Equal(new LocalDateTime(2022, 3, 16, 9, 30), timeline.LowerBound);
    }

    [Fact]
    public void ReInitialize_SetsUpperBound4HoursAhead()
    {
        var mocker = new AutoMocker();
        mocker.GetMock<IClock>().Setup(x => x.GetCurrentInstant()).Returns(new LocalDateTime(2022, 3, 16, 10, 8).InUtc().ToInstant());
        mocker.GetMock<ICurrentTimeZone>().Setup(x => x.TimeZone).Returns(DateTimeZoneProviders.Tzdb["Etc/UTC"]);

        var timeline = mocker.CreateInstance<TimelineStore>();

        timeline.ReInitialize();

        Assert.Equal(new LocalDateTime(2022, 3, 16, 14, 0), timeline.UpperBound);
    }
}