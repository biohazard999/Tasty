﻿using System;
using System.Collections.Generic;
using System.Linq;

using Xenial.Delicious.Metadata;
using Xenial.Delicious.Scopes;

namespace Xenial.Delicious.Execution
{
    public class TestExecutionContext
    {
        public TestExecutionContext(TestCase currentCase, TastyScope currentScope, TestGroup? currentGroup)
        {
            CurrentCase = currentCase ?? throw new ArgumentNullException(nameof(currentCase));
            CurrentScope = currentScope ?? throw new ArgumentNullException(nameof(currentScope));
            CurrentGroup = currentGroup;
        }

        public TestCase CurrentCase { get; }
        public TastyScope CurrentScope { get; }
        public TestGroup? CurrentGroup { get; }
    }
}
