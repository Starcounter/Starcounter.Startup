﻿using System;

namespace Starcounter.Startup.Routing
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class UrlAttribute : Attribute
    {
        public string Value { get; private set; }

        public UrlAttribute(string value)
        {
            Value = value;
        }
    }
}