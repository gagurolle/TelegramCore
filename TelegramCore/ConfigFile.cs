using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TelegramCore
{
    class ConfigFile: TelegramLogic//унаследовали чтобы работать со списком chat_id_telegram
    {  
        async static public void WriteFile(object value, string fileName)//Добавление значения без перезаписи файла
        {
            using (StreamWriter sw = new StreamWriter(fileName, true, System.Text.Encoding.Default))
            {
                sw.WriteLine(value.ToString());           
            }
        }
        static async public void WriteFile(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName, false, System.Text.Encoding.Default))//перезаписываем файл и добавляем в него значения заного с учетом новых
            {
                foreach (int y in chat_id_telegram) {
                    sw.WriteLineAsync(y.ToString());
                }
                sw.Close();
            }
            
        }

        static public void FileRead(string fileName)
        {
            
            if(fileName == fileName_config)
            {
                if (!File.Exists(fileName))
                {
                    using (StreamWriter sw = new StreamWriter(fileName, false, System.Text.Encoding.Default))
                    {
                        Console.WriteLine("Нет данных, закройте приложение и заполните файл " + fileName);
                        return;
                    }
                }
                else
                {
                    using (StreamReader sr = new StreamReader(fileName, System.Text.Encoding.Default))
                    {
                        string line;
                        line = sr.ReadLine();
                        {
                            admin_chat_id = Int32.Parse(line);
                        }
                        sr.Close();
                    }
                }
            }






            if (!File.Exists(fileName))
            {
                using (StreamWriter sw = new StreamWriter(fileName, false, System.Text.Encoding.Default))
                {
                    //Если файла не существет, то создаст этот файл 
                    return;
                }
            }
            using (StreamReader sr = new StreamReader(fileName, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    chat_id_telegram.Add(Int32.Parse(line));
                }
                sr.Close();
            }
        }


    }
}
