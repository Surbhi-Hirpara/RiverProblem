using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GoWithTheFlow
{
    class VM : INotifyPropertyChanged
    {
        #region Singleton
        private static VM vm;
        public static VM Instance { get { vm ??= new VM(); return vm; } }

        private VM()
        {

        }
        #endregion

        public static int nWords;
        public static int[] EachWordLen;

        private string paragraph = "";
        public string Paragraph { get { return paragraph; } set { paragraph = value; propChange(); } }

        private string input = "";
        public string Input { get { return input; } set { input = value; propChange(); } }

        int numWords;
        public int NumWords { get { return numWords; } set { numWords = value; propChange(); } }

        int lineWidth;
        public int LineWidth { get { return lineWidth; } set { lineWidth = value; propChange(); } }

        int riverLength;
        public int RiverLength { get { return riverLength; } set { riverLength = value; propChange(); } }
        public void ReadFile()
        {
            int NumLocation = Input.IndexOf(" ");
            NumWords = Convert.ToInt32(Input.Substring(0, NumLocation));
            paragraph = Input.Substring(NumLocation + 1);
            Paragraph = paragraph;
        }
        public void FindRiver()
        {
            nWords = NumWords;
            EachWordLen = new int[nWords];
            string[] Words = paragraph.Split(' ');
            int SumOfWordsLen = 0, HighestWordLen = 0;

            for (int i = 0; i < nWords; i++)
            {
                EachWordLen[i] = Words[i].Length;
                SumOfWordsLen += EachWordLen[i];
                HighestWordLen = Math.Max(HighestWordLen, EachWordLen[i]);
            }

            int Result = -1, ResultWidth = 0;

            for (int MaxWordLen = HighestWordLen; MaxWordLen <= SumOfWordsLen + nWords - 1; MaxWordLen++)
            {
                int Tmp = Check(MaxWordLen);
                if (Tmp > Result)
                {
                    Result = Tmp;
                    ResultWidth = MaxWordLen;
                }
            }
            LineWidth = ResultWidth;
            RiverLength = Result;
        }
        public int FindLocation(int wordWidth, int Row, int Col)
        {
            return (wordWidth * Row) + Col;
        }
        public int Check(int wordWidth)
        {
            int[] Row = new int[nWords];
            int[] Col = new int[nWords];
            int curRow = 0, curCol = -1;
            int i;
            for (i = 0; i < nWords; i++)
            {
                curCol += EachWordLen[i] + 1;
                if (curCol > wordWidth)//word wrap
                {
                    Row[i] = Row[i - 1] + 1;
                    Col[i] = EachWordLen[i];
                    curRow = Row[i];
                    curCol = Col[i];
                }
                // Same line.
                else
                {
                    Row[i] = curRow;
                    Col[i] = curCol;
                }
            }

            int[] Rivers = new int[nWords];
            i = 0;
            int Result = 0;
            // Init river table for first row.
            while (i < nWords && Row[i] == 0)
            {
                // another word on this line.
                if (i + 1 < nWords && Row[i + 1] == 0)
                {
                    Rivers[i] = 1;
                    Result = 1;
                }
                i++;
            }

            int pre = 0;
            for (; i < nWords - 1; i++)
            {
                // Last word on line with trailing spaces, can't be part of any river.
                if (Row[i + 1] > Row[i]) continue;

                Rivers[i] = 1;
                // Current index location
                int curI = FindLocation(wordWidth, Row[i], Col[i]);

                while (Col[pre] == wordWidth || FindLocation(wordWidth, Row[pre], Col[pre]) < curI - wordWidth - 1)
                {
                    pre++;
                }
                //previous index location
                int prevI = FindLocation(wordWidth, Row[pre], Col[pre]);

                // Up and to the left; update and advance to next spot.
                if (prevI == curI - wordWidth - 1)
                {
                    Rivers[i] = Math.Max(Rivers[i], Rivers[pre] + 1);
                    pre++;
                    prevI = FindLocation(wordWidth, Row[pre], Col[pre]);
                }
                // Directly below.
                else if (prevI == curI - wordWidth)
                {
                    Rivers[i] = Math.Max(Rivers[i], Rivers[pre] + 1);
                }
                // Check right separately
                if (prevI == curI - wordWidth + 1)
                {
                    Rivers[i] = Math.Max(Rivers[i], Rivers[pre] + 1);
                }
                Result = Math.Max(Result, Rivers[i]);
            }
            return Result;
        }
        #region
        public event PropertyChangedEventHandler PropertyChanged;

        private void propChange([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
