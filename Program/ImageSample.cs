using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaBoost;

namespace ConsoleTrainer
{
    struct ImageSample : ISample
    {
        public string fileName;
        public int location;

        ImageSample(string fileName, int location)
        {
            this.fileName = fileName;
            this.location = location;
        }
    }
}
