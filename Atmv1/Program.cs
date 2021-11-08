using System;

namespace Atmv1
{
    static class Entry
    {
        static void Main(string[] args)
        {
            var app = new App();
            app.Initialization();
            app.Execute();
        }
    }
}
