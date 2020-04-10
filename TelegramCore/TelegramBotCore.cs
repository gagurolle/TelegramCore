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
    public class TelegramCore
    {
        const string Version = "1.1";
        protected static string fileName_chats = "chat_id_list.txt";
        protected static string fileName_config = "config.ini";
        protected static int admin_chat_id = "";
        static HttpToSocks5Proxy proxy = new HttpToSocks5Proxy("ip", 1080);
        protected static ITelegramBotClient botClient = new TelegramBotClient("telegram_key", proxy) { Timeout = TimeSpan.FromSeconds(100) };
        public static List<int> chat_id_telegram = new List<int>();

        protected static int bottom_line = 0;
        protected static int upper_line = 0;
        protected static int gap = 20;
        protected static double _average = 0.0;

        ~TelegramCore()
        {
            botClient.StopReceiving();
            SendAdmin(9999999, "Бот завершил свою работу");
        }

        /// <summary>
        /// Установка верхней и нижней границы курса BTC. Нужно, чтобы определить, в какой момент отправлять сообщение в Telegram
        /// </summary>
        /// <param name="average"></param>
        public static void SetLine(double average)
        {
            bottom_line = (Int32)average - gap;
            upper_line = (Int32)average + gap;
            Console.WriteLine(bottom_line + "|" + upper_line);
        }
        /// <summary>
        /// Отправляет сообщение подписанным пользователям в телеграмм
        /// </summary>
        /// <param name="average"></param>
        /// <param name="direction"></param>
        public static void SendMessageTelegram(double average, string direction)
        {
            string _text = (direction == "up" ? "👆" : "👇") + "BTC_USD -> " + average.ToString() + "🔥";
            foreach (int chat_id in chat_id_telegram)
            {
                botClient.SendTextMessageAsync(chatId: chat_id, text: _text);
                Console.WriteLine(_text);
            }
        }
        /// <summary>
        /// Отправляет сообщение администратору о действиях других пользователей
        /// </summary>
        /// <param name="chat_id"></param>
        /// <param name="command"></param>
        public static void SendAdmin(int chat_id, string command)
        {
            botClient.SendTextMessageAsync(chatId: admin_chat_id, text: "Chat_id - " + chat_id + "||| Команда - " + command);
        }
        /// <summary>
        /// Подписка на ивенты телеграмма
        /// </summary>
        public static void Telegram()
        {
            var me = botClient.GetMeAsync();
            { }
            botClient.OnMessage += Bot_onMessage;
            botClient.OnCallbackQuery += BotClient_OnCallbackQuery;
            botClient.StartReceiving();
        }
        /// <summary>
        /// Отлавливаем нажатие на телеграм-клавиатуру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async static void BotClient_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            //Клавиатуры пока нет
            string buttonText = e.CallbackQuery.Data;
            string name = e.CallbackQuery.From.FirstName + " " + e.CallbackQuery.From.LastName;
            Console.WriteLine("Нажал на кнопку - " + name);
            await botClient.AnswerCallbackQueryAsync(e.CallbackQuery.Id, name);
        }
        /// <summary>
        /// Отлавливаем комманды от пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async static void Bot_onMessage(object sender, MessageEventArgs e)
        {

            var text = e?.Message?.Text;
            string answer = "";
            if (text == null)
            {
                return;
            }

            switch (text)
            {
                case "/start":
                    foreach (int i in chat_id_telegram)
                    {
                        Console.WriteLine(i);
                    }
                    if (!chat_id_telegram.Any(item => item == (Int32)e.Message.Chat.Id))
                    {
                        chat_id_telegram.Add((Int32)e.Message.Chat.Id);
                        ConfigFile.WriteFile(e.Message.Chat.Id, fileName_chats);
                        SendAdmin((Int32)e.Message.Chat.Id, "/start");
                        botClient.SendTextMessageAsync(chatId: e.Message.Chat.Id, text: "Вы подписались на отслеживание обновления курса BTC относительно USD. Промежуток составляет " + gap + " пунктов в обе стороны относительно цены. При прохождении этой границы будет отправлено уведомление и произойдет пересчет границ относительно отправленной цены. Команды бота: /start -> подписка на уведомление; /stop -> остановить подписку. Текущий курс BTC_USD = " + _average);
                        Console.WriteLine("Chat_id - " + (Int32)e.Message.Chat.Id + "||| Команда - " + "/connect");
                    }
                    break;
                case "/stop":

                    if (chat_id_telegram.Any(item => item == (Int32)e.Message.Chat.Id))
                    {
                        chat_id_telegram.Remove((Int32)e.Message.Chat.Id);
                        ConfigFile.WriteFile(fileName_chats);
                        botClient.SendTextMessageAsync(chatId: e.Message.Chat.Id, text: "Вы отписались от уведомлений. Для того чтобы возобновить уведомления, введите команду /start");
                        SendAdmin((Int32)e.Message.Chat.Id, "/stop");
                    }
                    break;
            }
        }
        public static async Task ReceiveMessage()//начинаем слушать сообщения от подписчиков
        {
            await Task.Run(() => {
                Console.WriteLine("Начали выполнять обработку сообщений");
                Telegram();
            });
        }

    
        }
    }

