using System;
using Telegram.Bot;
using System.Speech;
using System.Speech.Synthesis;
using System.IO;
using Telegram.Bot.Types.InputFiles;
using System.Threading.Tasks;
using System.Speech.AudioFormat;
using NAudio.Wave;
using NAudio.Lame;
using NAudio.Utils;
namespace speakBot
{
    class Program
    {
        static TelegramBotClient botClient;
        static void Main(string[] args)
        {

            new Program().Start().GetAwaiter().GetResult();
        }
        public async Task Start()
        {
            string token = "";
            token = "966526450:AAFTPo1skMyQjxcDSMINLdcRR8gTO8NUpHo";
            botClient = new TelegramBotClient(token);

            botClient.OnMessage += BotClient_OnMessage;
            botClient.StartReceiving();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            botClient.StopReceiving();
        }

        private async void BotClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {

            try
            {
                Console.WriteLine("START");
                string text = e.Message.Text;
                Console.WriteLine(text);
                if (string.IsNullOrEmpty(text)) return;
                if (!text.Contains("/sp", StringComparison.OrdinalIgnoreCase)) return;
                string speckText = text.Replace("/sp", "");
                SpeechSynthesizer synth = new SpeechSynthesizer();

                using (MemoryStream memory = new MemoryStream())
                {
                    synth.SetOutputToWaveStream(memory);

                    Console.WriteLine($"Speak {speckText}");
                    synth.Speak(speckText.Trim(' '));
                    Console.WriteLine("SPEAK");
                    await Task.Delay(500);

                    var obj = ConvertToMp3(memory);
                    // await botClient.SendTextMessageAsync(e.Message.Chat.Id, "dsds");
                    await botClient.SendVoiceAsync(e.Message.Chat.Id, new InputOnlineFile(new MemoryStream(obj), "audio.mp3"));
                }
            }
            catch
            (Exception ex) 
          { Console.WriteLine(ex.Message); }
            
        }
        byte[] ConvertToMp3(MemoryStream file)
        {
            var mem = new MemoryStream(file.ToArray());
            var target = new WaveFormat(8000, 16, 1);
            using (var outPutStream = new MemoryStream())
            using (var waveStream = new WaveFileReader(mem))
            using (var conversionStream = new WaveFormatConversionStream(target, waveStream))
            using (var writer = new LameMP3FileWriter(outPutStream, conversionStream.WaveFormat, 32, null))
            {
                conversionStream.CopyTo(writer);

                return outPutStream.ToArray();
            }

        }
    }
}
