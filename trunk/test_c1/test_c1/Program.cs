using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace test_c1
{
    class Program
    {        /// <summary>
        /// 3,10,12,13
        /// </summary>
        public static KeyValuePair<string, List<string>> kvShui = new KeyValuePair<string, List<string>>("水", new List<string>() { "凡 下 子", "呗 班 般 豹 倍 毕 表 亳 洞 娥 舫 肥 匪 纷 峰 俸 服 俯 釜 害 函 航 耗 狠 恨 哼 恒 洪 候 活 津 酒 洛 马 邙", "悲 备 博 淡 饭 斐 复 富 涵 寒 喝 贺 惠 混 惑 渐 荆 凉 买 茫 帽 媒 媚 寐 闷 闵 牌 评 迫 普 浅 清 深 淑 淘 雯 无 喜 闲 现 项 涯 淹 渊 云 粥", "湖 渴 雷 盟 迷 渺 莫 媲 瓶 惹 汤 微 熙 湘 游 煮" });

        /// <summary>
        /// 3,10,12,13
        /// </summary>
        public static KeyValuePair<string, List<string>> kvMu = new KeyValuePair<string, List<string>>("木", new List<string>() { "久 口 廿 已 弓 及", "桔 珂 栩 哭 库 栗 匿 栖 岂 起 虔 芹 桑 桃 柏 栢 苄 芳 芬 粉 芙 羔 高 格 根 恭 贡 骨 鬼 桂 核 桓 恢 姬 记 家 兼 桀 芥 衿 径 奚 校 芯 桠 芽 芫 倚 原 芸 芷 桎 株 桌", "杰 景 开 凯 轲 棵 植 棱 荔 络 棉 茗 期 棋 茜 笔 草 策 茶 棣 迭 栋 棼 贯 贵 极 集 几 间 荐 乔 球 荃 荏 茸 茹 阮 棠 稀 厦 荀 雅 雁 椅 茵 寓 栈 棹 茱 棕", "睛 敬 廉 夸 楞 莉 琳 莓 楣 募 楠 逆 睨 琪 枫 概 感 荷 嫁 拣 减 酱 郊 琴 倾 莎 颂 苋 莘 杨 楹 莜 愚 榆 预 御" });
        static void Main(string[] args)
        {
            var name1 = System.Web.HttpUtility.UrlEncodeUnicode("曾").Replace("%u", "$");
            var name2 = System.Web.HttpUtility.UrlEncodeUnicode("1").Replace("%u", "$");
            var name3 = System.Web.HttpUtility.UrlEncodeUnicode("2").Replace("%u", "$");

            var url = "http://www.qimingtong.com/evaluate/" + name1 + name2 + name3 + "0#eval_result_anchor";

            //.Split(' ')
            var shuiNames3 = kvShui.Value[0].Split(' ').ToList();
            var muNames10 = kvMu.Value[1].Split(' ').ToList();
            muNames10.AddRange(kvMu.Value[2].Split(' '));

            List<TextCName> ming1 = new List<TextCName>();
            foreach (var shui in shuiNames3)
            {
                foreach (var mu in muNames10)
                {
                    ming1.Add(new TextCName(shui,mu));
                }
            }

            var muNames3 = kvMu.Value[0].Split(' ').ToList();
            var shuiNames10 = kvShui.Value[1].Split(' ').ToList();
            shuiNames10.AddRange(kvShui.Value[2].Split(' '));
            List<TextCName> ming2 = new List<TextCName>();
            foreach (var mu in muNames3)
            {
                foreach (var shui in shuiNames10)
                {
                    ming2.Add(new TextCName(mu,shui));
                }
            }
            

            Console.WriteLine("ok");
            Console.ReadKey();
        }
    }
}
