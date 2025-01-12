using emu.lib;

namespace emu
{
    class Program
    {
        static void Main(string[] args)
        {
            var specFilePath = args[0];
            Processing.Process(specFilePath);
        }
    }
}