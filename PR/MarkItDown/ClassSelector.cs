using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Common;

namespace MarkItDown
{
    public class ClassSelector : Selector
    {
        private const int BtnPerLine = 4;
        private readonly string _rootFolder;
        private const string ClassesFolder = "classes\\";

        private readonly Dictionary<string, List<string>> _classes;
        
        public ClassSelector(string rootFolder)
        {
            _rootFolder = rootFolder;
            
            CreatePictureBox();
            
            _classes = BuildClasses();

            BuildClassesButtons();
        }

        private string ClassesDir
        {
            get { return _rootFolder + "\\" + ClassesFolder; }
        }

        private void BuildClassesButtons()
        {
            var startingX = _pictureBox.Size.Width + BtnSize;
            int x = startingX, y = BtnStart;
            var btnSize = BtnPadding * BtnPerLine + x + BtnSize;
            
            var countItemsSize = _classes.Aggregate(0, (c, kvp) => c+=kvp.Value.Count+BtnPerLine) / BtnPerLine + 1;
            var btnLineSize = countItemsSize * BtnPadding + y;

            Size = new Size(Math.Max(btnSize, _pictureBox.Size.Width), btnLineSize);

            var tempImgExtension = Path.GetExtension(Paths.TempImg);
            int lineCount = 0;
            foreach (var @class in _classes)
            {
                foreach (var item in @class.Value)
                {
                    Button b = new Button();
                    b.Text = ReplaceAsCardColor(item);
                    
                    if (File.Exists(ClassesDir + string.Format("{0}\\{1}", @class.Key, item) + tempImgExtension))
                    {
                        b.BackColor = Color.Gray;
                    }
                    
                    b.Size = new Size(BtnSize, BtnSize);
                    b.Location = new Point(x, y);
                    b.Tag = string.Format("{0}\\{1}", @class.Key, item);
                    b.Click += (sender, e) =>
                    {
                        var tag = ((Button) sender).Tag as string;
                        var tagItems = tag.Split('\\');
                        var classPath = ClassesDir + tagItems[0];
                        if (!Directory.Exists(classPath))
                        {
                            Directory.CreateDirectory(classPath);
                        }

                        File.Copy(Paths.TempImg, ClassesDir + tag + tempImgExtension, true);

                        Close();
                    };
                    Controls.Add(b);
                    x += BtnPadding;
                    lineCount++;
                    if (lineCount >= BtnPerLine)
                    {
                        x = startingX;
                        y += BtnPadding;
                        lineCount = 0;
                    }
                }

                lineCount = 0;
                x = startingX;
                y += BtnPadding;
            }
        }

        private string ReplaceAsCardColor(string name)
        {
            return name.Replace('c', '♣').Replace('s','♠').Replace('h','♥').Replace('d','♦');
        }

        private static Dictionary<string, List<string>> BuildClasses()
        {
            var lines = File.ReadAllLines(Paths.Classes);
            Dictionary<string, List<string>> classes = new Dictionary<string, List<string>>();
            foreach (var line in lines)
            {
                var item = line.Split('\\');

                if (classes.ContainsKey(item[0]))
                {
                    classes[item[0]].Add(item[1]);
                }
                else
                {
                    classes[item[0]] = new List<string>() {item[1]};
                }
            }

            return classes;
        }
    }
}