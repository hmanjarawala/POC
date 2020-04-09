using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchCalculator
{
    class CandidateCode
    {
        static void Main(string[] args)
        {
            int intNoOfTestcases;
            long[] lngNoOfTeamsParticipating;

            intNoOfTestcases = Convert.ToInt32(Console.ReadLine());
            if (intNoOfTestcases < 1 || intNoOfTestcases > 100)
                Console.WriteLine("No. of test cases should be between 1 to 100");
            else
            {
                lngNoOfTeamsParticipating = new long[intNoOfTestcases];

                for(int counter = 0; counter < intNoOfTestcases; counter++)
                {
                    lngNoOfTeamsParticipating[counter] = Convert.ToInt64(Console.ReadLine());
                }
                MatchCalculator mc = new MatchCalculator();
                for (int counter = 0; counter < intNoOfTestcases; counter++)
                {
                    Console.WriteLine(mc.CalculateMatches(lngNoOfTeamsParticipating[counter]));
                }
            }
            Console.ReadKey();
        }

        private class MatchCalculator
        {
            public long CalculateMatches(long noOfTeams)
            {
                if (noOfTeams == 2) return 1;
                long noOfMatches = noOfTeams / 2;
                if (noOfTeams % 2 > 0) noOfTeams = noOfMatches + 1;
                else noOfTeams = noOfMatches;
                return CalculateMatches(noOfTeams) + noOfMatches;
            }
        }
    }
}
