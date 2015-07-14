using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using 随身袋.MyControls;

namespace 随身袋.Helper
{
    public class SREngine
    {
        private SpeechRecognitionEngine recognizer;

        public delegate void SpeRec(string saytext);
        public event SpeRec SpeRecSay;
        public SREngine(string name, string[] choices)
        {
            try
            {
                //zh-CN
                recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("zh-CN"));
                //var myCIintl = new System.Globalization.CultureInfo("zh-CN");
                //foreach (RecognizerInfo config in SpeechRecognitionEngine.InstalledRecognizers())//获取所有语音引擎  
                //{
                //    if (config.Culture.Equals(myCIintl) && config.Id == "MS-2052-80-DESK")
                //    {
                //        recognizer = new SpeechRecognitionEngine(config);
                //        break;
                //    }//选择识别引擎
                //}

                // Create and load a dictation grammar.
                GrammarBuilder GB = new GrammarBuilder();//自然语法
                GB.Append(name);
                GB.Append(new Choices(choices));
                Grammar G = new Grammar(GB);
                recognizer.LoadGrammar(G);

                // Add a handler for the speech recognized event.
                recognizer.SpeechRecognized += recognizer_SpeechRecognized;

                // Configure input to the speech recognizer.
                recognizer.SetInputToDefaultAudioDevice();

                // Start asynchronous, continuous speech recognition.
                //recognizer.RecognizeAsync(RecognizeMode.Multiple);
                //识别模式为连续识别  
            }
            catch
            {
                throw new Exception("启用语音失败");
            }

        }

        void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (SpeRecSay != null)
            {
                SpeRecSay(e.Result.Text);
            }
        }


        public void Start()
        {
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        public void Stop()
        {
            recognizer.RecognizeAsyncStop();
        }
    }
}
