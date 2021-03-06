﻿using System;

namespace Xenial.Delicious.Execution.TestGroupMiddleware
{
    public static class ReportTestGroupMiddleware
    {
        public static TestExecutor UseTestGroupReporters(this TestExecutor executor)
            => executor.UseGroup(async (context, next) =>
            {
                try
                {
                    await next().ConfigureAwait(false);
                }
                finally
                {
                    //TODO: TestGroupReporters
                    //context.CurrentScope.Report(context.CurrentGroup);
                }
            });
    }
}
