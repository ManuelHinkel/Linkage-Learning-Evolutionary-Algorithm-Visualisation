using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FitnessFunction
{
    public class NKLandscapeRandom: NKLandscape
    {
        private int _seed;
        private Random _r;
        public NKLandscapeRandom()
        {
            _r = new Random();
            _seed = _r.Next();
            GenerateValues();
        }

        private void GenerateValues()
        {
            _r = new Random(_seed);
            k = _r.Next(1, 10);
            int count = 1 << k;
            VALUES = new double[count];

            for (int i = 0; i < count; i++)
            {
                VALUES[i] = _r.NextDouble();
            }
        }

        public override byte[] ConvertArgumentToBytes()
        {
            byte[] bytes = new byte[4];
            ByteUtil.WriteIntToBuffer(_seed, bytes, 0);
            return bytes;
        }

        public override bool CreateArgumentFromBytes(byte[] bytes)
        {
            _seed = BitConverter.ToInt32(bytes, 0);
            GenerateValues();
            return true;
        }
    }
}
