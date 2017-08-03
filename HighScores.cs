using System;
using System.IO;

namespace ScoreRecorder
{
    public static class HighScores
    {
        const string path = @"resources\HighScores.SpInv";
        static public bool Recorde(string Name, string Score, string Level)
        {
            try
            {
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                    WriteToScore(Name, Score, Level);
                }
                else
                {
                    if (Int32.Parse(Score) > ReadFromScore())
                        WriteToScore(Name, Score, Level);
                }
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }
        static public int HighScore()
        {
            if (File.Exists(path))
                return ReadFromScore();
            return 0;
        }
        private static void WriteToScore(string Name, string Score, string Level)
        {
            Stream output = File.OpenWrite(path);
            StreamWriter Wstr = new StreamWriter(output);

            Wstr.Write(Name + ":" + Score + "*" + Level);

            Wstr.Close();
            output.Close();
        }
        private static int ReadFromScore()
        {
            string strScore;
            int intScore,index;
            Stream input = File.OpenRead(path);
            StreamReader Rstr = new StreamReader(input);

            strScore = Rstr.ReadToEnd();

            Rstr.Close();
            input.Close();

            index = strScore.IndexOf(':');
            intScore = strScore.IndexOf('*');

            if (index > 0 && intScore > 0)
            {
                strScore = strScore.Substring(index + 1, intScore - index - 1);
                intScore = Int32.Parse(strScore);
            }
            else intScore = 0;
            return intScore;
        }
    }
}
