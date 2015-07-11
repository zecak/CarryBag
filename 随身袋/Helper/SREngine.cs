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
        private SpeechRecognitionEngine SRE;
        public SREngine(string name,string[] choices)
        {
            try
            {
                var config = SpeechRecognitionEngine.InstalledRecognizers().FirstOrDefault(m => m.Id == "MS-2052-80-DESK");//中文引擎配置,必须的
                SRE = new SpeechRecognitionEngine(config);//使用中文引擎
                SRE.SetInputToDefaultAudioDevice();//录音设备(麦克风)的[默认设备],注意是[默认设备],不是[默认通信设备],不然没效果
                GrammarBuilder GB = new GrammarBuilder();//自然语法
                GB.Append(name);
                GB.Append(new Choices(choices));
                Grammar G = new Grammar(GB);
                G.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(G_SpeechRecognized);
                SRE.LoadGrammar(G);
                SRE.RecognizeAsync(RecognizeMode.Multiple); //<=======异步调用识别引擎，允许多次识别（否则程序只响应你的一句话）
            }
            catch
            {
                throw new Exception("启用语音失败");
            }
           
        }

        public delegate void SpeRec(string saytext);
        public event SpeRec SpeRecSay;

        void G_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if(SpeRecSay!=null)
            {
                SpeRecSay(e.Result.Text);
            }
        }

        public void Start()
        {
            SRE.RecognizeAsync(RecognizeMode.Multiple);
        }

        public void Stop()
        {
            SRE.RecognizeAsyncStop();
        }
    }
}
