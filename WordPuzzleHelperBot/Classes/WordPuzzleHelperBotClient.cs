using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace WordPuzzleHelperBot.Classes
{
    class WordPuzzleHelperBotClient : TelegramBotClient
    {
        private WordsBuilder wordsBuilder;
        private bool isAddingNewWordMode = false;
        private bool isMarkingLessCommonWordMode = false;

        public WordPuzzleHelperBotClient(string token)
            : base(token)
        {
            wordsBuilder = new WordsBuilder();
        }

        /// <summary>
        /// Starts receiving updates.
        /// </summary>
        /// <param name="receiverOptions"></param>
        /// <param name="cancellationTokenSource"></param>
        public void StartReceiving(ReceiverOptions receiverOptions, CancellationTokenSource cancellationTokenSource)
        {
            this.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cancellationTokenSource.Token);
        }

        #region Handlers
        /// <summary>
        /// Handles chat bot udpades.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Type != UpdateType.Message)
                return;

            // Only process text messages
            if (update.Message!.Type != MessageType.Text)
                return;

            var chatId = update.Message.Chat.Id;
            var userFirstName = update.Message.Chat.FirstName;
            var messageText = update.Message.Text;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            switch (messageText)
            {
                case "/start":
                    await HandleStartCommandAsync(botClient, cancellationToken, chatId, userFirstName);
                    break;
                case "Добавить слово в словарь":
                    //isAddingNewWordMode = true;
                    //isMarkingLessCommonWordMode = false;
                    await HandleAddNewWordButtonClickAsync(botClient, cancellationToken, chatId);
                    break;
                case "Отметить слово, которое не знает игра":
                    //isMarkingLessCommonWordMode = true;
                    //isAddingNewWordMode = false;
                    await HandleMarkLessCommonWordButtonClickAsync(botClient, cancellationToken, chatId);
                    break;
                case "Вернуться в режим поиска слов":
                    //isMarkingLessCommonWordMode = false;
                    //isAddingNewWordMode = false;
                    await HandleBackToWordsSearchMode(botClient, cancellationToken, chatId);
                    break;
                default:
                    await HandleUserInputAsync(botClient, cancellationToken, chatId, messageText);
                    break;
            }
        }

        /// <summary>
        /// Hadles user's message sent to chat bot.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="chatId"></param>
        /// <param name="messageText"></param>
        /// <returns></returns>
        private async Task HandleUserInputAsync(ITelegramBotClient botClient, CancellationToken cancellationToken, long chatId, string messageText)
        {
            if (isAddingNewWordMode)
            {
                /*
                await wordsBuilder.AddToMainDictionary(messageText);
                isAddingNewWordMode = false;

                Message sendMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Слово успешно добавлено!",
                        cancellationToken: cancellationToken);
                */
            }
            else if (isMarkingLessCommonWordMode)
            {
                /*
                await wordsBuilder.AddToLessCommonWordsDictionary(messageText);
                isMarkingLessCommonWordMode = false;

                Message sendMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Слово успешно добавлено!",
                        cancellationToken: cancellationToken);
                */
            }
            else
            {
                Message sendMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: GenerateResponse(messageText),
                        cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Handles the "/start" command. 
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="chatId"></param>
        /// <param name="userFirstName"></param>
        /// <returns></returns>
        private async Task HandleStartCommandAsync(ITelegramBotClient botClient, CancellationToken cancellationToken, long chatId, string userFirstName)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Добавить слово в словарь"},
                new KeyboardButton[] { "Отметить слово, которое не знает игра" },
            })
            {
                ResizeKeyboard = true
            };

            Message sendSticker = await botClient.SendStickerAsync(
                    chatId: chatId,
                    sticker: "CAACAgIAAxkBAAEEKwliMJIttXq76fgq4G2dpIos37lixgACBQADwDZPE_lqX5qCa011IwQ",
                    cancellationToken: cancellationToken);

            Message sendGreetingMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"Привет, {userFirstName}!\n\n" +
                      "Меня зовут WordPuzzleHelper. " +
                      "Я был создан, чтобы помочь тебе отыскать все возможные слова из набора букв, который ты мне дашь" +
                      $"(используй меня, чтобы проходить игры на поиск слов{char.ConvertFromUtf32(0x1F609)})\n\n" +
                      "Пока что я знаю только русский словарь, но когда вырасту, " +
                      $"то обязательно выучу ещё парочку языков{char.ConvertFromUtf32(0x1F607)}\n\n",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);

            Message sendRulesMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "У меня есть всего три ограничения. Во введённом тобой наборе:\n\n" +
                      "- не может быть менее трёх или более семи букв;\n" +
                      "- допустимы только буквенные значения;\n" +
                      "- допустимы только буквы из кириллического алфавита.\n\n" +
                      $"Чтобы начать, просто отправь мне сообщение с буквами{char.ConvertFromUtf32(0x1F60A)}",
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Handles "Добавить слово в словарь" click.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        private async Task HandleAddNewWordButtonClickAsync(ITelegramBotClient botClient, CancellationToken cancellationToken, long chatId)
        {
            /*
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Вернуться в режим поиска слов"},
            })
            {
                ResizeKeyboard = true
            };

            Message sendMessage = await botClient.SendTextMessageAsync(
               chatId: chatId,
               text: "Есть, чем пополнить мой словарный запас?\n\n" +
               "Напиши мне одно или несколько слов одним сообщением через пробел или запятую " +
               "и мои создатели обязательно рассмотрят твоё предложение!",
               replyMarkup: replyKeyboardMarkup,
               cancellationToken: cancellationToken);
            */

            Message sendMessage = await botClient.SendTextMessageAsync(
               chatId: chatId,
               text: "Опция на стадии разработки!",
               cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Handles "Отметить слово, которое не знает игра" click.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        private async Task HandleMarkLessCommonWordButtonClickAsync(ITelegramBotClient botClient, CancellationToken cancellationToken, long chatId)
        {
            /*
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Вернуться в режим поиска слов"},
            })
            {
                ResizeKeyboard = true
            };

            Message sendGreetingMessage = await botClient.SendTextMessageAsync(
               chatId: chatId,
               text: "Глупая игра не знает слово, которое я нашёл?\n\n" +
               "Давай сделаем соответсвующую отметку и в следующий раз я обязательно предупрежу тебя, " +
               "что игра может не принять это слово.\n\n" +
               "Просто напиши мне одно или несколько слов одним сообщением через пробел или запятую, а я сделаю всё необходимое!",
               replyMarkup: replyKeyboardMarkup,
               cancellationToken: cancellationToken);
            */

            Message sendMessage = await botClient.SendTextMessageAsync(
               chatId: chatId,
               text: "Опция на стадии разработки!",
               cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Handles "Вернуться в режим поиска слов" click.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        private async Task HandleBackToWordsSearchMode(ITelegramBotClient botClient, CancellationToken cancellationToken, long chatId)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Добавить слово в словарь"},
                new KeyboardButton[] { "Отметить слово, которое не знает игра" },
            })
            {
                ResizeKeyboard = true
            };

            Message sendGreetingMessage = await botClient.SendTextMessageAsync(
               chatId: chatId,
               text: "Я готов к новому поиску слов!",
               replyMarkup: replyKeyboardMarkup,
               cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Handles errors.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="exception"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine($"Exception caught: {ErrorMessage}");
            return Task.CompletedTask;
        }
        #endregion

        /// <summary>
        /// Generates a response to the user.
        /// </summary>
        /// <param name="messageText"></param>
        /// <returns>the result of permutations if user's input is valid; otherwise, error message</returns>
        private string GenerateResponse(string messageText)
        {
            if (wordsBuilder.FindWords(messageText.Replace(" ", "").ToLower())) //Delete spaces from user's input and convert it to the lowercase
            {
                if (wordsBuilder.FoundWordsAll.Count == 0)
                    return $"Мне не удалось найти ни одного слова{char.ConvertFromUtf32(0x1F610)}";
                else
                    return "Все найденные слова:\n" + string.Join(", ", wordsBuilder.FoundWordsAll) +
                           "\n\nСлова, которые игра может НЕ знать:\n" + string.Join(", ", wordsBuilder.FoundWordsLessCommon);
            }
            else
                return "Упс, похоже у нас тут ошибка ввода! Напоминаю:\n\n" +
                       "- не может быть менее трёх или более семи букв;\n" +
                       "- допустимы только буквенные значения;\n" +
                       "- допустимы только буквы из кириллического алфавита.\n\n" +
                       $"Проверь и попробуй ещё раз{char.ConvertFromUtf32(0x1F601)}";
        }
    }
}