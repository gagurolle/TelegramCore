using System;
using Telegram.Bot;
using MihaZupan;
using Telegram.Bot.Args;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace TelegramCore
{
    class TelegramLogic : TelegramCore
    {
        public static void startBot()
        {
            try
            {
                ConfigFile.FileRead(fileName_chats);
                ConfigFile.FileRead(fileName_config);

                string rsi = "";
                string average = "0.0";
                string url = "";
                double AVERAGE;
                double RSI;

                foreach (int y in chat_id_telegram)
                {
                    Console.WriteLine(y);
                }

                Parse_trading_view parse = new Parse_trading_view();

                rsi = parse.GetValueRSI();
                average = parse.GetValueAverage();
                AVERAGE = double.Parse(average, CultureInfo.InvariantCulture);
                RSI = double.Parse(rsi, CultureInfo.InvariantCulture);
                _average = AVERAGE;
                SetLine(AVERAGE);

                ReceiveMessage();

                Console.WriteLine("Версия 1 ");

                botClient.SendTextMessageAsync(chatId: admin_chat_id, text: "🔥🔥🔥Начало отправки. Бот запущен");

                while (true)
                {
                    try
                    {
                        rsi = parse.GetValueRSI();
                        average = parse.GetValueAverage();
                    }
                    catch (Exception e)//Отлавливаем, если сайт затупил и в единицу времени кинул нам невалидные данные, просто ждем чутка и еще раз парсим
                    {
                        System.Threading.Thread.Sleep(1000);
                        continue;
                    }

                    AVERAGE = double.Parse(average, CultureInfo.InvariantCulture);
                    _average = AVERAGE;
                    RSI = double.Parse(rsi, CultureInfo.InvariantCulture);

                    if (AVERAGE < bottom_line)
                    {
                        SetLine(AVERAGE);
                        SendMessageTelegram(AVERAGE, "down");
                        System.Threading.Thread.Sleep(10000);
                    }
                    if (AVERAGE > upper_line)
                    {
                        SetLine(AVERAGE);
                        SendMessageTelegram(AVERAGE, "up");
                        System.Threading.Thread.Sleep(10000);
                    }

                    System.Threading.Thread.Sleep(50);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Произошла ошибка - " + e.Message);
                SendAdmin(100000000, "Произошла ошибка - " + e.Message);
                System.Threading.Thread.Sleep(100000);
                startBot();
            }
        }
    }
}
