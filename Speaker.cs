using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace termFinal
{
    internal class Speaker
    {
        public void Speak(string content)
        {
                Task.Run(() =>
                {
                    SpeechSynthesizer speech = new SpeechSynthesizer();
                    speech.Rate = 1;
                    speech.SelectVoice("Microsoft Huihui Desktop");// 设置播音员
                    speech.Volume = 100;
                    speech.Speak(content);
                });
            }

        }
    }

