using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Radilovsoft.Rest.Infrastructure.Test.Helpers
{
    public abstract class BaseHelperTest
    {
        protected static DateTime RandomDay(Random gen)
        {
            var start = new DateTime(1995, 1, 1);
            var range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }

        protected static void Equally(IReadOnlyList<TestDto> queryable1, IReadOnlyList<TestDto> queryable2)
        {
            for (int i = 0; i < queryable1.Count; i++)
            {
                if (!Equals(queryable2[i], queryable1[i]))
                {
                    Assert.Fail();
                }
            }
        }
    }
}