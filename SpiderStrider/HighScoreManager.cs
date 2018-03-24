using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SpiderStrider
{
    public class HighScoreManager
    {
        public static List<Highscore> highscores;

        public static void LoadFile()
        {
            try
            {
                using (Stream file = File.Open("highscore.sav", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    object obj = bf.Deserialize(file);
                    highscores = obj as List<Highscore>;
                }
            }
            catch (Exception e)
            {
                highscores = new List<Highscore>() { new Highscore("Jonathan", 10043) };
                SaveFile();
            }
        }

        public static void SaveFile()
        {
            using (Stream file = File.Open("highscore.sav", FileMode.OpenOrCreate))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, highscores);
            }
        }
    }

    [Serializable]
    public struct Highscore
    {
        public string name;
        public int score;
        
        public Highscore(string name, int score)
        {
            this.name = name;
            this.score = score;
        }
    }
}