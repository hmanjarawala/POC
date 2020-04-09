using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAUtility
{
    partial class UtilityFasad
    {
        public double GetAbsoluteNumber(double number)
        { 
            return Math.Abs(number); 
        } 
 

        public int GetDivisionQuadrent(double number1, double number2)
        { 
            return (int) (number1 / number2); 
        } 
 

        public double GetModuloNumber(double number1, double number2)
        { 
            return number1 % number2; 
        } 
 

        public double GetCeilingNumber(double number)
        { 
            return Math.Ceiling(number); 
        } 
 

        public double GetFloorNumber(double number)
        { 
            return Math.Floor(number); 
        } 
 

        public double GetMaxNumber(double number1, double number2)
        { 
            return Math.Max(number1, number2); 
        } 
 

        public double GetMinNumber(double number1, double number2)
        { 
            return Math.Min(number1, number2); 
        }

        public double FormatNumber(double number, int decimalPlace)
        {
            return Math.Round(number, decimalPlace);
        }

    }
}
