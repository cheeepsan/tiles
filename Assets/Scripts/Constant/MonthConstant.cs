using System;
using System.Collections.Generic;

namespace Constant
{
    /*
     * TODO: check validity from actual sources, not wikipedia
     */
    
    /**
     *  TODO: Think about language support
     * 
     */
    public static class MonthConstant
    {
        /*
         * 0 - 3 flood
         * 4 - 7 field works
         * 8 - 11 gather
         */
        public static Dictionary<int, String> months = new Dictionary<int, string>()
        {
            { 0, "I Akhet Thoth" },
            { 1, "II Akhet Phaophi" },
            { 2, "III Akhet Athyr" },
            { 3, "IV Akhet Choiak" },
            { 4, "I Peret Tybi" },
            { 5, "II Peret Mechir" },
            { 6, "III Peret Phamenoth" },
            { 7, "IV Peret Pharmuthi" },
            { 8, "I Shemu Pachons" },
            { 9, "II Shemu Payni" },
            { 10, "III Shemu Epiphi" },
            { 11, "IV Shemu Mesore" }
        };

        public static String GetMonthName(int monthIndex)
        {
            return months[monthIndex];
        }
    }
    
    
}