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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.S)
            {
                ShowClassesPreview();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ShowClassesPreview()
        {
            var form = CreatePreviewForm();
            var panel = CreateScrollablePanel();
            
            int yPos = 10;
            foreach (var classGroup in _classes)
            {
                yPos = AddClassHeader(panel, classGroup.Key, yPos);
                yPos = AddClassItems(panel, classGroup, yPos);
                yPos += 20; // Extra space between classes
            }

            form.Controls.Add(panel);
            form.Show();
        }

        private Form CreatePreviewForm()
        {
            return new Form
            {
                Size = new Size(800, 600),
                AutoScroll = true
            };
        }

        private Panel CreateScrollablePanel()
        {
            return new Panel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill
            };
        }

        private int AddClassHeader(Panel panel, string className, int yPos)
        {
            var classLabel = new Label
            {
                Text = className,
                Font = new Font(Font, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            panel.Controls.Add(classLabel);
            return yPos + 30;
        }

        private int AddClassItems(Panel panel, KeyValuePair<string, List<string>> classGroup, int yPos)
        {
            bool wasAnyImage = false;
            foreach (var item in classGroup.Value)
            {
                string imagePath = Path.Combine(ClassesDir, classGroup.Key, item + Path.GetExtension(Paths.TempImg));
                if (File.Exists(imagePath))
                {
                    wasAnyImage = true;
                    yPos = AddClassItem(panel, item, imagePath, yPos);
                }
            }

            if (!wasAnyImage)
            {
                yPos = AddNoClassesLabel(panel, yPos);
            }

            return yPos;
        }

        private int AddClassItem(Panel panel, string item, string imagePath, int yPos)
        {
            var controls = CreateClassItemControls(item, imagePath, yPos);
            
            SetupDeleteButtonHandler(controls.DeleteButton, controls.PictureBox, controls.Label, panel, imagePath);
            
            AddControlsToPanel(panel, controls);

            return CalculateNextYPosition(yPos, controls.PictureBox.Size.Height);
        }

        private (Label Label, PictureBox PictureBox, Button DeleteButton) CreateClassItemControls(string item, string imagePath, int yPos)
        {
            var label = new Label
            {
                Text = ReplaceAsCardColor(item),
                Location = new Point(30, yPos),
                AutoSize = true
            };

            var image = Image.FromFile(imagePath);
            var pictureBox = new PictureBox
            {
                Image = image,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Location = new Point(200, yPos)
            };

            var deleteButton = new Button
            {
                Text = "❌",
                Size = new Size(25, 25),
                Location = new Point(pictureBox.Right + 10, yPos),
                TextAlign = ContentAlignment.MiddleCenter
            };

            return (label, pictureBox, deleteButton);
        }

        private void SetupDeleteButtonHandler(Button deleteButton, PictureBox pictureBox, Label label, Panel panel, string imagePath)
        {
            deleteButton.Click += (s, e) =>
            {
                if (ConfirmDeletion())
                {
                    DeleteClassItem(imagePath, panel, pictureBox, label, deleteButton);
                }
            };
        }

        private bool ConfirmDeletion()
        {
            return MessageBox.Show(
                "Are you sure you want to delete this class image?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void DeleteClassItem(string imagePath, Panel panel, PictureBox pictureBox, Label label, Button deleteButton)
        {
            pictureBox.Image?.Dispose();
            pictureBox.Image = null;
            
            File.Delete(imagePath);
            panel.Controls.Remove(pictureBox);
            panel.Controls.Remove(label);
            panel.Controls.Remove(deleteButton);
        }

        private void AddControlsToPanel(Panel panel, (Label Label, PictureBox PictureBox, Button DeleteButton) controls)
        {
            panel.Controls.Add(controls.Label);
            panel.Controls.Add(controls.PictureBox);
            panel.Controls.Add(controls.DeleteButton);
        }

        private int CalculateNextYPosition(int currentYPos, int pictureBoxHeight)
        {
            return currentYPos + pictureBoxHeight + 10;
        }

        private int AddNoClassesLabel(Panel panel, int yPos)
        {
            var noClassesLabel = new Label
            {
                Text = "No classes defined",
                Location = new Point(30, yPos),
                AutoSize = true
            };
            panel.Controls.Add(noClassesLabel);
            return yPos + 20;
        }

        private string ClassesDir
        {
            get { return _rootFolder + "\\" + ClassesFolder; }
        }

        private void BuildClassesButtons()
        {
            var (startingX, btnSize, btnLineSize) = CalculateButtonLayoutDimensions();
            Size = new Size(Math.Max(btnSize, _pictureBox.Size.Width), btnLineSize);

            var buttonPosition = new Point(startingX, BtnStart);
            var lineCount = 0;

            foreach (var @class in _classes)
            {
                foreach (var item in @class.Value)
                {
                    CreateAndAddButton(@class.Key, item, buttonPosition);
                    UpdateButtonPosition(ref buttonPosition, ref lineCount, startingX);
                }

                ResetPositionForNewClass(ref buttonPosition, ref lineCount, startingX);
            }
        }

        private (int startingX, int btnSize, int btnLineSize) CalculateButtonLayoutDimensions()
        {
            var startingX = _pictureBox.Size.Width + BtnSize;
            var btnSize = BtnPadding * BtnPerLine + startingX + BtnSize;
            
            var countItemsSize = _classes.Aggregate(0, (c, kvp) => c += kvp.Value.Count + BtnPerLine) / BtnPerLine + 1;
            var btnLineSize = countItemsSize * BtnPadding + BtnStart;

            return (startingX, btnSize, btnLineSize);
        }

        private void CreateAndAddButton(string className, string item, Point position)
        {
            var button = new Button
            {
                Text = ReplaceAsCardColor(item),
                Size = new Size(BtnSize, BtnSize),
                Location = position,
                Tag = $"{className}\\{item}"
            };

            if (IsImageExists(className, item))
            {
                button.BackColor = Color.Gray;
            }

            button.Click += ButtonClick;
            Controls.Add(button);
        }

        private bool IsImageExists(string className, string item)
        {
            var tempImgExtension = Path.GetExtension(Paths.TempImg);
            return File.Exists(ClassesDir + $"{className}\\{item}" + tempImgExtension);
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            var tag = ((Button)sender).Tag as string;
            var tagItems = tag.Split('\\');
            var classPath = ClassesDir + tagItems[0];
            
            EnsureDirectoryExists(classPath);
            SaveImage(tag);
            Close();
        }

        private void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void SaveImage(string tag)
        {
            var tempImgExtension = Path.GetExtension(Paths.TempImg);
            File.Copy(Paths.TempImg, ClassesDir + tag + tempImgExtension, true);
        }

        private void UpdateButtonPosition(ref Point position, ref int lineCount, int startingX)
        {
            position.X += BtnPadding;
            lineCount++;
            if (lineCount >= BtnPerLine)
            {
                position.X = startingX;
                position.Y += BtnPadding;
                lineCount = 0;
            }
        }

        private void ResetPositionForNewClass(ref Point position, ref int lineCount, int startingX)
        {
            lineCount = 0;
            position.X = startingX;
            position.Y += BtnPadding;
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