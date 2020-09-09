using NeoCortexApi.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ImageStabilizationHtmTests
{
    public class Helpers
    {
        /// <summary>
        /// Shows default initialization of parameters of spatial pooler.
        /// </summary>
        /// <returns></returns>
        internal static Parameters GetDefaultParams()
        {
            Random rnd = new Random(42);

            var parameters = Parameters.getAllDefaultParameters();
            parameters.Set(KEY.POTENTIAL_RADIUS, 5);
            parameters.Set(KEY.POTENTIAL_PCT, 0.5);
            parameters.Set(KEY.GLOBAL_INHIBITION,false);
            parameters.Set(KEY.LOCAL_AREA_DENSITY, -1.0);
            //parameters.Set(KEY.NUM_ACTIVE_COLUMNS_PER_INH_AREA, 3.0);
            parameters.Set(KEY.STIMULUS_THRESHOLD, 0.0);
            parameters.Set(KEY.SYN_PERM_INACTIVE_DEC, 0.01);
            parameters.Set(KEY.SYN_PERM_ACTIVE_INC, 0.1);
            parameters.Set(KEY.SYN_PERM_CONNECTED, 0.1);
            parameters.Set(KEY.MIN_PCT_OVERLAP_DUTY_CYCLES, 0.1);
            parameters.Set(KEY.MIN_PCT_ACTIVE_DUTY_CYCLES, 0.1);
            parameters.Set(KEY.DUTY_CYCLE_PERIOD, 10);
            parameters.Set(KEY.MAX_BOOST, 10.0);
            parameters.Set(KEY.RANDOM, rnd);
            //int r = parameters.Get<int>(KEY.NUM_ACTIVE_COLUMNS_PER_INH_AREA);

            return parameters;
        }


        /// <summary>
        /// Creates random vector of specified dimension.
        /// </summary>
        /// <param name="numOfBits"></param>
        /// <param name="rnd"></param>
        /// <returns></returns>
        public static int[] GetRandomVector(int numOfBits, Random rnd = null)
        {
            if (rnd == null)
                rnd = new Random();

            int[] vector = new int[numOfBits];

            for (int i = 0; i < numOfBits; i++)
            {
                vector[i] = rnd.Next(0, 2);
            }

            return vector;
        }

        /// <summary>
        /// Creates string representation from one dimensional vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static string StringifyVector(int[] vector)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var vectorBit in vector)
            {
                sb.Append(vectorBit);
                sb.Append(", ");
            }

            return sb.ToString();
        }
    }
}
