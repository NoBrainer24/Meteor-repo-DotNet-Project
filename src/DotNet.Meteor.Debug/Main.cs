using System;

namespace DotNet.Meteor.Debug;

public class Program {
    public static void Main(string[] args) {
        var debugSession = new DebugSession(Console.OpenStandardInput(), Console.OpenStandardOutput());
        debugSession.Start();
    }
}