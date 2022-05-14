using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace LearnOpenTK
{
    class BlenderImportModel
    {
        List<string> lines;

        public BlenderImportModel(string path)
        {
            string line;
            try
            {
                lines = new List<string>();
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(path);
                //Read the first line of text
                line = sr.ReadLine();
                //Continue to read until you reach end of file
                while (line != null)
                {
                    lines.Add(line);
                    line = sr.ReadLine();
                }
                //close the file
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }

        void GetTextureCoord(int numVert, in List<float> vert)
        {
            float textCoord;
            for (int i = 0; i < lines.Count; i++)
            {
                if(lines[i].StartsWith("vt") && numVert == 0)
                {
                    string[] strResult = lines[i].ToString().Split(' ');
                    for (int j = 0; j < strResult.Length; j++)
                    {
                        if (strResult[j] == "vt" || strResult[j] == "")
                            continue;
                        textCoord = float.Parse(strResult[j], CultureInfo.InvariantCulture);
                        vert.Add(textCoord);
                    }
                    return;
                }
                if (lines[i].StartsWith("vt") && numVert != 0)
                    numVert--;
            }
        }

        public List<float> GetVertex()
        {
            List<float> result = new List<float>();
            float num;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i][0] == 'v' && lines[i][1] != 't' && lines[i][1] != 'n')
                {
                    string[] strResult = lines[i].ToString().Split(' ');
                    for (int j = 0; j < strResult.Length; j++)
                    {
                        if (strResult[j] == "v" || strResult[j] == "")
                            continue;
                        num = float.Parse(strResult[j], CultureInfo.InvariantCulture);
                        result.Add(num);
                    }
                    GetTextureCoord(i - 3, result);
                }
              
            }
            return result;
        }

        public List<uint> GetIndexies()
        {
            List<uint> result = new List<uint>();
            uint vertIndex;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i][0] == 'f')
                {
                    string[] str = lines[i].ToString().Split(' ');
                    for (int j = 0; j < str.Length; j++)
                    {
                        if (str[j] == "f" || str[j] == "")
                            continue;
                        string str1 = "";
                        int k = 0;
                        while(str[j][k] != '/')
                        {
                            str1 += str[j][k].ToString();
                            k++;
                        }
                        vertIndex = (uint)Convert.ToInt32(str1.ToString()) - 1;
                        result.Add(vertIndex);  
                    }
                }
            }
            return result;
        }

    }
}
