using System;
using Game;

namespace Game.Presenters
{
    public class TextPresenter : IResultPresenter<string>
    {
        public void Present(string result, ReconResult reconResult, Environment environment)
        {
            if (!string.IsNullOrEmpty(result))
            {
                Console.WriteLine($"Text: {result}");
            }
        }
    }
} 