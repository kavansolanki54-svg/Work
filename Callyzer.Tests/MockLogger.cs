using System;
using Callyzer.App.Interfaces;

namespace Callyzer.Tests
{
    public class MockLogger : ILoggerService
    {
        public void Debug(string tag, string message) { }
        public void Error(string tag, string message, Exception? ex = null) { }
        public void Info(string tag, string message) { }
        public void Warning(string tag, string message) { }
    }
}
